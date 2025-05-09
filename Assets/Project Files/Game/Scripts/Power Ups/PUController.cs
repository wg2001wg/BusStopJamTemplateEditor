using System.Collections.Generic;
using UnityEngine;
using Watermelon.IAPStore;

namespace Watermelon
{
    public class PUController : MonoBehaviour
    {
        private static PUController instance;

        [DrawReference]
        [SerializeField] PUDatabase database;

        [LineSpacer("Sounds")]
        [SerializeField] AudioClip activateSound;

        public static PUBehavior[] ActivePowerUps { get; private set; }
        public static PUUIController PowerUpsUIController { get; private set; }
        public static PUBehavior SelectedPU { get; private set; }

        private static Dictionary<PUType, PUBehavior> powerUpsLink;

        public static event PowerUpCallback Used;
        public static event PowerUpCallback Unlocked;

        private Transform behaviorsContainer;

        public void Init()
        {
#if MODULE_POWERUPS
            instance = this;

            behaviorsContainer = new GameObject("[POWER UPS]").transform;
            behaviorsContainer.gameObject.isStatic = true;

            PUSettings[] powerUpSettings = database.PowerUps;
            ActivePowerUps = new PUBehavior[powerUpSettings.Length];
            powerUpsLink = new Dictionary<PUType, PUBehavior>();

            for (int i = 0; i < ActivePowerUps.Length; i++)
            {
                // Initialize power ups
                powerUpSettings[i].InitializeSave();
                powerUpSettings[i].Init();

                // Spawn behavior object 
                GameObject powerUpBehaviorObject = Instantiate(powerUpSettings[i].BehaviorPrefab, behaviorsContainer);
                powerUpBehaviorObject.transform.ResetLocal();

                PUBehavior powerUpBehavior = powerUpBehaviorObject.GetComponent<PUBehavior>();
                powerUpBehavior.InitializeSettings(powerUpSettings[i]);
                powerUpBehavior.Init();

                ActivePowerUps[i] = powerUpBehavior;

                // Add power up to dictionary
                powerUpsLink.Add(ActivePowerUps[i].Settings.Type, ActivePowerUps[i]);
            }

            UIGame gameUI = UIController.GetPage<UIGame>();

            PowerUpsUIController = gameUI.PowerUpsUIController;
            PowerUpsUIController.Init(this);
#else
            Debug.LogError("[PU Controller]: Module Define isn't active!");
#endif
        }

        public static bool PurchasePowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                if(powerUpBehavior.Settings.HasEnoughCurrency())
                {
                    CurrencyController.Substract(powerUpBehavior.Settings.CurrencyType, powerUpBehavior.Settings.Price);

                    powerUpBehavior.Settings.Save.Amount += powerUpBehavior.Settings.PurchaseAmount;

                    PowerUpsUIController.RedrawPanels();

                    return true;
                }
                else
                {
                    UIController.ShowPage<UIStore>();

                    return false;
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }

            return false;
        }

        public static void AddPowerUp(PUType powerUpType, int amount)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount += amount;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static void SetPowerUpAmount(PUType powerUpType, int amount)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount = amount;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static bool UsePowerUp(PUType powerUpType)
        {
            if(powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                if(!powerUpBehavior.IsBusy)
                {
                    if(powerUpBehavior.Activate())
                    {
                        PUSettings settings = powerUpBehavior.Settings;

                        AudioController.PlaySound(settings.CustomAudioClip.Handle(instance.activateSound));

                        settings.Save.Amount--;

                        PowerUpsUIController.OnPowerUpUsed(powerUpBehavior);

                        Used?.Invoke(powerUpType);

                        return true;
                    }
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }

            return false;
        }

        public static void ResetPowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];

                powerUpBehavior.Settings.Save.Amount = 0;

                PowerUpsUIController.RedrawPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));
            }
        }

        public static void UnlockPowerUp(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                PUBehavior powerUpBehavior = powerUpsLink[powerUpType];
                PUSettings settings = powerUpBehavior.Settings;

                if(!settings.IsUnlocked)
                {
                    settings.IsUnlocked = true;

                    Unlocked?.Invoke(powerUpType);
                }
            }
        }

        public static void ResetPowerUps()
        {
            foreach(PUBehavior powerUp in ActivePowerUps)
            {
                powerUp.Settings.Save.Amount = 0;
            }

            PowerUpsUIController.RedrawPanels();
        }

        public static PUBehavior GetPowerUpBehavior(PUType powerUpType)
        {
            if (powerUpsLink.ContainsKey(powerUpType))
            {
                return powerUpsLink[powerUpType];
            }

            Debug.LogWarning(string.Format("[Power Ups]: Power up with type {0} isn't registered.", powerUpType));

            return null;
        }

        public static void ResetBehaviors()
        {
            for(int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].ResetBehavior();
            }
        }

        [Button("Give Test Amount")]
        public void GiveDebugAmount()
        {
            if (!Application.isPlaying) return;

            for(int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].Settings.Save.Amount = 999;
            }

            PowerUpsUIController.RedrawPanels();
        }

        [Button("Reset Amount")]
        public void ResetDebugAmount()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < ActivePowerUps.Length; i++)
            {
                ActivePowerUps[i].Settings.Save.Amount = 0;
            }

            PowerUpsUIController.RedrawPanels();
        }

        public delegate void PowerUpCallback(PUType powerUpType);
    }
}
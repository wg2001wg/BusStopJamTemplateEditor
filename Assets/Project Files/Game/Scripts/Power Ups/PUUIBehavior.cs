using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUUIBehavior : MonoBehaviour
    {
        [Group("Refs")]
        [SerializeField] Image backgroundImage;

        [Group("Refs")]
        [SerializeField] Image iconImage;

        [Group("Refs")]
        [SerializeField] GameObject defaultElementsObjects;

        [Group("Refs")]
        [SerializeField] GameObject amountContainerObject;

        [Group("Refs")]
        [SerializeField] TextMeshProUGUI amountText;

        [Group("Refs")]
        [SerializeField] GameObject amountPurchaseObject;

        [Group("Refs")]
        [SerializeField] GameObject busyStateVisualsObject;

        [Space]
        [SerializeField] GameObject timerObject;
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] Image timerBackground;

        [Space]
        [SerializeField] GameObject lockStateObject;
        [SerializeField] TextMeshProUGUI lockText;

        [Space]
        [SerializeField] SimpleBounce bounce;

        protected PUBehavior behavior;
        public PUBehavior Behavior => behavior;

        protected PUSettings settings;
        public PUSettings Settings => settings;

        private Button button;

        private bool isTimerActive;
        private Coroutine timerCoroutine;

        private bool isActive = false;
        public bool IsActive => isActive;

        private bool isLocked = false;
        public bool IsLocked => isLocked;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClicked());
        }

        public void Init(PUBehavior powerUpBehavior)
        {
            behavior = powerUpBehavior;
            settings = powerUpBehavior.Settings;

            ApplyVisuals();

            Redraw();

            bounce.Init(transform);

            gameObject.SetActive(false);

            isActive = false;
        }

        protected virtual void ApplyVisuals()
        {
            iconImage.sprite = settings.Icon;
            iconImage.color = Color.white;

            backgroundImage.color = settings.BackgroundColor;
        }

        public void SetBlockState(int levelNumber)
        {
            if(levelNumber >= settings.RequiredLevel)
            {
                lockStateObject.SetActive(false);

                defaultElementsObjects.SetActive(true);

                isLocked = false;
            }
            else
            {
                lockStateObject.SetActive(true);
                lockText.text = string.Format("LEVEL {0}", settings.RequiredLevel);

                defaultElementsObjects.SetActive(false);

                isLocked = true;
            }
        }

        public void Activate()
        {
            isActive = true;

            gameObject.SetActive(true);

            transform.localScale = Vector3.zero;
            transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);

            Redraw();
        }

        public void Disable()
        {
            isActive = false;

            gameObject.SetActive(false);
        }

        private IEnumerator TimerCoroutine(PUTimer timer)
        {
            isTimerActive = true;

            UIGame uiGame = UIController.GetPage<UIGame>();

            timerObject.SetActive(true);
            timerBackground.fillAmount = 1.0f;
            timerText.text = timer.Seconds;

            iconImage.color = new Color(1, 1, 1, 0.3f);

            while (timer.IsActive)
            {
                yield return null;
                yield return null;

                timerBackground.fillAmount = 1.0f - timer.State;
                timerText.text = timer.Seconds;

                if (timerBackground.fillAmount <= 0.0f)
                    break;
            }

            timerObject.SetActive(false);
            iconImage.color = Color.white;

            isTimerActive = false;
        }

        public void OnButtonClicked()
        {
            if (isLocked) return;

            if (settings.Save.Amount > 0)
            {
                if (!behavior.IsBusy)
                {
                    if (PUController.UsePowerUp(settings.Type))
                    {
                        AudioController.PlaySound(AudioController.AudioClips.buttonSound);

                        bounce.Bounce();
                    }
                }
            }
            else
            {
                AudioController.PlaySound(AudioController.AudioClips.buttonSound);

                PUController.PowerUpsUIController.PowerUpPurchasePanel.Show(settings);
            }
        }

        public void Redraw()
        {
            int amount = settings.Save.Amount;
            if (amount > 0)
            {
                amountContainerObject.SetActive(true);
                amountPurchaseObject.SetActive(false);

                amountText.text = amount.ToString();
            }
            else
            {
                amountContainerObject.SetActive(false);
                amountPurchaseObject.SetActive(true);
            }

            PUTimer timer = behavior.GetTimer();
            if (!isTimerActive)
            {
                if (timer != null)
                {
                    timerCoroutine = StartCoroutine(TimerCoroutine(timer));
                }
            }

            if (settings.VisualiseActiveState)
                RedrawBusyVisuals(behavior.IsBusy);

            behavior.OnRedrawn();
        }

        protected virtual void RedrawBusyVisuals(bool state)
        {
            busyStateVisualsObject.SetActive(behavior.IsBusy);
        }
    }
}

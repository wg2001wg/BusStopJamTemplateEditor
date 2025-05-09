using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon.BusStop;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] RectTransform safeZoneTransform;
        [SerializeField] PUUIController powerUpsUIController;
        public PUUIController PowerUpsUIController => powerUpsUIController;

        [SerializeField] Button replayButton;
        [SerializeField] UILevelQuitPopUp quitPopUp;

        [SerializeField] TextMeshProUGUI levelText;
        private UIScaleAnimation levelTextScaleAnimation;

        [Space(5f)]
        [SerializeField] GameObject devOverlay;

        public override void Init()
        {
            levelTextScaleAnimation = new UIScaleAnimation(levelText.rectTransform);

            devOverlay.SetActive(DevPanelEnabler.IsDevPanelDisplayed());

            replayButton.onClick.AddListener(OnReplayButtonClicked);

            NotchSaveArea.RegisterRectTransform(safeZoneTransform);

            quitPopUp.OnCancelExitEvent += ExitPopCloseButton;
            quitPopUp.OnConfirmExitEvent += ExitPopUpConfirmExitButton;
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            TutorialCanvasController.SetParent(transform);

            UpdateLevelNumber();

            levelTextScaleAnimation.Show(scaleMultiplier: 1.05f, immediately: true);

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            levelTextScaleAnimation.Hide(scaleMultiplier: 1.05f, immediately: false);

            quitPopUp.Hide();

            UIController.OnPageClosed(this);
        }

        #endregion

        private void OnReplayButtonClicked()
        {
            if (LivesSystem.InfiniteMode)
            {
                ExitPopUpConfirmExitButton();
            }
            else
            {
                quitPopUp.Show();
            }
        }

        public void ExitPopCloseButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            quitPopUp.Hide();
        }

        public void ExitPopUpConfirmExitButton()
        {
            quitPopUp.Hide();

            LivesSystem.UnlockLife(true);

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            UIController.HidePage<UIGame>();

            GameController.ReplayLevel();
        }

        private void UpdateLevelNumber()
        {
            levelText.text = string.Format("LEVEL {0}", LevelController.DisplayLevelNumber + 1);
        }

        #region Development

        public void ReloadDev()
        {
            ReflectionUtils.InjectInstanceComponent<GameController>("isGameActive", false, ReflectionUtils.FLAGS_STATIC_PRIVATE);

            GameController.RefreshLevelDev();
        }

        public void HideDev()
        {
            devOverlay.SetActive(false);
        }

        public void OnLevelInputUpdatedDev(string newLevel)
        {
            int level = -1;

            if (int.TryParse(newLevel, out level))
            {
                ReflectionUtils.InjectInstanceComponent<GameController>("isGameActive", false, ReflectionUtils.FLAGS_STATIC_PRIVATE);

                LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
                levelSave.DisplayLevelNumber = Mathf.Clamp((level - 1), 0, int.MaxValue);
                levelSave.RealLevelNumber = levelSave.DisplayLevelNumber;

                GameController.RefreshLevelDev();
            }
        }

        public void PrevLevelDev()
        {
            ReflectionUtils.InjectInstanceComponent<GameController>("isGameActive", false, ReflectionUtils.FLAGS_STATIC_PRIVATE);

            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelNumber = Mathf.Clamp(levelSave.DisplayLevelNumber - 1, 0, int.MaxValue);
            levelSave.RealLevelNumber = levelSave.DisplayLevelNumber;

            GameController.RefreshLevelDev();
        }

        public void NextLevelDev()
        {
            ReflectionUtils.InjectInstanceComponent<GameController>("isGameActive", false, ReflectionUtils.FLAGS_STATIC_PRIVATE);

            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelNumber = levelSave.DisplayLevelNumber + 1;
            levelSave.RealLevelNumber = levelSave.DisplayLevelNumber;

            GameController.RefreshLevelDev();
        }

        #endregion
    }
}

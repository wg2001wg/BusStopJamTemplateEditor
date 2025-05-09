using TMPro;
using UnityEngine;
using Watermelon.BusStop;
using Watermelon.IAPStore;
using Watermelon.SkinStore;

namespace Watermelon
{
    public class UIMainMenu : UIPage
    {
        public readonly float STORE_AD_RIGHT_OFFSET_X = 300F;

        [Space]
        [SerializeField] RectTransform safeZoneTransform;
        [SerializeField] UIScaleAnimation coinsLabelScalable;
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [SerializeField] TextMeshProUGUI levelText;
        private UIScaleAnimation levelTextScaleAnimation;

        [Space]
        [SerializeField] UIMainMenuButton iapStoreButton;
        [SerializeField] UIMainMenuButton noAdsButton;
        [SerializeField] UIMainMenuButton skinsButton;

        [Space]
        [SerializeField] UINoAdsPopUp noAdsPopUp;
        public UINoAdsPopUp NoAdsPopUp => noAdsPopUp;

        private TweenCase showHideStoreAdButtonDelayTweenCase;

        private void OnEnable()
        {
            AdsManager.ForcedAdDisabled += ForceAdPurchased;
        }

        private void OnDisable()
        {
            AdsManager.ForcedAdDisabled -= ForceAdPurchased;
        }

        public override void Init()
        {
            levelTextScaleAnimation = new UIScaleAnimation(levelText.rectTransform);

            coinsPanel.Init();
            coinsPanel.AddButton.onClick.AddListener(IAPStoreButton);

            iapStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);
            noAdsButton.Init(STORE_AD_RIGHT_OFFSET_X);
            skinsButton.Init(STORE_AD_RIGHT_OFFSET_X);

            iapStoreButton.Button.onClick.AddListener(IAPStoreButton);
            noAdsButton.Button.onClick.AddListener(NoAdButton);
            skinsButton.Button.onClick.AddListener(SkinsButton);

            NotchSaveArea.RegisterRectTransform(safeZoneTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            TutorialCanvasController.SetParent(transform);

            UpdateLevelNumber();

            showHideStoreAdButtonDelayTweenCase.KillActive();

            HideAdButton(true);
            iapStoreButton.Hide(true);
            skinsButton.Hide(true);

            levelTextScaleAnimation.Show(scaleMultiplier: 1.05f, immediately: false);
            coinsLabelScalable.Show(immediately: true);

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.05f, delegate
            {
                ShowAdButton();
                iapStoreButton.Show();
                skinsButton.Show();
            });

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            showHideStoreAdButtonDelayTweenCase.KillActive();

            coinsLabelScalable.Hide(immediately: true);
            levelTextScaleAnimation.Hide(scaleMultiplier: 1.05f, immediately: true);

            HideAdButton(true);

            iapStoreButton.Hide(true);
            skinsButton.Hide(true);

            UIController.OnPageClosed(this);
        }

        private void UpdateLevelNumber()
        {
            levelText.text = string.Format("LEVEL {0}", LevelController.DisplayLevelNumber + 1);
        }

        #endregion

        #region Ad Button Label

        private void ShowAdButton(bool immediately = false)
        {
            if (AdsManager.IsForcedAdEnabled())
            {
                noAdsButton.Show(immediately);
            }
            else
            {
                noAdsButton.Hide(immediately: true);
            }
        }

        private void HideAdButton(bool immediately = false)
        {
            noAdsButton.Hide(immediately);
        }

        private void ForceAdPurchased()
        {
            HideAdButton(immediately: true);
        }

        #endregion

        #region Buttons

        public void TapToPlayButton()
        {
            if (UIController.IsDisplayed<UISettings>()) return;

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            GameController.StartGame();
        }

        public void IAPStoreButton()
        {
            if (UIController.GetPage<UIStore>().IsPageDisplayed)
                return;

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapOrSkinsStoreClosed;


            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        public void SkinsButton()
        {
            if (UIController.GetPage<UISkinStore>().IsPageDisplayed)
                return;

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UISkinStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapOrSkinsStoreClosed;

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void OnIapOrSkinsStoreClosed(UIPage page, System.Type pageType)
        {
            if (pageType.Equals(typeof(UIStore)) || pageType.Equals(typeof(UISkinStore)))
            {
                UIController.PageClosed -= OnIapOrSkinsStoreClosed;

                UIController.ShowPage<UIMainMenu>();
            }
        }

        public void NoAdButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            noAdsPopUp.Show();
        }

        #endregion
    }


}

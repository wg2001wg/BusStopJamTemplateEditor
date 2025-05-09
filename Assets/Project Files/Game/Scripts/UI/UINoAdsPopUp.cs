using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UINoAdsPopUp : MonoBehaviour, IPopupWindow
    {
        [SerializeField] UIScaleAnimation panelScalable;
        [SerializeField] UIFadeAnimation backFade;
        [SerializeField] Button bigCloseButton;
        [SerializeField] Button smallCloseButton;
        [SerializeField] IAPButton removeAdsButton;

        public bool IsOpened => gameObject.activeSelf;

        private void Awake()
        {
            bigCloseButton.onClick.AddListener(ClosePanel);
            smallCloseButton.onClick.AddListener(ClosePanel);

            removeAdsButton.Init(ProductKeyType.NoAds);

            backFade.Hide(immediately: true);
            panelScalable.Hide(immediately: true);
        }

        private void PurcaseCompleted(ProductKeyType productKeyType)
        {
            if(productKeyType == ProductKeyType.NoAds)
            {
                AdsManager.DisableForcedAd();

                ClosePanel();
            }
        }

        public void Show()
        {
            bigCloseButton.interactable = true;
            smallCloseButton.interactable = true;

            gameObject.SetActive(true);
            backFade.Show(0.2f, onCompleted: () =>
            {
                panelScalable.Show(immediately: false, duration: 0.3f);
            });

            UIController.OnPopupWindowOpened(this);

            IAPManager.PurchaseCompleted += PurcaseCompleted;
        }

        private void ClosePanel()
        {
            bigCloseButton.interactable = false;
            smallCloseButton.interactable = false;
            
            backFade.Hide(0.2f);
            panelScalable.Hide(immediately: false, duration: 0.4f, onCompleted: () =>
            {
                gameObject.SetActive(false);
            });

            UIController.OnPopupWindowClosed(this);

            IAPManager.PurchaseCompleted -= PurcaseCompleted;
        }
    }
}
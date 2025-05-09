using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UICurrencyButton : MonoBehaviour
    {
        [SerializeField] CurrencyType currencyType;
        [SerializeField] DisableMode disableMode;

        [HideIf("IsColorMode")]
        [SerializeField] Sprite activeButtonSprite;
        [HideIf("IsColorMode")]
        [SerializeField] Sprite disabledButtonSprite;

        [HideIf("IsSpriteMode")]
        [SerializeField] Color activeButtonColor;
        [HideIf("IsSpriteMode")]
        [SerializeField] Color disabledButtonColor;

        [Space]
        [SerializeField] Button button;
        [SerializeField] Image buttonImage;
        [SerializeField] TMP_Text buttonText;
        [SerializeField] Image currencyImage;
        [SerializeField] CanvasGroup textAndIconCanvasGroup;

        private int currentPrice;
        private Currency currency;

        private bool isSubscribed;

        public event SimpleCallback Purchased;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Init(int price)
        {
            Init(price, currencyType);
        }

        public void Init(int price, CurrencyType currencyType)
        {
            this.currencyType = currencyType;
            this.currency = CurrencyController.GetCurrency(currencyType);

            button.onClick.AddListener(OnButtonClicked);

            currentPrice = price;

            currencyImage.sprite = currency.Icon;
            buttonText.text = currency.AmountFormatted;

            Subscribe();

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            // activate button
            if (currency.Amount >= currentPrice)
            {
                button.interactable = true;
                textAndIconCanvasGroup.alpha = 1f;

                if (disableMode == DisableMode.Sprite)
                {
                    buttonImage.sprite = activeButtonSprite;
                }
                else
                {
                    buttonImage.color = activeButtonColor;
                }
            }
            // disable button
            else
            {
                button.interactable = false;
                textAndIconCanvasGroup.alpha = 0.6f;

                if (disableMode == DisableMode.Sprite)
                {
                    buttonImage.sprite = disabledButtonSprite;
                }
                else
                {
                    buttonImage.color = disabledButtonColor;
                }
            }
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            if (currency == null)
                return;

            isSubscribed = true;
            currency.OnCurrencyChanged += OnCurrencyChanged;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            if (currency == null)
                return;

            isSubscribed = false;
            currency.OnCurrencyChanged -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged(Currency currency, int amountDifference)
        {
            UpdateVisuals();
        }

        private void OnButtonClicked()
        {
#if MODULE_HAPTIC
            Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            if (CurrencyController.HasAmount(currencyType, currentPrice))
            {
                CurrencyController.Substract(currencyType, currentPrice);

                Purchased?.Invoke();
            }
        }

        #region Editor

        private enum DisableMode
        {
            Sprite = 0,
            Color = 1,
        }

        private bool IsColorMode()
        {
            return disableMode == DisableMode.Color;
        }

        private bool IsSpriteMode()
        {
            return disableMode == DisableMode.Sprite;
        }

        #endregion
    }
}
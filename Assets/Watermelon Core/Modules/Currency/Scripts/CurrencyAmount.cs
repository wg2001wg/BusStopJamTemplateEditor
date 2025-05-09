using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class CurrencyAmount
    {
        private const string TEXT_FORMAT = "<sprite name={0}>{1}";

        [SerializeField] CurrencyType currencyType;
        public CurrencyType CurrencyType => currencyType;

        [SerializeField] int amount;
        public int Amount => amount;

        public Currency Currency => CurrencyController.GetCurrency(currencyType);
        public string FormattedPrice => CurrencyHelper.Format(amount);

        public CurrencyAmount(CurrencyType currencyType, int amount)
        {
            this.currencyType = currencyType;
            this.amount = amount;
        }

        public bool EnoughMoneyOnBalance()
        {
            return CurrencyController.HasAmount(currencyType, amount);
        }

        public void SubstractFromBalance()
        {
            CurrencyController.Substract(currencyType, amount);
        }

        public void AddToBalance()
        {
            CurrencyController.Add(currencyType, amount);
        }

        public string GetTextWithIcon()
        {
            return string.Format(TEXT_FORMAT, currencyType, amount);
        }
    }
}

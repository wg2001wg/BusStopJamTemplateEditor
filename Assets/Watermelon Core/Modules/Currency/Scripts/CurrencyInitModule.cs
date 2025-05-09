using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Currencies", false)]
    public class CurrencyInitModule : InitModule
    {
        public override string ModuleName => "Currencies";

        [SerializeField] CurrencyDatabase currenciesDatabase;
        public CurrencyDatabase Database => currenciesDatabase;

        public override void CreateComponent()
        {
            CurrencyController.Init(currenciesDatabase);
        }
    }
}

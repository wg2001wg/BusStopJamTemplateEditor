using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Currency Database", menuName = "Data/Core/Currency Database")]
    public class CurrencyDatabase : ScriptableObject
    {
        [SerializeField] Currency[] currencies;
        public Currency[] Currencies => currencies;
    }
}
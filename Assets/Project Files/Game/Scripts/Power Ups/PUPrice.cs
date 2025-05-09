using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PUPrice
    {
        [SerializeField] PUType powerUpType;
        public PUType PowerUpType => powerUpType;

        [SerializeField] int amount;
        public int Amount => amount;
    }
}

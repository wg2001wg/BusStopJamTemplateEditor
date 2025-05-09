using TMPro;
using UnityEngine;

namespace Watermelon
{
    public class LivesReward : Reward
    {
        [SerializeField] int livesAmount = 1;

        [Space]
        [SerializeField] TextMeshProUGUI amountText;

        public override void Init()
        {
            if (amountText != null)
            {
                amountText.text = livesAmount.ToString();
            }
        }

        public override void ApplyReward()
        {
            LivesSystem.AddLife(livesAmount, true);
        }
    }
}

using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Power Ups Database", menuName = "Data/Power Ups/Database")]
    public class PUDatabase : ScriptableObject
    {
        [SerializeField] PUSettings[] powerUps;
        public PUSettings[] PowerUps => powerUps;
    }
}

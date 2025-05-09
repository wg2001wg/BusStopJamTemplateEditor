using UnityEngine;

namespace Watermelon.BusStop
{
    [System.Serializable]
    public class TileData
    {
        [SerializeField] GameObject cellPrefab;
        public GameObject CellPrefab => cellPrefab;

        [SerializeField] GameObject borderPrefab;
        public GameObject BorderPrefab => borderPrefab;

        [SerializeField] GameObject innerCornerPrefab;
        public GameObject InnerCornerPrefab => innerCornerPrefab;

        [SerializeField] GameObject outerCornerPrefab;
        public GameObject OuterCornerPrefab => outerCornerPrefab;
    }
}
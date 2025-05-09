using UnityEngine;
using Watermelon.BusStop;

namespace Watermelon
{
    [System.Serializable]
    public class BusSkinData : AbstractSkinData
    {
        [SkinPreview]
        [SerializeField] Sprite previewSprite;
        public Sprite PreviewSprite => previewSprite;

        [SerializeField] BusPrefab[] busData;
        public BusPrefab[] BusData => busData;

        [System.Serializable]
        public class BusPrefab
        {
            public GameObject prefab;
            public LevelElement.Type type;
        }
    }
}
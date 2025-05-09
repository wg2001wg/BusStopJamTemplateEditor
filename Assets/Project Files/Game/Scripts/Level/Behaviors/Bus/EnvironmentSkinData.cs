using UnityEngine;
using Watermelon.BusStop;

namespace Watermelon
{
    [System.Serializable]
    public class EnvironmentSkinData : AbstractSkinData
    {
        [SkinPreview]
        [SerializeField] Sprite previewSprite;
        public Sprite PreviewSprite => previewSprite;

        [SerializeField] GameObject environmentPrefab;
        public GameObject EnvironmentPrefab => environmentPrefab;

        [SerializeField] TileData tileData;
        public TileData TileData => tileData;
    }
}
using UnityEngine;
using Watermelon.BusStop;
using Watermelon.SkinStore;

namespace Watermelon
{
    public class GameController : MonoBehaviour
    {
        private static GameController gameController;

        [DrawReference]
        [SerializeField] GameData data;

        [Space]
        [SerializeField] UIController uiController;
        [SerializeField] MusicSource musicSource;

        private ParticlesController particlesController;
        private LevelController levelController;
        private TutorialController tutorialController;
        private PUController powerUpsController;
        private SkinController skinController;
        private SkinStoreController skinStoreController;

        private static bool isGameActive;
        public static bool IsGameActive => isGameActive;

        public static event SimpleCallback OnLevelChangedEvent;
        private static LevelSave levelSave;

        public static GameData Data => gameController.data;

        private void Awake()
        {
            gameController = this;

            levelSave = SaveController.GetSaveObject<LevelSave>("level");

            // Cache components
            CacheComponent(out particlesController);
            CacheComponent(out levelController);
            CacheComponent(out tutorialController);
            CacheComponent(out powerUpsController);
            CacheComponent(out skinController);
            CacheComponent(out skinStoreController);

            uiController.Init();

            particlesController.Init();

            tutorialController.Init();
            powerUpsController.Init();

            skinController.Init();
            skinStoreController.Init(skinController);

            musicSource.Init();
            musicSource.Activate();

            uiController.InitPages();

            // Add raycast controller component
            RaycastController raycastController = gameObject.AddComponent<RaycastController>();
            raycastController.Init();

            levelController.Init();
        }

        private void Start()
        {
            UIController.ShowPage<UIMainMenu>();

            LoadLevel(() =>
            {
                GameLoading.MarkAsReadyToHide();
            });
        }

        private void OnDestroy()
        {
            EnvironmentBehavior.UnloadSpawnedBusses();
        }

        private static void LoadLevel(System.Action OnComplete = null)
        {
            gameController.levelController.LoadLevel(() =>
            {
                OnComplete?.Invoke();
            });

            OnLevelChangedEvent?.Invoke();
        }

        public static void StartGame()
        {
            // On Level is loaded
            isGameActive = true;

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIGame>();

            LivesSystem.LockLife();
        }

        public static void LoseGame()
        {
            if (!isGameActive)
                return;

            isGameActive = false;

            RaycastController.Disable();

            UIController.HidePage<UIGame>();
            UIController.ShowPage<UIGameOver>();

            AudioController.PlaySound(AudioController.AudioClips.failSound);

            levelSave.ReplayingLevelAgain = true;
        }

        public static void WinGame()
        {
            if (!isGameActive)
                return;

            isGameActive = false;

            RaycastController.Disable();

            levelSave.ReplayingLevelAgain = false;

            LevelData completedLevel = LevelController.LoadedStageData;

            UIController.HidePage<UIGame>();
            UIController.ShowPage<UIComplete>();

            AudioController.PlaySound(AudioController.AudioClips.completeSound);

            SaveController.Save();
        }

        public static void LoadNextLevel()
        {
            if (isGameActive)
                return;

            gameController.levelController.AdjustLevelNumber();

            UIController.ShowPage<UIMainMenu>();

            levelSave.ReplayingLevelAgain = false;

            AdsManager.ShowInterstitial(null);

            LoadLevel();
        }

        public static void ReplayLevel()
        {
            isGameActive = false;

            UIController.ShowPage<UIMainMenu>();

            levelSave.ReplayingLevelAgain = true;

            AdsManager.ShowInterstitial(null);

            LoadLevel();
        }

        public static void RefreshLevelDev()
        {
            UIController.ShowPage<UIGame>();
            levelSave.ReplayingLevelAgain = true;

            LoadLevel();
        }

        private void OnApplicationQuit()
        {
            // to make sure we will load similar level next time game launched (in case we outside level bounds)
            levelSave.ReplayingLevelAgain = true;
        }

        #region Extensions
        public bool CacheComponent<T>(out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T)unboxedComponent;

                return true;
            }

            Debug.LogError(string.Format("Scripts Holder doesn't have {0} script added to it", typeof(T)));

            component = null;

            return false;
        }
        #endregion
    }
}
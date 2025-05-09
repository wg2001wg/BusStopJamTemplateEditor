#pragma warning disable 0067

using UnityEngine;
using Watermelon.IAPStore;
using Watermelon.SkinStore;

namespace Watermelon.BusStop
{
    public class RaycastController : MonoBehaviour
    {
        private UIStore iapStorePage;
        private UIMainMenu mainMenuPage;
        private UISkinStore storePage;

        private static bool isActive;

        public static event SimpleCallback OnInputActivated;
        public static event SimpleCallback OnMovementInputActivated;

        public void Init()
        {
            isActive = true;

            iapStorePage = UIController.GetPage<UIStore>();
            mainMenuPage = UIController.GetPage<UIMainMenu>();
            storePage = UIController.GetPage<UISkinStore>();
        }

        private void Update()
        {
            if (!isActive)
                return;

            if (InputController.ClickAction.WasPressedThisFrame() && !IsRaycastBlockedByUI() && !UIController.IsPopupOpened)
            {
                Ray ray = Camera.main.ScreenPointToRay(InputController.MousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    IClickableObject clickableObject = hit.transform.GetComponent<IClickableObject>();
                    if (clickableObject != null)
                    {
                        if (!GameController.IsGameActive)
                        {
                            if (LivesSystem.Lives > 0 || LivesSystem.InfiniteMode)
                            {
                                GameController.StartGame();
                                clickableObject.OnObjectClicked();
                            }
                            else
                            {
                                UIAddLivesPanel.Show((lifeRecieved) =>
                                {
                                    if (lifeRecieved)
                                    {
                                        GameController.StartGame();
                                        clickableObject.OnObjectClicked();
                                    }
                                });
                            }
                        }
                        else
                        {
                            clickableObject.OnObjectClicked();
                        }
                    }
                }
            }
        }

        private bool IsRaycastBlockedByUI()
        {
            return iapStorePage.IsPageDisplayed || storePage.IsPageDisplayed;
        }

        public static void Enable()
        {
            isActive = true;
            OnInputActivated?.Invoke();
        }

        public static void Disable()
        {
            isActive = false;
        }
    }
}

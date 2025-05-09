using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UILevelQuitPopUp : MonoBehaviour, IPopupWindow
    {
        [SerializeField] Button closeSmallButton;
        [SerializeField] Button closeBigButton;
        [SerializeField] Button confirmButton;

        public SimpleCallback OnCancelExitEvent;
        public SimpleCallback OnConfirmExitEvent;

        public bool IsOpened => gameObject.activeSelf;

        private void Awake()
        {
            closeSmallButton.onClick.AddListener(ExitPopCloseButton);
            closeBigButton.onClick.AddListener(ExitPopCloseButton);
            confirmButton.onClick.AddListener(ExitPopUpConfirmExitButton);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            UIController.OnPopupWindowOpened(this);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            UIController.OnPopupWindowClosed(this);
        }

        public void ExitPopCloseButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            OnCancelExitEvent?.Invoke();

            gameObject.SetActive(false);
        }

        public void ExitPopUpConfirmExitButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            OnConfirmExitEvent?.Invoke();

            gameObject.SetActive(false);
        }
    }
}

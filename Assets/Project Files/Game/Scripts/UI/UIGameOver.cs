using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGameOver : UIPage
    {
        [SerializeField] UIScaleAnimation levelFailedScalable;
        [SerializeField] UIScaleAnimation heartScalable;

        [SerializeField] UIFadeAnimation backgroundFade;

        [SerializeField] UIScaleAnimation replayButtonScalable;
        [SerializeField] Button replayButton;

        private TweenCase replayPingPongCase;

        public override void Init()
        {
            replayButton.onClick.AddListener(ReplayButton);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            levelFailedScalable.Hide(immediately: true);
            heartScalable.Hide(immediately: true);
            replayButtonScalable.Hide(immediately: true);

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            Tween.DelayedCall(fadeDuration * 0.8f, delegate { 
            
                levelFailedScalable.Show(scaleMultiplier: 1.1f);
                heartScalable.Show( scaleMultiplier: 1.1f);
                
                replayButtonScalable.Show(scaleMultiplier: 1.05f);

                replayPingPongCase = replayButtonScalable.Transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                UIController.OnPageOpened(this);
            });
        }

        public override void PlayHideAnimation()
        {
            backgroundFade.Hide(0.3f);

            Tween.DelayedCall(0.3f, delegate {

                if (replayPingPongCase != null && replayPingPongCase.IsActive) replayPingPongCase.Kill();

                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Buttons 

        public void ReplayButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            UIController.HidePage<UIGameOver>();

            LivesSystem.UnlockLife(true);

            GameController.ReplayLevel();
        }

        #endregion
    }
}
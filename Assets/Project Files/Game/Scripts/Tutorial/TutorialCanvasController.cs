using UnityEngine;

namespace Watermelon
{
    public class TutorialCanvasController : MonoBehaviour
    {
        private static TutorialCanvasController instance;

        public static readonly int POINTER_DEFAULT = Animator.StringToHash("Default");
        public static readonly int POINTER_TOPDOWN = Animator.StringToHash("Top Down");

        [SerializeField] CanvasGroup fadeCanvasGroup;

        [Space]
        [SerializeField] Animator pointerAnimator;

        private static Canvas tutorialCanvas;
        private static RectTransform canvasRectTransform;

        private static bool isActive;

        private static TransformCase activeTransformCase;

        private static TweenCase fadeTweenCase;

        public void Initialise()
        {
            instance = this;

            tutorialCanvas = GetComponent<Canvas>();
            tutorialCanvas.enabled = false;

            canvasRectTransform = (RectTransform)tutorialCanvas.transform;
        }

        public static void ActivatePointer(Vector3 position, int animationHash)
        {
            RectTransform pointerTransform = (RectTransform)instance.pointerAnimator.transform;
            pointerTransform.gameObject.SetActive(true);

            pointerTransform.localPosition = WorldToCanvasPosition(canvasRectTransform, Camera.main, position);
            pointerTransform.SetAsLastSibling();

            tutorialCanvas.enabled = true;

            instance.pointerAnimator.Play(animationHash, -1, 0);
        }

        private static Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
        {
            // Convert 3D world position to screen space
            Vector3 screenPosition = camera.WorldToScreenPoint(position);

            // Convert screen space position to canvas local position
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPosition, camera, out canvasPosition);

            // Set UI element's position to the calculated canvas position
            return canvasPosition;
        }

        public static void ActivateTutorialCanvas(RectTransform element, bool createDummy, bool fadeImage)
        {
            if (isActive)
                return;

            isActive = true;

            activeTransformCase = new TransformCase(element);
            activeTransformCase.SetNewParent(tutorialCanvas.transform, createDummy);

            tutorialCanvas.enabled = true;

            if (fadeImage)
            {
                instance.fadeCanvasGroup.gameObject.SetActive(true);
                instance.fadeCanvasGroup.alpha = 0;

                fadeTweenCase = instance.fadeCanvasGroup.DOFade(1.0f, 0.3f);
            }
        }

        public static void ResetTutorialCanvas()
        {
            if (!isActive)
                return;

            activeTransformCase.Reset();
            activeTransformCase = null;

            if (fadeTweenCase != null && !fadeTweenCase.IsCompleted)
                fadeTweenCase.Kill();

            instance.fadeCanvasGroup.alpha = 0;
            instance.fadeCanvasGroup.gameObject.SetActive(false);

            instance.pointerAnimator.gameObject.SetActive(false);

            tutorialCanvas.enabled = false;

            isActive = false;
        }

        public static void ResetPointer()
        {
            instance.pointerAnimator.gameObject.SetActive(false);

            tutorialCanvas.enabled = false;
        }

        public static void SetParent(Transform parentTransform)
        {
            if (instance == null) return;

            instance.transform.SetParent(parentTransform);
            instance.transform.SetAsFirstSibling();
        }

        private class TransformCase
        {
            private RectTransform rectTransform;

            private Transform parentTransform;

            private Vector2 anchoredPosition;
            private Vector2 size;
            private Vector3 scale;
            private Quaternion rotation;

            private int siblingIndex;

            private GameObject dummyObject;

            public TransformCase(RectTransform element)
            {
                rectTransform = element;

                siblingIndex = element.GetSiblingIndex();

                parentTransform = element.parent;

                anchoredPosition = element.anchoredPosition;
                size = element.sizeDelta;
                scale = element.localScale;
                rotation = element.localRotation;
            }

            public void SetNewParent(Transform transform, bool createDummy)
            {
                if (createDummy)
                {
                    dummyObject = new GameObject("[TUTORIAL DUMMY]", typeof(RectTransform));
                    dummyObject.transform.SetParent(parentTransform);
                    dummyObject.transform.SetSiblingIndex(siblingIndex);

                    RectTransform dummyRectTransform = (RectTransform)dummyObject.transform;
                    dummyRectTransform.anchoredPosition = anchoredPosition;
                    dummyRectTransform.sizeDelta = size;
                    dummyRectTransform.localScale = scale;
                    dummyRectTransform.localRotation = rotation;

                    dummyObject.SetActive(true);
                }

                rectTransform.SetParent(transform, true);
            }

            public void Reset()
            {
                if (dummyObject != null)
                    Destroy(dummyObject);

                rectTransform.SetParent(parentTransform, true);
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = size;
                rectTransform.localScale = scale;
                rectTransform.localRotation = rotation;

                rectTransform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}

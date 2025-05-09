using System.Collections;
using UnityEngine;

namespace Watermelon.BusStop
{
    public abstract class BaseCharacterBehavior : LevelElementBehavior, IClickableObject
    {
        [SerializeField] 
        protected Renderer characterRenderer;

        private Material defaultMaterial;

        private TweenCase shakeTweenCase;

        protected bool isMovementActive;
        public bool IsMovementActive => isMovementActive;

        private bool isClicked;

        private Vector3[] path;

        private void Awake()
        {
            defaultMaterial = characterRenderer.material;
        }

        public override void Initialise(LevelElement levelElement, ElementPosition elementPosition)
        {
            base.Initialise(levelElement, elementPosition);

            isClicked = false;
        }

        public override void OnElementPlacedOnMap()
        {
            isClicked = false;
        }

        public abstract void OnElementClicked(bool isClickAllowed);
        public abstract void OnElementSubmittedToBus();
        public abstract void MoveTo(Vector3[] path, bool isSlots, SimpleCallback onCompleted);

        public void OnObjectClicked()
        {
            if (isClicked) return;

            bool isClickAllowed = true;

            if (!LevelController.SubmitIsAllowed()) isClickAllowed = false;
            if (!isHighlighted) isClickAllowed = false;
            if (isMovementActive) isClickAllowed = false;

            if (isClickAllowed)
            {
                isClicked = true;

                LevelController.OnElementClicked(this, elementPosition);

                AudioController.PlaySound(AudioController.AudioClips.clickSound);
            }
            else
            {
                AudioController.PlaySound(AudioController.AudioClips.clickBlockedSound);

                shakeTweenCase = transform.DOShake(0.05f, 0.08f);
            }

            OnElementClicked(isClickAllowed);

#if MODULE_HAPTIC
            Haptic.Play(Haptic.HAPTIC_MEDIUM);
#endif
        }

        public void SetCustomMaterial(Material customMaterial)
        {
            characterRenderer.material = customMaterial;
        }

        public void ResetMaterial()
        {
            if (characterRenderer.material != defaultMaterial)
                characterRenderer.material = defaultMaterial;
        }

        public override void Unload()
        {
            shakeTweenCase.KillActive();

            isClicked = false;
        }

        public sealed override bool IsPlayableElement()
        {
            return true;
        }

        public void ChangeLastPositionInPath(Vector3 newLastPosition)
        {
            path[^1] = newLastPosition; 
        }

        protected IEnumerator MovementCoroutine(Vector3[] pathToMove, float moveSpeed, bool useRotate, SimpleCallback onCompleted)
        {
            path = pathToMove;
            int pathIndex = 0;

            isMovementActive = true;

            while (isMovementActive)
            {
                float distance = Vector3.Distance(path[pathIndex], transform.position);

                if (useRotate)
                {
                    Vector3 rotationVector = path[pathIndex] - transform.position;
                    rotationVector.y = 0;

                    if (rotationVector != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(rotationVector.normalized, Vector3.up);
                    }
                }

                transform.position = Vector3.MoveTowards(transform.position, path[pathIndex], Time.deltaTime * moveSpeed);

                if (distance <= 0.05f)
                {
                    pathIndex++;
                }

                if (pathIndex >= path.Length)
                {
                    isMovementActive = false;

                    onCompleted?.Invoke();

                    break;
                }

                yield return null;
            }
        }
    }
}

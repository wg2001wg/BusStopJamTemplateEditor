using UnityEngine;

namespace Watermelon
{
    public abstract class PUBehavior : MonoBehaviour
    {
        protected PUSettings settings;
        public PUSettings Settings => settings;

        private bool isBusy;
        public bool IsBusy 
        {
            get => isBusy;
            protected set 
            { 
                isBusy = value; 
                isDirty = true;
            }
        }

        protected bool isDirty = true;
        public bool IsDirty => isDirty;

        public void InitializeSettings(PUSettings settings)
        {
            this.settings = settings;
        }

        public abstract void Init();
        public abstract bool Activate();

        public virtual bool IsActive() => true;

        public virtual string GetFloatingMessage()
        {
            return settings.FloatingMessage;
        }

        public virtual PUTimer GetTimer()
        {
            return null;
        }

        public virtual void ResetBehavior()
        {

        }

        public void SetDirty()
        {
            isDirty = true;
        }

        public void OnRedrawn()
        {
            isDirty = false;
        }
    }
}

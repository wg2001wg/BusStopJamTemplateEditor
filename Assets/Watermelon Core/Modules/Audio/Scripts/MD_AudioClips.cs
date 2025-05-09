using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Audio Clips", menuName = "Data/Core/Audio Clips")]
    public class AudioClips : ScriptableObject
    {
        [BoxGroup("Gameplay", "Gameplay")]
        public AudioClip matchSound;
        [BoxGroup("Gameplay")]
        public AudioClip completeSound;
        [BoxGroup("Gameplay")]
        public AudioClip failSound;
        [BoxGroup("Gameplay")]
        public AudioClip clickSound;
        [BoxGroup("Gameplay")]
        public AudioClip clickBlockedSound;

        [BoxGroup("UI", "UI")]
        public AudioClip buttonSound;
    }
}

// -----------------
// Audio Controller v 0.4
// -----------------
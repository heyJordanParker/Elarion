using UnityEngine;

namespace Elarion.Audio {
    public abstract class AudioPlayer : ScriptableObject {
        public abstract void Play(AudioSource source);
    }
}
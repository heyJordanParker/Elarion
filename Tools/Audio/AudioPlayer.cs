using UnityEngine;

namespace Elarion.Tools.Audio {
    public abstract class AudioPlayer : ScriptableObject {
        public abstract void Play(AudioSource source);
    }
}
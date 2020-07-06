using Elarion.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Elarion.Tools.Audio {
    [CreateAssetMenu(menuName = "Utils/Audio Player", order = 51)]
    public class SimpleAudioPlayer : AudioPlayer {
        public AudioClip[] clips;

        [MinMaxSlider(0, 2)]
        public Vector2 volume = new Vector2(0.9f,1.1f);

        [MinMaxSlider(0, 2)]
        public Vector2 pitch = new Vector2(0.9f,1.1f);

        public override void Play(AudioSource source) {
            if(clips.Length == 0) return;

            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = Random.Range(volume.x, volume.y);
            source.pitch = Random.Range(pitch.x, pitch.y);
            source.Play();
        }
    }
}
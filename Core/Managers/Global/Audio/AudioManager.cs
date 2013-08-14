using System;
using System.Collections;
using UnityEngine;

namespace Elarion.CoreManagers {

	[Flags]
	public enum AudioEffect {
		None = 0, 
		FadeIn = 1, 
		FadeOut = 2
	}

	/// <summary>
	/// Manager class.
	/// The Audio Manager takes care of different volume adjustments and playing audio.
	/// </summary>
	public class AudioManager : ExtendedBehaviour {
		private float _masterVolume = 1;
		private float _environmentVolume = 1;
		private float _ambienceVolume = 1;
		private float _fxVolume = 1;
		private float _musicVolume = 1;

		private bool _playMusic = true;
		private bool _errorSpeech = true;

		//all volumes vary from 0 to 1
		public float MasterVolume { get { return _masterVolume; } private set { _masterVolume = value; } }
		public float EnvironmentVolume { get { return _environmentVolume; } private set { _environmentVolume = value; } }
		public float AmbienceVolume { get { return _ambienceVolume; } private set { _ambienceVolume = value; } }
		public float FXVolume { get { return _fxVolume; } private set { _fxVolume = value; } }
		public float MusicVolume { get { return _musicVolume; } private set { _musicVolume = value; } }

		public bool PlayMusic { get { return _playMusic; } private set { _playMusic = value; } }
		public bool ErrorSpeech { get { return _errorSpeech; } private set { _errorSpeech = value; } }

//		public AudioSource Play3D(AudioClip clip, Transform emitter, float volume = 1f, float pitch = 1f) {
//
//			volume *= MasterVolume;
//
//			var go = gameObject;
//			var source = go.AddComponent<AudioSource>();
//			source.clip = clip;
//			source.volume = volume;
//			source.pitch = pitch;
//			source.Play();
//			Destroy(source, clip.length);
//			return source;
//		}

//		public AudioSource Play3D(AudioClip clip, Vector3 point, float volume = 1f, float pitch = 1f) {
//			var go = new GameObject("Audio: " + clip.name);
//			go.transform.position = point;
//
//			return Play3D(clip, go.transform, volume, pitch);
//		}
//
//		public AudioSource Play(AudioClip clip, bool loop, AudioEffect effect = AudioEffect.None, float effectDuration = 1f) {
//			var source = gameObject.AddComponent<AudioSource>();
//			source.clip = clip;
//			source.dopplerLevel = 0;
//			source.maxDistance = 1000000;
//			source.loop = loop;
//			source.rolloffMode = AudioRolloffMode.Linear;
//			StartCoroutine(ApplyEffect(source, effect, loop, effectDuration));
//			return source;
//		}

		IEnumerator ApplyEffect(AudioSource source, AudioEffect effect, bool loop, float effectDuration) {
			source.Play();
			if((effect & AudioEffect.FadeIn) == AudioEffect.FadeIn) {
				source.volume = 0;
				while(source.volume < 1) {
					yield return null;
					source.volume += Time.deltaTime;
				}
				source.volume = 1;
			}

			if(loop) yield break;

			yield return new WaitForSeconds(source.clip.length - 2 * effectDuration);

			if((effect & AudioEffect.FadeOut) == AudioEffect.FadeOut) {
				while(source.volume > 0) {
					yield return null;
					source.volume -= Time.deltaTime;
				}
			}

			Destroy(source, source.clip.length);
		}

	}

}

using System.Collections;
using UnityEngine;

namespace Elarion {
	[RequireComponent(typeof(AudioSource))]
	public class PlaySoundOnEvent : MonoBehaviour {

		public string eventName;
		public float delay = 0;
		public AudioClip audioClip;
		public float volume = 1;

		private AudioSource _audioSource;

		protected void Awake() {
			if(string.IsNullOrEmpty(eventName)) return;
			_audioSource = GetComponent<AudioSource>();
			gameObject.Subscribe(eventName, "StartSound");
		}

		private void StartSound() { StartCoroutine(PlaySound()); }

		private IEnumerator PlaySound() {
			yield return new WaitForSeconds(delay);
			_audioSource.PlayOneShot(audioClip, volume);
		}
	}
}

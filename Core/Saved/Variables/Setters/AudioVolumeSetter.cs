using UnityEngine;
using UnityEngine.Audio;

namespace Elarion.Saved.Variables.Setters {
    public class AudioVolumeSetter : MonoBehaviour {
        public AudioMixer mixer;
        public string parameterName = "";
        public SavedFloat variable;

        private void Update() {
            var dB = variable.Value > 0.0f ? 20.0f * Mathf.Log10(variable.Value) : -80.0f;

            mixer.SetFloat(parameterName, dB);
        }
    }
}
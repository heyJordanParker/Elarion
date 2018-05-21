using Elarion.Saved.Variables.References;
using UnityEngine;
using UnityEngine.Audio;

namespace Elarion.Saved.Variables.Setters {
    /// <summary>
    /// Takes a FloatVariable and sends a curve filtered version of its value 
    /// to an exposed audio mixer parameter every frame on Update.
    /// </summary>
    public class AudioParameterSetter : MonoBehaviour {
        [Tooltip("Mixer to set the parameter in.")]
        public AudioMixer mixer;

        [Tooltip("Name of the parameter to set in the mixer.")]
        public string parameterName = "";

        [Tooltip("Variable to send to the mixer parameter.")]
        public SavedFloat variable;

        [Tooltip("Minimum value of the Variable that is mapped to the curve.")]
        public FloatReference min;

        [Tooltip("Maximum value of the Variable that is mapped to the curve.")]
        public FloatReference max;

        [Tooltip("Curve to evaluate in order to look up a final value to send as the parameter.\n" +
                 "T=0 is when Variable == Min\n" +
                 "T=1 is when Variable == Max")]
        public AnimationCurve curve;

        private void Update() {
            var t = Mathf.InverseLerp(min.Value, max.Value, variable.Value);
            var value = curve.Evaluate(Mathf.Clamp01(t));
            mixer.SetFloat(parameterName, value);
        }
    }
}
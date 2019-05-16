using UnityEngine;

namespace Elarion.DataBinding.Variables.Setters {
    /// <summary>
    /// Takes a FloatVariable and sends its value to an Animator parameter 
    /// every frame on Update.
    /// </summary>
    public class AnimatorParameterSetter : MonoBehaviour {
        [Tooltip("Variable to read from and send to the Animator as the specified parameter.")]
        public SavedFloat variable;

        [Tooltip("Animator to set parameters on.")]
        public Animator animator;

        [Tooltip("Name of the parameter to set with the value of Variable.")]
        public string parameterName;

        [SerializeField, HideInInspector]
        private int _parameterHash;

        private void OnValidate() {
            _parameterHash = Animator.StringToHash(parameterName);
        }

        private void Update() {
            animator.SetFloat(_parameterHash, variable.Value);
        }
    }
}
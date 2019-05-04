using Elarion.Saved.Variables.References;
using UnityEngine;

namespace Elarion.Saved.Variables.Setters {
    public abstract class TextSetterBase : ExtendedBehaviour {

        [SerializeField]
        private StringReference _string;

        protected string Text => _string.Value;

        private void OnEnable() {
            _string.Subscribe(OnStringChanged);
            UpdateText();
        }

        private void OnDisable() {
            _string.Unsubscribe(OnStringChanged);
        }
        
        protected abstract void UpdateText();

        private void OnStringChanged(string newText) {
            UpdateText();
        }
    }
}
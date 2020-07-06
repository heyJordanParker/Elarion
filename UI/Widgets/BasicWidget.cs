using System.Collections;
using Elarion.Coroutines;
using Elarion.Extensions;
using Elarion.Workflows.Events;
using TMPro;
using UnityEngine;

namespace Elarion.UI.Widgets {
    public abstract class BasicWidget<TSavedVariable, TVariableType> : MonoBehaviour where TSavedVariable : SavedEvent<TVariableType> {

        public TSavedVariable savedVariable;
        
        public TMP_Text text;
        
        [Tooltip("Base text before formatting.")]
        public string baseText = "{0} Items";

        private ECoroutine _updateCoroutine;

        protected abstract string StringifiedVariable { get; }

        protected virtual void OnEnable() {
            savedVariable.Subscribe(OnVariableChanged);
        }

        protected virtual void OnDisable() {
            savedVariable.Unsubscribe(OnVariableChanged);
        }
        
        protected IEnumerator UpdateVisibility() {
            yield return EWait.ForEndOfFrame;
            
            UpdateText();

            _updateCoroutine = null;
        }
        
        protected virtual void UpdateText() {
            text.SetText(string.Format(baseText, StringifiedVariable));
        }
        
        private void OnVariableChanged(TVariableType newValue) {
            if(_updateCoroutine == null) {
                _updateCoroutine = this.CreateCoroutine(UpdateVisibility());
            }
        }

        protected virtual void OnValidate() {
            if(!baseText.Contains("{0}")) {
                baseText += " {0}";
            }
        }

    }
}
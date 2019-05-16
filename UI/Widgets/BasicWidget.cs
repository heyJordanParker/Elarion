using System.Collections;
using Elarion.Common.Coroutines;
using Elarion.Common.Extensions;
using Elarion.DataBinding.Events;
using TMPro;
using UnityEngine;

namespace Elarion.UI.Widgets {
    public abstract class BasicWidget<TSavedVariable, TVariableType> : MonoBehaviour where TSavedVariable : SavedEvent<TVariableType> {

        public TSavedVariable savedVariable;
        
        public TMP_Text text;
        
        [Tooltip("Base text before formatting.")]
        public string baseText = "{0} Items";

        private ECoroutine _updateCoroutine;

        protected virtual string StringifiedVariable => savedVariable.name;

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
            TryStartUpdateCoroutine();
        }

        protected void TryStartUpdateCoroutine() {
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
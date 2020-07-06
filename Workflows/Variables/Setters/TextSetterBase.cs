using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Workflows.Events;
using UnityEngine;

namespace Elarion.Workflows.Variables.Setters {
    public abstract class TextSetterBase : ExtendedBehaviour {
        [SerializeField, Reorderable]
        private List<SimpleEvent> _savedValues;

        private string _initialText;
        
        protected abstract string Text { get; set; }

        private void Awake() {
            _initialText = Text;
        }
        
        private void OnEnable() {
            foreach(var savedValue in _savedValues) {
                savedValue.Subscribe(OnStringChanged);
            }
            
            UpdateText();
        }

        private void OnDisable() {
            foreach(var savedValue in _savedValues) {
                savedValue.Unsubscribe(OnStringChanged);
            }
        }

        private void OnStringChanged() {
            UpdateText();
        }
        
        protected void UpdateText() {
            Text = string.Format(_initialText, _savedValues.Select(sv => sv.ToString()).ToArray());
        }
    }
}
using System;
using Elarion.Attributes;
using Elarion.DataBinding.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Elarion.DataBinding.Variables {
    [SavedVariable]
    public abstract class SavedVariable<TVariable> : SavedEvent<TVariable> {
        
        [FormerlySerializedAs("_value")]
        [SerializeField]
        private TVariable _initialValue;

        [SerializeField]
        private TVariable _runtimeValue;
        
        [Header("Extra Parameters")]
        [SerializeField]
        private bool _readOnly;
        
        public TVariable Value {
            get => _runtimeValue;
            set {
                if(value != null && value.Equals(_runtimeValue) ||
                   value == null && _runtimeValue == null) {
                    return;
                }

                if(_readOnly) {
                    Debug.LogWarning($"Trying to change a readonly SavedVariable {name}", this);
                    return;
                }
                
                _runtimeValue = value;
                Raise(_runtimeValue);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();

            Reset();
        }

        protected override void OnDisable() {
            base.OnDisable();

            _runtimeValue = default;
        }

        public void Reset() {
            _runtimeValue = _initialValue;
        }

        /// <summary>
        /// Used by Unity Events
        /// </summary>
        public void SetValue(TVariable newValue) {
            Value = newValue;
        }
        
        public override void Subscribe(Action<TVariable> onValueChanged) {
            base.Subscribe(onValueChanged);
        }

        public override void Unsubscribe(Action<TVariable> onValueChanged) {
            base.Unsubscribe(onValueChanged);
        }

        [InspectorButton("Save RuntimeValue as InitialValue", drawInEditorMode: false)]
        protected void Save() {
            if(!Application.isPlaying) {
                Debug.LogWarning("Can't save runtime value outside of play mode.");
                return;
            }

            _initialValue = _runtimeValue;
        }
        
        public static implicit operator TVariable(SavedVariable<TVariable> savedVariable) {
            return savedVariable == null ? default(TVariable) : savedVariable.Value;
        }
    }
}
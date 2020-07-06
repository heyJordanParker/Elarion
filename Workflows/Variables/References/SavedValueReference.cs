using System;
using UnityEngine;

namespace Elarion.Workflows.Variables.References {
    // Used for the custom inspector
    [Serializable]
    public abstract class SavedValueReferenceBase { }
    
    [Serializable]
    public abstract class SavedValueReference<TSavedType, TType> : SavedValueReferenceBase where TSavedType : SavedValue<TType> {
        
        [SerializeField]
        protected bool useVariable;

        [SerializeField]
        protected TType constantValue;
        [SerializeField]
        protected TSavedType variable;
                
        private event Action<TType> Event = v => { };

        protected SavedValueReference(TType value = default) {
            constantValue = value;
        }

        public TType Value {
            get {
                if(!useVariable) {
                    return constantValue;
                }

                return variable == null ? default : variable.Value;
            }
            set {
                if(!useVariable) {
                    if(value.Equals(constantValue)) {
                        return;
                    }
                    
                    constantValue = value;
                    
                    Raise(constantValue);
                } else {
                    variable.Value = value;
                }
            }
        }

        public TType InitialValue {
            get {
                if(!useVariable) {
                    return constantValue;
                }
                
                return variable == null ? default : variable.InitialValue;
            }
        }

        protected void Raise(TType value) {
            if(useVariable) {
                // The SavedVariable will take care of the event for us
                return;
            }

            Event?.Invoke(value);
        }
        
        public virtual void Subscribe(Action<TType> onValueChanged) {
            if(useVariable) {
                // Subscribe to the saved variable instead
                variable.Subscribe(onValueChanged);
                return;
            }
            
            Event += onValueChanged;
        }

        public virtual void Unsubscribe(Action<TType> onValueChanged) {
            if(useVariable) {
                // Unsubscribe to the saved variable instead
                variable.Unsubscribe(onValueChanged);
                return;
            }
            
            Event -= onValueChanged;
        }
    }
}
using System;
using UnityEngine;

namespace Elarion.Saved.Variables.References {
    // Used for the custom inspector
    [Serializable]
    public abstract class SavedValueReferenceBase { }
    
    [Serializable]
    public abstract class SavedValueReference<TSavedType, TType> : SavedValueReferenceBase where TSavedType : SavedVariable<TType> {
        
        [SerializeField]
        protected bool useConstant;
        [SerializeField]
        protected TType constantValue;
        [SerializeField]
        protected TSavedType variable;
                
        private event Action<TType> Event = v => { };

        protected SavedValueReference(TType value = default(TType)) {
            useConstant = true;
            constantValue = value;
        }

        public TType Value {
            get {
                if(useConstant) {
                    return constantValue;
                }

                if(variable == null) {
                    return default(TType);
                }
                
                return variable.Value;
            }
            set {
                if(useConstant) {
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

        protected void Raise(TType value) {
            if(!useConstant) {
                // The SavedVariable will take care of the event for us
                return;
            }
            
            Event(value);
        }
        
        public virtual void Subscribe(Action<TType> onValueChanged) {
            if(!useConstant) {
                // Subscribe to the saved variable instead
                variable.Subscribe(onValueChanged);
                return;
            }
            
            Event += onValueChanged;
        }

        public virtual void Unsubscribe(Action<TType> onValueChanged) {
            if(!useConstant) {
                // Unsubscribe to the saved variable instead
                variable.Unsubscribe(onValueChanged);
                return;
            }
            
            Event -= onValueChanged;
        }
    }
}
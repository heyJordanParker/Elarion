using System;
using System.Collections.Generic;
using Elarion.Saved.Events;
using UnityEngine;

namespace Elarion.Saved.Variables.References {
    // Used for the custom inspector
    public abstract class SavedValueReferenceBase { }
    
    public abstract class SavedValueReference<TSavedType, TType> : SavedValueReferenceBase, IEventDispatcher<TType> where TSavedType : SavedVariable<TType> {
        
        [SerializeField]
        protected bool useConstant;
        [SerializeField]
        protected TType constantValue;
        [SerializeField]
        protected TSavedType variable;
                
        private readonly List<IEventListener<TType>> _eventListeners =
            new List<IEventListener<TType>>();

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
            
            for(int i = _eventListeners.Count - 1; i >= 0; i--) {
                _eventListeners[i].OnEventRaised(value);
            }
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

        public virtual void AddListener(IEventListener<TType> listener) {
            if(!useConstant) {
                // Listen to the saved variable instead
                variable.AddListener(listener);
                return;
            }
            
            if(!_eventListeners.Contains(listener)) {
                _eventListeners.Add(listener);
            }
        }

        public virtual void RemoveListener(IEventListener<TType> listener) {
            if(!useConstant) {
                // Stop listening to the saved variable instead
                variable.RemoveListener(listener);
                return;
            }
            
            if(_eventListeners.Contains(listener)) {
                _eventListeners.Remove(listener);
            }
        }
    }
}
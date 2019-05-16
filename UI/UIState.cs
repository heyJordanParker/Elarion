using System;
using UnityEngine;

namespace Elarion.UI {
    [DisallowMultipleComponent]
    public abstract class UIState : BaseUIBehaviour {
        [Serializable, Flags]
        protected enum States {
            None = 0 << 0, // the element is off (in the hierarchy)
            Opened = 1 << 0, // is this element considered open
            InTransition = 1 << 1, // is this element in transition (maybe rename to InAnimation)
            FocusedThis = 1 << 2, // is this element focused - usually yes, but might not be if there's an edgemenu for example
            Disabled = 1 << 3, // not interactable while visible; hook UI effects to make it sexy
            Interactable = 1 << 4, // is this element interactable (accepting input events)
        }
        
        private bool _initialized;
        
        protected bool IsStateDirty => !_initialized  || _currentState != _previousState;

        private States _previousState = States.None;
        private States _currentState = States.None;

        protected override void Start() {
            base.Start();

            _initialized = true;
        }

        /// <summary>
        /// Simply updates the state every frame. Override to change behavior.
        /// </summary>
        protected virtual void Update() {
            UpdateState();
        }

        /// <summary>
        /// Override this method to add state updates every frame. Call the base method at the end to finish the update and fire events.
        /// </summary>
        protected virtual void UpdateState() {
            if(!IsStateDirty) {
                return;
            }
            
            OnStateChanged(_currentState, _previousState);
            
            _previousState = _currentState;
        }

        protected virtual void OnStateChanged(States currentState, States previousState) { }

        private States CurrentState {
            get => _currentState;
            set => _currentState = value;
        }

        public bool IsOpened {
            get => HasFlag(CurrentState, States.Opened);
            set => CurrentState = SetFlag(CurrentState, States.Opened, value);
        }

        public bool IsInTransition {
            get => HasFlag(CurrentState, States.InTransition);
            set => CurrentState = SetFlag(CurrentState, States.InTransition, value);
        }

        public bool IsFocusedThis {
            get => HasFlag(CurrentState, States.FocusedThis);
            set => CurrentState = SetFlag(CurrentState, States.FocusedThis, value);
        }

        public bool IsDisabled {
            get => HasFlag(CurrentState, States.Disabled);
            set => CurrentState = SetFlag(CurrentState, States.Disabled, value);
        }

        public bool IsInteractable {
            get => HasFlag(CurrentState, States.Interactable);
            set => CurrentState = SetFlag(CurrentState, States.Interactable, value);
        }

        /// <summary>
        /// Non allocating version of HasFlag
        /// </summary>
        private static bool HasFlag(States state, States flag) {
            return (state & flag) == flag;
        }

        /// <summary>
        /// Non allocating version of SetFlag 
        /// </summary>
        private static States SetFlag(States state, States flag, bool value) {
            if(value) {
                // Set the flag
                state |= flag;
            } else {
                // Remove the flag
                state &= ~flag;
            }

            return state;
        }
    }
}
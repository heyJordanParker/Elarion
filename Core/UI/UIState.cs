using System;
using System.Collections.Generic;
using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI {
    [DisallowMultipleComponent]
    public class UIState : BaseUIBehaviour {
        [Serializable, Flags]
        protected enum States {
            None = 0 << 0, // the element is off (in the hierarchy)
            Opened = 1 << 0, // is this element considered open
            InTransition = 1 << 1, // is this element in transition (maybe rename to InAnimation)
            FocusedThis = 1 << 2, // is this element focused - usually yes, but might not be if there's an edgemenu for example
            Disabled = 1 << 3, // not interactable while visible; hook UI effects to make it sexy
            Interactable = 1 << 4, // is this element interactable (accepting input events)
            // hovered; for UIElements that inherit the Selectable class
            // selected; for UIElements that inherit the Selectable class
        }
        
        public event Action StateChanged = () => { };
        public event Action Opened = () => { };
        public event Action Closed = () => { };
        public event Action EnteredTransition = () => { };
        public event Action ExitedTransition = () => { };

        protected bool IsStateDirty {
            get { return currentState != previousState; }
        }

        protected States previousState = States.None;
        protected States currentState = States.None;

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
            
            // TODO integrate the focusing and unfocusing of panels with the builtin event system (ExecuteEvents and such)
            // TODO add UIElements (simpler; can't be focused)

            OnStateChanged(currentState, previousState);
            
            previousState = currentState;
        }

        protected virtual void OnStateChanged(States currentState, States previousState) {
            #region StateChangedEvents

            StateChanged();

            if(!previousState.HasFlag(States.Opened) && currentState.HasFlag(States.Opened)) {
                Opened();
            }
            
            if(previousState.HasFlag(States.Opened) && !currentState.HasFlag(States.Opened)) {
                Closed();
            }
            
            if(!previousState.HasFlag(States.InTransition) && currentState.HasFlag(States.InTransition)) {
                EnteredTransition();
            }
            
            if(previousState.HasFlag(States.InTransition) && !currentState.HasFlag(States.InTransition)) {
                ExitedTransition();
            }
            
            #endregion
        }

        private States CurrentState {
            get { return currentState; }
            set { currentState = value; }
        }

        public bool IsOpened {
            get { return CurrentState.HasFlag(States.Opened); }
            set { CurrentState = CurrentState.SetFlag(States.Opened, value); }
        }

        public bool IsInTransition {
            get { return CurrentState.HasFlag(States.InTransition); }
            set { CurrentState = CurrentState.SetFlag(States.InTransition, value); }
        }

        public bool IsFocusedThis {
            get { return CurrentState.HasFlag(States.FocusedThis); }
            set { CurrentState = CurrentState.SetFlag(States.FocusedThis, value); }
        }

        public bool IsDisabled {
            get { return CurrentState.HasFlag(States.Disabled); }
            set { CurrentState = CurrentState.SetFlag(States.Disabled, value); }
        }

        public bool IsInteractable {
            get { return CurrentState.HasFlag(States.Interactable); }
            set { CurrentState = CurrentState.SetFlag(States.Interactable, value); }
        }
    }
}
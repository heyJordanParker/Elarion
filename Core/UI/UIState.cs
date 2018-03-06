using System;
using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI {
    // TODO move the state here - use this for visualization (readonly) and to clean up the UIComponent class
    
    [DisallowMultipleComponent]
    public sealed class UIState : MonoBehaviour {
        [Serializable, Flags]
        public enum States {
            NotInitialized = -1,
            None = 0 << 0, // the element is off (in the hierarchy)
            Opened = 1 << 0, // is this element considered open
            Rendering = 1 << 1,
            InTransition = 1 << 2, // is this element in transition (maybe rename to InAnimation)
            FocusedThis = 1 << 3, // is this element focused - usually yes, but might not be if there's an edgemenu for example
            FocusedChild = 1 << 4, // a child element is currently active
            Disabled = 1 << 5, // not interactable while visible; hook UI effects to make it sexy
            RenderingChild = 1 << 6, // a child element is currently active
            Interactable = 1 << 7, // is this element interactable (accepting input events)
            // hovered; for UIElements that inherit the Selectable class
            // selected; for UIElements that inherit the Selectable class
        }
        
        public event Action StateChanged = () => { };
        public event Action Opened = () => { };
        public event Action Closed = () => { };
        public event Action StartedRendering = () => { };
        public event Action StoppedRendering = () => { };
        public event Action EnteredTransition = () => { };
        public event Action ExitedTransition = () => { };
        public event Action Focused = () => { };
        public event Action Blurred = () => { };

        private States _oldState = States.NotInitialized;
        private States _currentState = States.None;

        private void LateUpdate() {
            if(_currentState == _oldState) {
                return;
            }
            
            OnStateChanged();
        }
        
        private void OnStateChanged() {
            StateChanged();

            if(!_oldState.HasFlag(States.Opened) && _currentState.HasFlag(States.Opened)) {
                Opened();
            }
            
            if(_oldState.HasFlag(States.Opened) && !_currentState.HasFlag(States.Opened)) {
                Closed();
            }
            
            if(!_oldState.HasFlag(States.Rendering) && _currentState.HasFlag(States.Rendering)) {
                StartedRendering();
            }
            
            if(_oldState.HasFlag(States.Rendering) && !_currentState.HasFlag(States.Rendering)) {
                StoppedRendering();
            }
            
            if(!_oldState.HasFlag(States.InTransition) && _currentState.HasFlag(States.InTransition)) {
                EnteredTransition();
            }
            
            if(_oldState.HasFlag(States.InTransition) && !_currentState.HasFlag(States.InTransition)) {
                ExitedTransition();
            }
            
            if(!(_oldState.HasFlag(States.FocusedThis) || _oldState.HasFlag(States.FocusedChild)) &&
               (_currentState.HasFlag(States.FocusedThis) || _currentState.HasFlag(States.FocusedChild))) {
                Focused();
            }
            
            if((_oldState.HasFlag(States.FocusedThis) || _oldState.HasFlag(States.FocusedChild)) &&
               !(_currentState.HasFlag(States.FocusedThis) || _currentState.HasFlag(States.FocusedChild))) {
                Blurred();
            }
                
            _oldState = _currentState;

        }
        
        public States CurrentState {
            get { return _currentState; }
            set { _currentState = value; }
        }

        public bool IsOpened {
            get { return CurrentState.HasFlag(States.Opened); }
            set { CurrentState = CurrentState.SetFlag(States.Opened, value); }
        }
        
        public bool IsRendering {
            get { return CurrentState.HasFlag(States.Rendering); }
            set { CurrentState = CurrentState.SetFlag(States.Rendering, value); }
        }

        public bool IsInTransition {
            get { return CurrentState.HasFlag(States.InTransition); }
            set { CurrentState = CurrentState.SetFlag(States.InTransition, value); }
        }

        public bool IsFocusedThis {
            get { return CurrentState.HasFlag(States.FocusedThis); }
            set { CurrentState = CurrentState.SetFlag(States.FocusedThis, value); }
        }

        public bool IsFocusedChild {
            get { return CurrentState.HasFlag(States.FocusedChild); }
            set { CurrentState = CurrentState.SetFlag(States.FocusedChild, value); }
        }

        public bool IsRenderingChild {
            get { return CurrentState.HasFlag(States.RenderingChild); }
            set { CurrentState = CurrentState.SetFlag(States.RenderingChild, value); }
        }

        public bool IsDisabled {
            get { return CurrentState.HasFlag(States.Disabled); }
            set { CurrentState = CurrentState.SetFlag(States.Disabled, value); }
        }

        public bool IsInteractable {
            get { return CurrentState.HasFlag(States.Interactable); }
            set { CurrentState = CurrentState.SetFlag(States.Interactable, value); }
        }
        
        /// <summary>
        /// Helper function that returns true if any 
        /// </summary>
        public bool IsFocused {
            get { return IsFocusedThis || IsFocusedChild; }
        }
    }
}
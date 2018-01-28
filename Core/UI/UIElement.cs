using System;
using System.Linq;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animation;
using UnityEngine;

namespace Elarion.UI {
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIElement : BasicUIElement {
        
        // TODO UIForm inheritor - add error checking submitting and so on builtin
        // TODO UIDialog inheritor - custom amount of buttons, extensible, based on prefab (so the user can skin it); dialog skins?
        
        // TODO more generic UI states - focused, in transition, full screen, compound screen, disabled (elements can be visible and disabled)
        
        // TODO remove the canvas
        
        // TODO make sure the UIElement's parent UIElement is actually a parent gameobject
        
        // TODO visibility options - mobile only, landscape only, portrait only, desktop only etc
        
        // TODO child elements (get via UIManager; UIManager stores parent elements)
        
        // if it has a parent - let the user select AppearWithParent, ApearAfterParent, Manual
        // to achieve that create a hidden variable (has parent) and update it with the OnValidate method - don't include it outside the editor

        public UIEffect[] effects;

        // TODO handle ActiveChild flag via hooking to child events
        public event Action OnOpenEvent = () => { }; 
        public event Action OnCloseEvent = () => { }; 

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;
        protected UIAnimator animator;

        private UIState _oldState;
        private UIState _state;

        protected UIState State {
            get { return _state; }
            set { _state = value; }
        }
        
        public bool Active {
            get {
                return Visible || InTransition || ActiveChild;
            }
        }
        
        public bool ActiveChild {
            get { return State.HasFlag(UIState.VisibleChild); }
            set { State = State.SetFlag(UIState.VisibleChild, value); }
        }

        public bool InTransition {
            get { return State.HasFlag(UIState.InTransition); }
            set { State = State.SetFlag(UIState.InTransition, value); }
        }
        
        public bool Visible {
            get { return State.HasFlag(UIState.Visible); }
            set { State = State.SetFlag(UIState.Visible, value); }
        }
        
        public bool Fullscreen {
            get { return State.HasFlag(UIState.Fullscreen); }
            set { State = State.SetFlag(UIState.Fullscreen, value); }
        }
        
        public bool Disabled {
            get { return State.HasFlag(UIState.Disabled); }
            set { State = State.SetFlag(UIState.Disabled, value); }
        }

        public UIAnimator Animator {
            get { return animator; }
        }

        public CanvasGroup CanvasGroup {
            get { return canvasGroup; }
        }

        public float Alpha {
            get { return canvasGroup.alpha; }
            set { canvasGroup.alpha = Mathf.Clamp01(value); }
        }

        protected override void Awake() {
            base.Awake();

            _oldState = _state = UIState.NotInitialized;
            
            animator = GetComponent<UIAnimator>();

            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
            
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void Start() {
            if(UIManager == null) {
                Debug.LogWarning("Enabling a UIElement without a UIManager on the scene.", gameObject);
                return;
            }

            if(Parent != null) {
                Parent.RegisterChild(this);
                return;
            }
            
            UIManager.RegisterUIElement(this);
        }

        protected virtual void OnDestroy() {
            if(Parent != null) {
                Parent.UnregisterChild(this);
                return;
            }
            
            if(UIManager == null) {
                return;
            }
            
            UIManager.UnregisterUIElement(this);
        }

        // TODO use the OnOpenEvent event, not the method
        // TODO register the OnOpen method in the OnOpenEvent event
        public void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(Visible) {
                return;
            }
            
            OnOpen();
            
            if(animator == null || skipAnimation) {
                return;
            }
            
            animator.ResetToSavedProperties();

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation);
                return;
            }
            
            animator.Play(UIAnimationType.OnOpen);
            
            // TODO open child elements
            // turn the element off after animations, effects and child elements' effects and animations
        }

        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!Visible) {
                return;
            }
            
            OnClose();
            
            if(animator == null || skipAnimation) {
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation);
                return;
            }
            
            animator.Play(UIAnimationType.OnClose);

            // after all the effects
            // Unblur screen only after the elements have finished their animations; don't blur the screen if the elements haven't changed
        }

        protected virtual void Update() {
            // TODO use an event?
            if(animator != null) {
                InTransition = animator.Animating;
            }

            if(_oldState != _state) {
                UpdateState(); 
            }
        }

        protected virtual void OnOpen() {
            Visible = true;
        }

        protected virtual void OnClose() {
            Visible = false;
        }
        
        protected virtual void UpdateState() {
            // TODO check if any of the children elements are animating. If so - don't disable this; use a OnClose event to disable this alongside the child
            
            // TODO when I remove the canvas: disable children instead of the canvas
            // TODO when I remove the canvas: disable any graphic component on this object instead of the canvas
            // (this all is to actually hide the object) 
            canvas.enabled = Active;
            
            foreach(var effect in effects) {
                if(_oldState.HasFlag(effect.state) &&
                   !_state.HasFlag(effect.state)) {
                    
                    effect.Stop(this);
                }
                if(!_oldState.HasFlag(effect.state) &&
                   _state.HasFlag(effect.state)) {
                    
                    effect.Start(this);
                }
            }
            
            _oldState = _state;
        }

        protected internal void RegisterChild(UIElement child) {
            
        }

        protected internal void UnregisterChild(UIElement child) {
            
        } 

        // TODO cache parent in a field
        // TODO setter - sets the parent game object and refreshes the parent field
        internal UIElement Parent {
            get {
                var parentElements = GetComponentsInParent<UIElement>(includeInactive: true);

                if(parentElements == null || parentElements.Length < 2) {
                    return null;
                }

                // GetComponentsInParent operates recursively - the first member is this object, the second is the first parent with the component
                return parentElements[1];
            }
        }

        // TODO dynamically register and unregister children objects in their parents instead of the UIManager
        // TODO hook the parent object to children OnOpen/OnClose events - use them to determine when should it get disabled (after all the children have finished animating)
        internal UIElement[] Children {
            get {    
                return gameObject.GetComponentsInChildren<UIElement>(includeInactive: true).Skip(1).ToArray();
            }
        }

        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
        
    }
}
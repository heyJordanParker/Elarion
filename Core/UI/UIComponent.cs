using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO separate this into UIPanel, UIElement, and use the UIBehavior as the base class
    // TODO UIPanel should be very similar to this class
    // TODO UIElement shouldn't use a canvas and care about child elements - just an animated transform with texture 
    // TODO UIScene might not be necessary at all with this setup; just add a Fullscreen checkbox to the UIPanel

    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : MonoBehaviour {
        
        // TODO visibility options (in another component) - mobile only, landscape only, portrait only, desktop only etc
        
        // TODO hide the canvas, canvas group, and other components the user shouldn't modify (follow the UIManager's hide configuration though) 

        [SerializeField]
        protected UIEffect[] effects;
        
        [SerializeField]
        protected bool interactable = true;
        
        public event Action OnStateChanged = () => { };

        protected UIAnimator animator;
        // TODO move this to the UIPanel
        protected List<UIComponent> childElements;

        private UIState _oldState;
        private UIState _state;
        private UIComponent _parentComponent;
        private bool _isCanvasInteractable;
        
        public RectTransform Transform { get; private set; }
        
        protected UIState State {
            get { return _state; }
            set { _state = value; }
        }

        public bool ShouldRender {
            get { return Opened || InTransition || ActiveChild; }
        }
        
        public bool ActiveChild {
            get { return State.HasFlag(UIState.VisibleChild); }
            set { State = State.SetFlag(UIState.VisibleChild, value); }
        }

        public bool InTransition {
            get { return State.HasFlag(UIState.InTransition); }
            set { State = State.SetFlag(UIState.InTransition, value); }
        }
        
        public bool Opened {
            get { return State.HasFlag(UIState.Opened); }
            set { State = State.SetFlag(UIState.Opened, value); }
        }
        
        public bool Fullscreen {
            get { return State.HasFlag(UIState.Fullscreen); }
            set { State = State.SetFlag(UIState.Fullscreen, value); }
        }
        
        public bool Disabled {
            get { return State.HasFlag(UIState.Disabled); }
            set { State = State.SetFlag(UIState.Disabled, value); }
        }

        protected virtual bool Interactable {
            get {
                var interactableParent = Parent == null || Parent.Interactable;
                return interactableParent && interactable && _isCanvasInteractable;
            }
        }

        public UIAnimator Animator {
            get { return animator; }
        }
        
        public UIComponent Parent {
            get { return _parentComponent; }
        }

        public abstract float Alpha {
            get; set;
        }

        protected abstract Behaviour Render { get; }

        protected virtual void Awake() {
            Transform = GetComponent<RectTransform>();

            _oldState = _state = UIState.NotInitialized;
            
            childElements = new List<UIComponent>();
            
            animator = GetComponent<UIAnimator>();
            
            UpdateParent();
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

        public void Open(bool resetToSavedProperties = true, bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(Opened) {
                return;
            }

            Opened = true;

            foreach(var child in childElements) {
                child.Open(resetToSavedProperties, skipAnimation);
            }
            
            if(animator == null || skipAnimation) {
                return;
            }

            if(resetToSavedProperties) {
                animator.ResetToSavedProperties();
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation);
                return;
            }
            
            animator.Play(UIAnimationType.OnOpen);
        }

        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!Opened) {
                return;
            }

            Opened = false;


            foreach(var child in childElements) {
                child.Close(skipAnimation);
            }
            
            if(animator == null || skipAnimation) {
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation);
                return;
            }
            
            animator.Play(UIAnimationType.OnClose);

            // TODO figure out how to blur the screen (based on the selected game object; get inspiration from the Selectable class)
        }

        protected virtual void Update() {
            if(animator != null) {
                InTransition = animator.Animating;
            }

            Disabled = !Interactable;

            // Update the state only once per frame
            if(_oldState != _state) {
                UpdateState(); 
            }
        }

        private void UpdateState() {
            if(_state == _oldState) {
                return;
            }
            
            Render.enabled = ShouldRender;

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

            OnStateChanged();
            _oldState = _state;
        }

        private void UpdateChildState() {
            ActiveChild = childElements.SingleOrDefault(childElement => childElement.ShouldRender);
        }
        
        private void UpdateParent() {
            var parentElements = GetComponentsInParent<UIComponent>(includeInactive: true);

            if(parentElements == null || parentElements.Length < 2) {
                _parentComponent = null;
                return;
            }

            // GetComponentsInParent operates recursively - the first member is this object, the second is the first parent with the component
            _parentComponent = parentElements[1];
        }

        protected internal void RegisterChild(UIComponent child) {
            child.OnStateChanged += UpdateChildState;
            childElements.Add(child);
        }

        protected internal void UnregisterChild(UIComponent child) {
            child.OnStateChanged -= UpdateChildState;
            childElements.Remove(child);
        }
        
        // TODO optimize this; Unity calls it even if the Canvas' alpha changed - might be slow 
        protected virtual void OnCanvasGroupChanged() {
            var isCanvasIntractable = true;

            for(var t = transform; t != null; t = t.parent) {
                var canvases = t.GetComponents<CanvasGroup>();
                
                var finishedIteration = false;
                
                foreach(var canvas in canvases) {
                    if(!canvas.interactable) {
                        isCanvasIntractable = false;
                        finishedIteration = true;
                    }

                    if(canvas.ignoreParentGroups) {
                        finishedIteration = true;
                    }
                }

                if(finishedIteration)
                    break;
            }

            // TODO the enabled state is based on this and the personal state; if both are true - this object becomes enabled
            _isCanvasInteractable = isCanvasIntractable;
        }
        
        protected virtual void OnTransformParentChanged() {
            UpdateParent();
        }
        
        protected virtual void OnTransformChildrenChanged() { }
        
        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }

        protected virtual void OnValidate() {
            // TODO make sure this lives under a UIScene (what about popups?)
            
            // TODO create UI Manager if missing; Trigger it to create the UI Canvas; Make this a child to the UI Canvas if it's the topmost canvas (too rigid?)

        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {

    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : UIBehaviour {
        
        // TODO visibility options (in another component) - mobile only, landscape only, portrait only, desktop only etc
        // TODO visibility options (screen/parent size based - use the OnParentChanged thing)
        // TODO visibility options component can work with a manually-settable Hidden flag (no enabling/disabling conflicts)
        // TODO make sure the visibility options component can be attached to a UIRoot in addition to any UIComponent
        
        // TODO hide/lock the canvas, canvas group, and other components the user shouldn't modify; HideFlags/Custom Editors?
        
        public event Action OnStateChanged = () => { };

        [SerializeField]
        protected UIEffect[] effects;
        
        [SerializeField]
        protected bool interactable = true;
        
        protected UIAnimator animator;
        
        private UIState _oldState;
        private UIState _state;

        private bool _initialized;

        public RectTransform Transform { get; private set; }
        
        public UIBehaviour Parent { get; protected set; }

        protected IEnumerable<UIComponent> ChildElements {
            get {
                return UIComponentCache.Where(component => component.Parent == this);
            }
        }
        
        protected UIState State {
            get { return _state; }
            set { _state = value; }
        }

        public virtual bool ShouldRender {
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
        
        public bool Disabled {
            get { return State.HasFlag(UIState.Disabled); }
            set { State = State.SetFlag(UIState.Disabled, value); }
        }

        protected virtual bool Interactable {
            get {
                var parentComponent = Parent as UIComponent;
                var interactableParent = parentComponent == null || parentComponent.Interactable;
                return interactableParent && interactable;
            }
        }

        public UIAnimator Animator {
            get { return animator; }
        }

        public abstract float Alpha {
            get; set;
        }
        
        protected abstract Behaviour Render { get; }
        
        protected override void Awake() {
            base.Awake();
            Transform = GetComponent<RectTransform>();
            animator = GetComponent<UIAnimator>();
            
            // test
            _initialized = true;
            
            UIComponentCache.Add(this);
        }

        protected override void Start() {
            // this is to prevent OnEnable functions from calling the Open method when the scene loads 
            _initialized = true;
        }

        protected override void OnEnable() {
            base.OnEnable();
            
            UpdateParent();

            if(!gameObject.activeInHierarchy || !_initialized) {
                return;
            }
            
            Open(focus: false, skipAnimation: true);
        }

        protected override void OnDisable() {
            base.OnDisable();
            
            ActiveChild = false;
            
            if(Opened) {
                Close(skipAnimation: true);
            }
            
            UpdateState();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            UIComponentCache.Remove(this);
        }

        public void Open(bool resetToSavedProperties = true, bool focus = false, bool skipAnimation = false, UIAnimation overrideAnimation = null, bool renderOnTop = true, bool autoEnable = true) {
            if(!autoEnable && !gameObject.activeSelf) {
                // don't open disabled objects if we're not intending to enable them
                return;
            }
            
            if(Opened) {
                return;
            }
            
            Opened = true;
            
            if(!gameObject.activeSelf && autoEnable) {
                // this calls OnEnable which calls the Open method which instantly returns because Opened == true 
                gameObject.SetActive(true);
            }
            
            // TODO enable parents & open them; that'll even change the scene if an element from another scene gets opened 
            
            // TODO register opens/closes in a static stack for undo functionality

            if(renderOnTop) {
                Transform.SetAsLastSibling();
            }

            // TODO Handle focus - focus this element and unfocus every other child in the parent element (same hierarchical level)
            // Parent.Focus(this); Parent.Focus(null) should also be valid
            // TODO unfocus all other parent children (cache them in an array), focus this object (Focused = true), add all unfocused children to a OnClose callback (one-time)
            
            foreach(var child in ChildElements) {
                child.Open(resetToSavedProperties: resetToSavedProperties, skipAnimation: skipAnimation, autoEnable: false);
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


            foreach(var child in ChildElements) {
                child.Close(skipAnimation: skipAnimation);
            }
            
            if(animator == null || skipAnimation) {
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation);
                return;
            }
            
            animator.Play(UIAnimationType.OnClose);
        }

        protected virtual void Update() {
            UpdateState(); 
        }

        private void UpdateState() {
            if(animator != null) {
                InTransition = animator.Animating;
            }

            Disabled = !Interactable;
            
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
            ActiveChild = ChildElements.SingleOrDefault(childElement => childElement.ShouldRender);
        }
        
        private void UpdateParent() {
            foreach(var childElement in ChildElements) {
                childElement.OnStateChanged -= UpdateChildState;
            }
            
            var parentComponents = GetComponentsInParent<UIComponent>(includeInactive: true);

            if(parentComponents == null || parentComponents.Length < 2) {

                var parentRoot = GetComponentInParent<UIRoot>();

                if(parentRoot == null) {
                    if(!gameObject.activeInHierarchy) {
                        return;
                    }
                    
                    Debug.LogWarning("UIComponents must have a UIRoot in the parent hierarchy; Disabling.", gameObject);
                    gameObject.SetActive(false);
                    Parent = null;
                    return;
                }
                
                Parent = parentRoot;
            } else {
                // GetComponentsInParent operates recursively - the first member is this object, the second is the first parent with the component
                Parent = parentComponents[1];
            }
            
            foreach(var childElement in ChildElements) {
                childElement.OnStateChanged += UpdateChildState;
            }
        }

        protected override void OnTransformParentChanged() {
            UpdateParent();
        }
        
        protected virtual void OnTransformChildrenChanged() { }

        protected override void OnValidate() {
            base.OnValidate();
            UpdateParent();
            // TODO make sure this lives under a UIScene (what about popups?)
            
            // TODO create UI Manager if missing; Trigger it to create the UI Canvas; Make this a child to the UI Canvas if it's the topmost canvas (too rigid?)

        }
        
        private static List<UIComponent> _uiComponentCache;

        protected static List<UIComponent> UIComponentCache {
            get {
                if(_uiComponentCache == null) {
                    _uiComponentCache = new List<UIComponent>();
                }

                return _uiComponentCache;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : UIBehaviour {
        // TODO visibility options (in another component) - mobile only, landscape only, portrait only, desktop only, depending on parent State etc
        // TODO visibility options (screen/parent size based - use the OnParentChanged thing)
        // TODO visibility options component can work with a manually-settable Hidden flag (no enabling/disabling conflicts)
        // TODO visibility options require a canvas - they turn off the canvas when the condition isn't met (in this manner they can work on any object in the hierarchy and it's children)

        // TODO hide/lock the canvas, canvas group, and other components the user shouldn't modify; HideFlags/Custom Editors?

        public event Action OnStateChanged = () => { };
        
        // TODO open with parent option (currently it's always true)

        [SerializeField]
        protected UIEffect[] effects;

        // TODO dynamically update components because it's currently only in OnValidate
        // TODO interactable should be false when closed
        [SerializeField]
        private bool _interactable = true;

        protected UIAnimator animator;

        private UIState _oldState;
        private UIState _state;

        public RectTransform Transform { get; private set; }

        public UIBehaviour Parent { get; protected set; }

        protected IEnumerable<UIComponent> ChildElements {
            get { return UIComponentCache.Where(component => component.Parent == this); }
        }

        public UIState State {
            get { return _state; }
            protected set { _state = value; }
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

        public virtual bool Disabled {
            get { return State.HasFlag(UIState.Disabled); }
            set { State = State.SetFlag(UIState.Disabled, value); }
        }

        public bool Focused {
            get { return ThisFocused || FocusedChild; }
        }

        protected bool ThisFocused {
            get { return State.HasFlag(UIState.FocusedThis); }
            set { State = State.SetFlag(UIState.FocusedThis, value); }
        }

        protected bool FocusedChild {
            get { return State.HasFlag(UIState.FocusedChild); }
            set { State = State.SetFlag(UIState.FocusedChild, value); }
        }
        
        protected virtual bool SelfInteractable {
            get { return _interactable; }
            set { _interactable = value; }
        }

        protected bool Interactable {
            get {
                var parentComponent = Parent as UIComponent;
                var interactableParent = parentComponent == null || parentComponent.Interactable;
                return interactableParent && SelfInteractable;
            }
        }

        protected virtual bool OpenOnEnable {
            get { return true; }
        }

        public UIAnimator Animator {
            get { return animator; }
        }

        public abstract float Alpha { get; set; }

        protected abstract Behaviour Render { get; }

        protected override void Awake() {
            base.Awake();
            
            Transform = GetComponent<RectTransform>();
            animator = GetComponent<UIAnimator>();

            UIComponentCache.Add(this);
        }

        protected override void OnEnable() {
            base.OnEnable();

            UpdateParent();

            if(!gameObject.activeInHierarchy) {
                return;
            }

            if(!Opened) {
                Opened = OpenOnEnable;
            }
            
            UpdateState();
        }

        protected override void OnDisable() {
            base.OnDisable();
            
            ActiveChild = false;

            if(Opened) {
                Opened = false;
            }

            UpdateState();
        }
        
        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        protected virtual void OnOpen() { }

        /// <summary>
        /// Called after the object has been closed and all close animations have finished playing (if any)
        /// </summary>
        protected virtual void OnClose() { }

        protected override void OnDestroy() {
            base.OnDestroy();
            UIComponentCache.Remove(this);
        }

        public void Focus() {
            UnfocusAll();

            ThisFocused = true;

            if(EventSystem.current.currentSelectedGameObject == null) {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public void Unfocus() {
            ThisFocused = false;

            if(EventSystem.current.currentSelectedGameObject == gameObject) {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public static void UnfocusAll() {
            foreach(var component in UIComponentCache) {
                component.Unfocus();
            }
        }

        public void Open(bool resetToSavedProperties = true, bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(Opened) {
                return;
            }
            
            // returns and fires a warning if the parent object isn't open && active
            
            // TODO move this to the scene's OpenInternal
            Transform.SetAsLastSibling();

            OpenInternal(resetToSavedProperties, skipAnimation, overrideAnimation, true);
        }

        public void OpenRecursive() {
            // traverse transform hierachy and enable objects
            // open the parent (everything else that's enabled below will open automatically)
            
            // opens object and its' parents
        }

        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!Opened) {
                return;
            }

            CloseInternal(skipAnimation, overrideAnimation);
        }
        
        protected virtual void OpenInternal(bool resetToSavedProperties, bool skipAnimation, UIAnimation overrideAnimation, bool autoEnable) {
            if(Opened) {
                Debug.LogWarning("Opening a UIComponent that's already open.", gameObject);
            }

            if(!gameObject.activeSelf && !autoEnable) {
                Debug.LogWarning("Opening a UIComponent that's disabled.", gameObject);
            }
            
            if(!gameObject.activeSelf && autoEnable) {
                gameObject.SetActive(true);
            }
            
            Opened = true;
            
            // TODO register opens/closes in a static stack for undo functionality

            foreach(var child in ChildElements) {
                if(!child.gameObject.activeSelf) {
                    continue;
                }

                if(child.Opened) {
                    continue;
                }
                child.OpenInternal(resetToSavedProperties, skipAnimation, overrideAnimation, false);
            }

            if(animator == null || skipAnimation) {
                OnOpen();
                return;
            }

            if(resetToSavedProperties) {
                animator.ResetToSavedProperties();
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, callback: OnOpen);
            } else {
                animator.Play(UIAnimationType.OnOpen, callback: OnOpen);
            }
        }

        protected virtual void CloseInternal(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!Opened) {
                Debug.LogWarning("Closing a UIComponent that's already closed.", gameObject);
            }
            
            Opened = false;
            ThisFocused = false;

            foreach(var child in ChildElements) {
                child.Close(skipAnimation: skipAnimation);
            }

            if(animator == null || skipAnimation) {
                OnClose();
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, callback: OnClose);
            } else {
                animator.Play(UIAnimationType.OnClose, callback: OnClose);
            }
        }

        protected virtual void Update() {
            UpdateState();
        }

        private void UpdateState() {
            if(animator != null) {
                InTransition = animator.Animating;
            }

            Disabled = !Interactable || !ShouldRender;

            if(_state == _oldState) {
                return;
            }
            
            Render.enabled = ShouldRender;

            foreach(var effect in effects) {

                if(effect.ShouldBeActive(this) && !effect.Active) {
                    effect.Start(this);
                } else if(!effect.ShouldBeActive(this) && effect.Active) {
                    effect.Stop(this);
                }
            }

            OnStateChanged();
            _oldState = _state;
        }

        private void UpdateChildState() {
            ActiveChild = ChildElements.SingleOrDefault(childElement => childElement.ShouldRender);
            FocusedChild = ChildElements.SingleOrDefault(childElement => childElement.Focused);
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
            
            UpdateChildState();
        }

        protected override void OnTransformParentChanged() {
            UpdateParent();
        }

        protected virtual void OnTransformChildrenChanged() { }

        protected override void OnValidate() {
            base.OnValidate();

//            #if UNITY_EDITOR
//            if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
                SelfInteractable = _interactable;
//            }
//            #endif
            
            UpdateParent();
            // TODO make sure this lives under a UIScene (what about popups?)

            // TODO create UI Manager if missing; Trigger it to create the UI Canvas; Make this a child to the UI Canvas if it's the topmost canvas (too rigid?)

        }

        private static List<UIComponent> _uiComponentCache;
        internal static List<UIComponent> UIComponentCache {
            get {
                if(_uiComponentCache == null) {
                    _uiComponentCache = new List<UIComponent>();
                }

                return _uiComponentCache;
            }
        }
    }
}
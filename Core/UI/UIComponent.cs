using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.Extensions;
using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class UIComponent : UIBehaviour {
        // TODO visibility options (in another component) - mobile only, landscape only, portrait only, desktop only, depending on parent State etc
        // TODO visibility options (screen/parent size based - use the OnParentChanged thing)
        // TODO visibility options component can work with a manually-settable Hidden flag (no enabling/disabling conflicts)
        // TODO visibility options require a canvas - they turn off the canvas when the condition isn't met (in this manner they can work on any object in the hierarchy and it's children)

        public event Action<UIState,UIState> OnStateChanged = (currentState, oldState) => { };
        
        [SerializeField]
        protected UIEffect[] effects;

        // TODO dynamically update components because it's currently only in OnValidate
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
            get {
                if(Opened || InTransition || ActiveChild) {
                    return true;
                }
                
                // TODO find a more elegant solution
                // Hack: render if both this and the parent are closing, but this doesn't have a close animation (otherwise this just disappears while the parent is animating)
                var parentComponent = Parent as UIComponent;

                if(parentComponent == null || parentComponent.Opened) {
                    return false;
                }

                if(animator != null && animator.HasAnimation(UIAnimationType.OnClose)) {
                    return false;
                }
                
                return parentComponent.ShouldRender;
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

        public bool Opened {
            get { return State.HasFlag(UIState.Opened); }
            set { State = State.SetFlag(UIState.Opened, value); }
        }

        public virtual bool Disabled {
            get { return State.HasFlag(UIState.Disabled); }
            set { State = State.SetFlag(UIState.Disabled, value); }
        }

        public bool Focused {
            get { return FocusedThis || FocusedChild; }
        }

        protected bool FocusedThis {
            get { return State.HasFlag(UIState.FocusedThis); }
            set {
                if(FocusedThis == value) {
                    return;
                }
                
                State = State.SetFlag(UIState.FocusedThis, value);

                if(UIRoot) {
                    UIRoot.FocusComponent(this, value);
                }
            }
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
        }

        protected override void OnDisable() {
            base.OnDisable();

            if(!Opened) return;
            
            Close(true);
            UpdateState();
        }
        
        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterOpen() { }

        /// <summary>
        /// Called after the object has been closed and all close animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterClose() { }

        protected override void OnDestroy() {
            base.OnDestroy();
            UIComponentCache.Remove(this);
        }

        public virtual void Focus() {
            UnfocusAll();

            FocusedThis = true;
        }

        public virtual void Unfocus() {
            FocusedThis = false;
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

            var parentComponent = Parent as UIComponent;

            if(!Parent.isActiveAndEnabled ||
               parentComponent != null && !parentComponent.Opened) {
                    return;
            }

            OpenInternal(resetToSavedProperties, skipAnimation, overrideAnimation, true);
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
                AfterOpen();
                return;
            }

            if(resetToSavedProperties) {
                animator.ResetToSavedProperties();
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, callback: AfterOpen);
            } else {
                animator.Play(UIAnimationType.OnOpen, callback: AfterOpen);
            }
        }

        protected virtual void CloseInternal(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool closeChildren = true) {
            
            if(!Opened) {
                Debug.LogWarning("Closing a UIComponent that's already closed.", gameObject);
            }
            
            Opened = false;
            FocusedThis = false;

            if(closeChildren) {
                foreach(var child in ChildElements) {
                    child.Close(skipAnimation: skipAnimation);
                }
            }

            if(animator == null || skipAnimation) {
                AfterClose();
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, callback: AfterClose);
            } else {
                animator.Play(UIAnimationType.OnClose, callback: AfterClose);
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
            
            // TODO play focus/unfocus animations

            OnStateChanged(_state, _oldState);
            _oldState = _state;
        }

        private void UpdateChildState(UIState currentState, UIState oldState) {
            ActiveChild = ChildElements.SingleOrDefault(childElement => childElement.ShouldRender && childElement.gameObject.activeInHierarchy);
            FocusedChild = ChildElements.SingleOrDefault(childElement => childElement.Focused && childElement.gameObject.activeInHierarchy);
        }

        private void UpdateParent() {
            foreach(var childElement in ChildElements) {
                childElement.OnStateChanged -= UpdateChildState;
            }

            var parentComponents = GetComponentsInParent<UIComponent>(includeInactive: true);

            if(parentComponents == null || parentComponents.Length < 2) {

                if(UIRoot == null) {
                    if(!gameObject.activeInHierarchy) {
                        return;
                    }

                    Debug.LogWarning("UIComponents must have a UIRoot in the parent hierarchy; Disabling.", gameObject);
                    gameObject.SetActive(false);
                    Parent = null;
                    return;
                }

                Parent = UIRoot;
            } else {
                // GetComponentsInParent operates recursively - the first member is this object, the second is the first parent with the component
                Parent = parentComponents[1];
            }
            
            foreach(var childElement in ChildElements) {
                childElement.OnStateChanged += UpdateChildState;
            }
            
            UpdateChildState(State, _oldState);
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
        }
        
        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<b>Opened: </b>" + Opened);
            stringBuilder.AppendLine("<b>Rendering: </b>" + ShouldRender);
            stringBuilder.AppendLine("<b>In Transition: </b>" + InTransition);
            stringBuilder.AppendLine("<b>Focused: </b>" + FocusedThis);
            stringBuilder.AppendLine("<b>Disabled: </b>" + Disabled);
            stringBuilder.AppendLine("<b>Focused Child: </b>" + FocusedChild);
            stringBuilder.AppendLine("<b>Visible Child: </b>" + ActiveChild);
            return stringBuilder.ToString();
        }

        protected UIRoot UIRoot {
            get {
                var uiRoot = UIRoot.UIRootCache.SingleOrDefault(root => root.transform.IsParentOf(transform));

                if(uiRoot == null) {
                    uiRoot = GetComponentInParent<UIRoot>();
                }

                return uiRoot;
            }
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
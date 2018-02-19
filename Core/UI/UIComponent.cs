using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.Extensions;
using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class UIComponent : UIBehaviour, ISubmitHandler, ICancelHandler {
        // TODO visibility options (in another component) - mobile only, landscape only, portrait only, desktop only, depending on parent State etc
        // TODO visibility options (screen/parent size based - use the OnParentChanged thing)
        // TODO visibility options component can work with a manually-settable Hidden flag (no enabling/disabling conflicts)
        // TODO visibility options require a canvas - they turn off the canvas when the condition isn't met (in this manner they can work on any object in the hierarchy and it's children)

        public event Action<UIState,UIState> OnStateChanged = (currentState, oldState) => { };
        
        [SerializeField]
        protected UIEffect[] effects;

        [SerializeField]
        private UIOpenType _openType = UIOpenType.OpenWithParent;

        protected UIAnimator animator;

        private UIRoot _uiRoot;
        
        private UIState _oldState = UIState.NotInitialized;
        private UIState _state = UIState.None;

        public RectTransform Transform { get; private set; }

        public UIBehaviour Parent { get; protected set; }

        protected IEnumerable<UIComponent> ChildElements {
            get { return UIComponentCache.Where(component => component.Parent == this); }
        }

        public UIState State {
            get { return _state; }
            protected set { _state = value; }
        }

        protected virtual UIOpenType OpenType {
            get { return _openType; }
        }

        public virtual GameObject FirstFocused { get; set; }

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

                if(HasAnimator && animator.HasAnimation(UIAnimationType.OnClose)) {
                    return false;
                }
                
                return parentComponent.ShouldRender;
            }
        }
        
        public bool ActiveChild {
            get { return State.HasFlag(UIState.VisibleChild); }
            set { State = State.SetFlag(UIState.VisibleChild, value); }
        }

        // should this be in transition while the parent is in transition?
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

        public bool Interactable {
            get { return State.HasFlag(UIState.Interactable); }
            protected set { State = State.SetFlag(UIState.Interactable, value); }
        }

        protected virtual bool InteractableSelf {
            get { return true; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        protected bool InteractableParent {
            get {
                var parentComponent = Parent as UIComponent;
                return parentComponent == null || parentComponent.Interactable;
            }
        }

        // TODO HasComponent function in utils
        protected bool HasAnimator {
            get { return animator != null && animator.enabled; }
        }

        public bool IsAnimating {
            get { return HasAnimator && animator.Animating; }
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
            
            UpdateState();
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
        protected virtual void AfterOpen() {
            OpenChildren(UIOpenType.OpenAfterParent, false);
        }

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
        
        // TODO consider using a UnityEvent for handling those 
        public void OnSubmit(BaseEventData eventData) {
            if(!Interactable) {
                return;
            }

            OnSubmitInternal(eventData);
            
            Debug.Log("Submitted " + name, gameObject);
        }
        
        public void OnCancel(BaseEventData eventData) {
            if(!Interactable) {
                return;
            }
            
            OnCancelInternal(eventData);
            Debug.Log("Cancelled " + name, gameObject);
        }

        protected virtual void OnSubmitInternal(BaseEventData eventData) { }
        protected virtual void OnCancelInternal(BaseEventData eventData) { }

        public void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool focus = true) {
            if(Opened) {
                return;
            }
            
            var parentComponent = Parent as UIComponent;

            if(!Parent.isActiveAndEnabled ||
               parentComponent != null && !parentComponent.Opened) {
                    return;
            }
            
            if(!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }

            OpenInternal(skipAnimation, overrideAnimation);

            if(focus) {
                FocusThis();
            }
        }
        
        protected virtual void OpenInternal(bool skipAnimation, UIAnimation overrideAnimation) {
            if(Opened) {
                Debug.LogException(new Exception("Opening a UIComponent that's already open."), gameObject);
            }
            
            Opened = true;
            
            // TODO register opens/closes in a static stack for undo functionality

            OpenChildren(UIOpenType.OpenWithParent, skipAnimation);

            if(skipAnimation) {
                OpenChildren(UIOpenType.OpenAfterParent, true);
            }

            if(!HasAnimator || skipAnimation) {
                AfterOpen();
                return;
            }
            
            animator.ResetToSavedProperties();

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, callback: AfterOpen);
            } else {
                animator.Play(UIAnimationType.OnOpen, callback: AfterOpen);
            }
        }

        protected virtual void OpenChildren(UIOpenType openTypeFilter, bool skipAnimation) {
            foreach(var child in ChildElements) {
                if(!child.gameObject.activeSelf ||
                   child.Opened ||
                   child.OpenType != openTypeFilter) {
                    continue;
                }
                
                child.OpenInternal(skipAnimation, null);
            }
        }
        
        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!Opened) {
                return;
            }

            CloseInternal(skipAnimation, overrideAnimation);
        }

        protected virtual void CloseInternal(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool closeChildren = true) {
            if(!Opened) {
                Debug.LogWarning("Closing a UIComponent that's already closed.", gameObject);
            }
            
            Opened = false;

            if(FocusedThis) {
                FocusParent();
            }

            if(closeChildren) {
                foreach(var child in ChildElements) {
                    child.Close(skipAnimation: skipAnimation);
                }
            }

            if(!HasAnimator || skipAnimation) {
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

        /// <summary>
        /// Update State Method. Override to add custom state behavior.
        /// </summary>
        /// <returns>Returns false if the change wasn't updated (likely because it didn't change).</returns>
        protected virtual bool UpdateState() {
            if(HasAnimator) {
                InTransition = IsAnimating;
            }

            Interactable = Opened && !Disabled && !InTransition && InteractableSelf && InteractableParent;

            if(_state == _oldState) {
                return false;
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

            return true;
        }

        private void UpdateChildState(UIState currentState, UIState oldState) {
            ActiveChild = false;
            FocusedChild = false;
            
            foreach(var child in ChildElements) {
                if(!child.isActiveAndEnabled) {
                    continue;
                }

                if(!ActiveChild && child.ShouldRender) {
                    ActiveChild = true;
                }

                if(!FocusedChild && child.Focused) {
                    FocusedChild = true;
                }

                if(ActiveChild && FocusedChild) {
                    // no further state changes can happen after both flags are true
                    break;
                }
            }
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

        protected virtual void FocusThis() {
            // Get the bottom focused component
            if(FirstFocused == null) {
                return;
            }

            if(!Opened) {
                var parentComponent = Parent as UIComponent;
                if(parentComponent) {
                    parentComponent.FocusThis();
                    return;
                }
                
                UIRoot.CurrentScene.FocusThis();
                return;
            }

            var focusedComponent = this;

            // Traverse hierarchy down
            while(true) {
                if(focusedComponent.FirstFocused == null) {
                    break;
                }
                
                var nextComponent = focusedComponent.FirstFocused.GetComponent<UIComponent>();

                if(nextComponent == null || nextComponent == focusedComponent || !nextComponent.Opened) {
                    break;
                }

                focusedComponent = nextComponent;
            }
            
            focusedComponent.Focus();

            var firstFocused = focusedComponent.FirstFocused;

            var firstFocusedComponent = firstFocused.GetComponent<UIComponent>();

            // don't focus a child component that's not opened
            if(firstFocusedComponent != null && !firstFocusedComponent.Opened) {
                firstFocused = ChildElements.Where(child => child.Opened).Select(child => child.gameObject)
                    .FirstOrDefault();
            }
            
            if(firstFocused == null) {
                firstFocused = focusedComponent.gameObject;
            }

            var selectable = firstFocused.GetFirstSelectableChild();

            if(selectable) {
                UIRoot.Focus(selectable);
            }
        }

        private void FocusParent() {
            var parent = Parent as UIComponent;

            if(parent == null || !parent.Opened) {
                parent = UIRoot.CurrentScene;
            }

            if(parent == null || !parent.Opened) {
                return;
            }
            
            parent.Focus();
            parent.FocusThis();
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();
            UpdateParent();
        }

        protected virtual void OnTransformChildrenChanged() { }

        protected override void OnValidate() {
            base.OnValidate();
            
            UpdateParent();
            // TODO make sure this lives under a UIScene (what about popups?)
        }
        
        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<b>" + GetType().Name + ": </b>" + name);
            stringBuilder.AppendLine("<b>Opened: </b>" + Opened);
            stringBuilder.AppendLine("<b>Rendering: </b>" + ShouldRender);
            stringBuilder.AppendLine("<b>In Transition: </b>" + InTransition);
            stringBuilder.AppendLine("<b>Focused: </b>" + FocusedThis);
            stringBuilder.AppendLine("<b>Disabled: </b>" + Disabled);
            stringBuilder.AppendLine("<b>Interactable: </b>" + Interactable);
            stringBuilder.AppendLine("<b>Focused Child: </b>" + FocusedChild);
            stringBuilder.AppendLine("<b>Visible Child: </b>" + ActiveChild);
            return stringBuilder.ToString();
        }

        public UIRoot UIRoot {
            get {
                if(_uiRoot == null) {
                    _uiRoot = UIRoot.UIRootCache.SingleOrDefault(root => root.transform.IsParentOf(transform));

                    if(_uiRoot == null) {
                        _uiRoot = GetComponentInParent<UIRoot>();
                    }
                }

                return _uiRoot;
            }
        }

        // TODO implement a hierarchy interface containing transform, parent, and children in UIComponent and UIRoot
        // TODO create a smarter & faster search using the fields from the hierarchy interface
        private static List<UIComponent> _uiComponentCache;

        internal static List<UIComponent> UIComponentCache {
            get {
                if(_uiComponentCache == null) {
                    _uiComponentCache = new List<UIComponent>();
                }

                return _uiComponentCache;
            }
        }

        public static UIComponent GetUIComponentParent(Transform t) {
            var target = t;

            while(target != null) {
                var component = UIComponentCache.SingleOrDefault(c => c.gameObject == target.gameObject);
                if(component != null) {
                    return component;
                }

                target = target.transform.parent;
            }
            
            return null;
        } 
    }
}
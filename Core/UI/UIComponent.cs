using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class UIComponent : BaseUIBehaviour, IPointerClickHandler {
        
        public event Action<UIState,UIState> OnStateChanged = (currentState, oldState) => { };
        
        [SerializeField]
        private UIOpenType _openType = UIOpenType.OpenWithParent;
        
        [SerializeField]
        protected bool focusable = true;
        
        [SerializeField]
        [ConditionalVisibility("focusable")]
        private GameObject _firstFocused;

        private UIRoot _uiRoot;
        
        private UIState _oldState = UIState.NotInitialized;
        private UIState _state = UIState.None;

        private UIComponent _initialFocusedComponent;
        private Selectable _initialFocusedSelectable;

        private UIAnimator _animator;
        
        public BaseUIBehaviour Parent { get; protected set; }

        protected List<UIComponent> ChildComponents { get; set; }

        public UIAnimator Animator {
            get {
                if(_animator == null) {
                    _animator = GetComponent<UIAnimator>();
                }

                return _animator;
            }
        }
        
        public UIOpenConditions OpenConditions { get; protected set; }

        public UIState State {
            get { return _state; }
            protected set { _state = value; }
        }

        public UIOpenType OpenType {
            get { return _openType; }
        }

        public UIComponent InitialFocusedComponent {
            get {
                if(!Focusable) {
                    return null;
                }
                
                // recursively we get to the bottom component
                if(_initialFocusedComponent &&
                   _initialFocusedComponent._initialFocusedComponent &&
                   _initialFocusedComponent._initialFocusedComponent != this) {
                    return _initialFocusedComponent.InitialFocusedComponent;
                }

                return _initialFocusedComponent ? _initialFocusedComponent : this;

            }
        }

        public Selectable InitialFocusedSelectable {
            get { return _initialFocusedSelectable; }
        }
        
        public virtual bool ShouldRender {
            get {
                if(Opened || InTransition || ActiveChild) {
                    return true;
                }
                
                // TODO find an elegant solution
                // Hack: render if both this and the parent are closing, but this doesn't have a close animation (otherwise this just disappears while the parent is animating)
                var parentComponent = Parent as UIComponent;

                if(parentComponent == null || parentComponent.Opened) {
                    return false;
                }

                if(HasAnimator && Animator.HasAnimation(UIAnimationType.OnClose)) {
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
            set { State = State.SetFlag(UIState.FocusedThis, value); }
        }

        protected bool FocusedChild {
            get { return State.HasFlag(UIState.FocusedChild); }
            set { State = State.SetFlag(UIState.FocusedChild, value); }
        }

        public virtual bool Focusable {
            get { return focusable && Opened && !Disabled; }
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

        public bool HasAnimator {
            get {
                return Animator != null && Animator.enabled;
            }
        }

        public bool IsAnimating {
            get { return _animationControllers.Any(ac => ac.Animating); }
        }

        public abstract float Alpha { get; set; }

        protected abstract Behaviour Render { get; }

        private IAnimationController[] _animationControllers;

        protected override void Awake() {
            base.Awake();
            _animator = GetComponent<UIAnimator>();
            _animationControllers = GetComponents<IAnimationController>();
            OpenConditions = GetComponent<UIOpenConditions>();

            if(!_firstFocused) {
                return;
            }

            _initialFocusedComponent = _firstFocused.GetComponent<UIComponent>();
            _initialFocusedSelectable = _firstFocused.GetComponent<Selectable>();
        }

        protected override void OnEnable() {
            base.OnEnable();

            UpdateParent();
            UpdateChildren();
            UpdateState();
        }

        protected override void OnDisable() {
            base.OnDisable();

            if(!Opened) return;
            
            Close(true);
            
            UpdateChildren();
            UpdateState();
        }
        
        public virtual void OnPointerClick(PointerEventData eventData) {
            UIRoot.Select(null);
            Focus();
        }

        public virtual void Focus(bool setSelection = false) {
            if(FocusedThis || !UIRoot || !Focusable) {
                return;
            }

            if(FocusedComponent) {
                FocusedComponent.Unfocus();
            }
            
            var focusedComponent = FindNextFocusedComponent();

            FocusedComponent = focusedComponent;
            
            if(!setSelection) {
                return;
            }
            
            var selectable = InitialFocusedSelectable;

            if(!selectable) {
                selectable = FocusedComponent.gameObject.GetFirstSelectableChild();
            }

            if(selectable && UIRoot) {
                UIRoot.Select(selectable);
            }
        }
        
        private UIComponent FindNextFocusedComponent() {
            if(Focusable) {
                // recursively find a focusable child
                return InitialFocusedComponent;
            }
            
            // find a focusable parent
            var parentComponent = Parent as UIComponent;
            if(parentComponent) {
                return parentComponent.FindNextFocusedComponent();
            }

            if(UIRoot.CurrentScene == this) {
                return null;
            }
                
            return UIRoot.CurrentScene.FindNextFocusedComponent();

        }

        public virtual void Unfocus() {
            if(!FocusedThis) {
                return;
            }

            FocusedComponent = null;
        }

        /// <summary>
        /// Called after the object has been closed and all close animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterClose() {
            if(HasAnimator) {
                Animator.ResetToSavedProperties();
                UpdateState(); // to instantly hide it; otherwise it'll stay visible for a whole frame with its' properties reset
            }
        }

        // TODO leave just one inheritable open/close/submit/cancel method
        // TODO create PreOpen (and rename AfterOpen to PostOpen) methods to handle other cases (like setting this to the last child before opening [UIScene])
        public virtual void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool focus = true, bool enable = true) {
            
            if(enable && !gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
            
            if(!CanOpen) {
                return;
            }

            PreOpen();
            
            OpenInternal(skipAnimation, overrideAnimation);

            if(focus) {
                Focus(true);
            }
        }
        
        public virtual bool CanOpen {
            get {
                if(Opened || !isActiveAndEnabled || !Parent.isActiveAndEnabled) {
                    return false;
                }
            
                var parentComponent = Parent as UIComponent;

                if(parentComponent != null && !parentComponent.Opened) {
                    return false;
                }
            
                if(OpenConditions && !OpenConditions.CanOpen) {
                    return false;
                }

                return true;
            }
        }

        protected virtual void PreOpen() {
            
        }
        
        protected virtual void OpenInternal(bool skipAnimation, UIAnimation overrideAnimation) {
            
            Opened = true;
            
            OpenChildren(UIOpenType.OpenWithParent, skipAnimation);

            if(skipAnimation) {
                OpenChildren(UIOpenType.OpenAfterParent, true);
            }

            if(!HasAnimator || skipAnimation) {
                AfterOpen();
                return;
            }
            
            Animator.ResetToSavedProperties();

            var animation = overrideAnimation;

            if(animation == null) {
                animation = Animator.GetAnimation(UIAnimationType.OnOpen);
            }
            
            Animator.Play(animation, callback: AfterOpen);

        }
        
        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterOpen() {
            OpenChildren(UIOpenType.OpenAfterParent, false);
        }

        protected virtual void OpenChildren(UIOpenType openTypeFilter, bool skipAnimation) {
            foreach(var child in ChildComponents) {
                if(!child.gameObject.activeSelf ||
                   child.Opened ||
                   child.OpenType != openTypeFilter) {
                    continue;
                }
                
                child.Open(skipAnimation, enable: false, focus: false);
            }
        }
        
        public virtual bool CanClose {
            get { return Opened; }
        }
        
        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!CanClose) {
                return;
            }

            CloseInternal(skipAnimation, overrideAnimation);
        }

        protected virtual void CloseInternal(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool closeChildren = true) {
            
            Opened = false;

            if(FocusedThis) {
                var nextFocused = FindNextFocusedComponent();
                if(nextFocused) {
                    nextFocused.Focus(true);
                }
            }

            if(closeChildren) {
                foreach(var child in ChildComponents) {
                    child.Close(skipAnimation);
                }
            }

            if(!HasAnimator || skipAnimation) {
                AfterClose();
                return;
            }

            var animation = overrideAnimation;

            if(animation == null) {
                animation = Animator.GetAnimation(UIAnimationType.OnClose);
            }
            
            Animator.Play(animation, callback: AfterClose);
        }

        protected virtual void Update() {
            UpdateState();
        }

        /// <summary>
        /// Update State Method. Override to add custom state behavior.
        /// </summary>
        /// <returns>Returns false if the change wasn't updated (likely because it didn't change).</returns>
        protected virtual bool UpdateState() {
            InTransition = IsAnimating;
            
            FocusedThis = this == FocusedComponent;

            Interactable = Opened && !Disabled && !InTransition && InteractableSelf && InteractableParent;
            
            Render.enabled = ShouldRender;

            if(_state == _oldState) {
                return false; // state hasn't updated
            }
            
            OnStateChanged(_state, _oldState);
            _oldState = _state;

            return true; // state updated
        }

        private void UpdateChildState(UIState currentState, UIState oldState) {
            ActiveChild = false;
            FocusedChild = false;
            
            foreach(var child in ChildComponents) {
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
            var parentComponents = GetComponentsInParent<UIComponent>(includeInactive: true);

            if(parentComponents == null || parentComponents.Length < 2) {

                if(UIRoot == null) {
                    if(!isActiveAndEnabled) {
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
        }

        private void UpdateChildren() {
            if(ChildComponents == null) {
                ChildComponents = new List<UIComponent>();
            } else {
                foreach(var childComponent in ChildComponents) {
                    childComponent.OnStateChanged -= UpdateChildState;
                }
            }

            if(!gameObject.activeSelf) {
                // unhook from child events and return if the game object is disabled
                return;
            }

            // Skip the first UIComponent - it's always the one attached to this game object
            ChildComponents = GetComponentsInChildren<UIComponent>(includeInactive: true).Skip(1).ToList();
            
            foreach(var childComponent in ChildComponents) {
                childComponent.OnStateChanged += UpdateChildState;
            }
            
            UpdateChildState(State, _oldState);
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();
            UpdateParent();
        }

        protected virtual void OnTransformChildrenChanged() {
            UpdateChildren();
        }

        protected override void OnValidate() {
            base.OnValidate();
            
            UpdateParent();

            // update first focused
            if(!focusable) {
                return;
            }

            if(_firstFocused != null) {
                if(!_firstFocused.transform.IsChildOf(transform)) {
                    _firstFocused = null;
                } else {
                    return;
                }
            }

            _firstFocused = gameObject.GetComponentsInChildren<UIComponent>().Where(c => c.Focusable && c != this).Select(c => c.gameObject).FirstOrDefault();
            
            if(_firstFocused != null) {
                return;
            }
            
            var selectable = GetComponentInChildren<Selectable>();
            if(selectable != null) {
                _firstFocused = selectable.gameObject;
            }
        }

        public string Description {
            get {
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

        public static UIComponent FocusedComponent { get; set; }
    }
}
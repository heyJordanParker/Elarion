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
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIState))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : BaseUIBehaviour, IPointerClickHandler {
        // TODO use a null Parent for the topmost component
        [SerializeField]
        private UIOpenType _openType = UIOpenType.OpenWithParent;

        [SerializeField]
        protected bool focusable = true;

        [SerializeField]
        [ConditionalVisibility("focusable")]
        private GameObject _firstFocused;

        private UIRoot _uiRoot;

        private UIState _state;

        private UIComponent _initialFocusedComponent;
        private Selectable _initialFocusedSelectable;

        private UIAnimator _animator;
        private IAnimationController[] _animationControllers;

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
            get {
                if(!_state) {
                    _state = gameObject.Component<UIState>();
                }

                return _state;
            }
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
                if(State.IsOpened || State.IsInTransition || State.IsRenderingChild) {
                    return true;
                }

                // TODO more elegant solution described in the OpenInternal method
                // Hack: render if both this and the parent are closing, but this doesn't have a close animation (otherwise this just disappears while the parent is animating)
                var parentComponent = Parent as UIComponent;

                if(parentComponent == null || parentComponent.State.IsOpened) {
                    return false;
                }

                if(HasAnimator && Animator.HasAnimation(UIAnimationType.OnClose)) {
                    return false;
                }

                return parentComponent.ShouldRender;
            }
        }

        public virtual bool Focusable {
            get { return focusable && State.IsOpened && !State.IsDisabled; }
        }

        protected virtual bool InteractableSelf {
            get { return true; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        protected bool InteractableParent {
            get {
                var parentComponent = Parent as UIComponent;
                return parentComponent == null || parentComponent.State.IsInteractable;
            }
        }

        public bool HasAnimator {
            get { return Animator != null && Animator.enabled; }
        }

        public bool IsAnimating {
            get { return _animationControllers.Any(ac => ac.Animating); }
        }

        public abstract float Alpha { get; set; }

        public abstract Behaviour Renderer { get; }

        protected virtual bool CanOpen {
            get {
                if(State.IsOpened || !isActiveAndEnabled || !Parent.isActiveAndEnabled) {
                    return false;
                }

                var parentComponent = Parent as UIComponent;

                if(parentComponent != null && !parentComponent.State.IsOpened) {
                    return false;
                }

                if(OpenConditions && !OpenConditions.CanOpen) {
                    return false;
                }

                return true;
            }
        }

        protected override void Awake() {
            base.Awake();
            _state = GetComponent<UIState>();
            _animator = GetComponent<UIAnimator>();
            _animationControllers = GetComponents<IAnimationController>();
            
            State.StateChanged += OnStateChanged;

            OpenConditions = GetComponent<UIOpenConditions>();

            if(!_firstFocused) {
                return;
            }

            _initialFocusedComponent = _firstFocused.GetComponent<UIComponent>();
            _initialFocusedSelectable = _firstFocused.GetComponent<Selectable>();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            State.StateChanged -= OnStateChanged;
        }

        protected override void OnEnable() {
            base.OnEnable();

            UpdateParent();
            UpdateChildren();
            OnStateChanged();
        }

        protected override void OnDisable() {
            base.OnDisable();

            if(!State.IsOpened) return;

            Close(true);

            UpdateChildren();
            OnStateChanged();
        }

        public virtual void OnPointerClick(PointerEventData eventData) {
            UIRoot.Select(null);
            Focus();
        }

        public virtual void Focus(bool setSelection = false) {
            if(State.IsFocusedThis || !UIRoot || !Focusable) {
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
            if(!State.IsFocusedThis) {
                return;
            }

            FocusedComponent = null;
        }

        // TODO leave just one inheritable open/close/submit/cancel method
        // TODO create PreOpen (and rename AfterOpen to PostOpen) methods to handle other cases (like setting this to the last child before opening [UIScene])
        public virtual void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool focus = true,
            bool enable = true) {

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

        protected virtual void PreOpen() { }

        protected virtual void OpenInternal(bool skipAnimation, UIAnimation overrideAnimation) {

            State.IsOpened = true;

            OpenChildren(UIOpenType.OpenWithParent, skipAnimation);

            if(skipAnimation) {
                OpenChildren(UIOpenType.OpenAfterParent, true);
            }

            if(!HasAnimator || skipAnimation) {
                // TODO hook up the transition state to the parent's transition state (set it to true if the parent is transitioning and hook to parent's TransitionEnded event and set the transition to false & unhook)
                // InTransition = Parent.InTransition
                // Parent.State.TransitionEnded += OnParentTransitionEnded; 
                // OnParentTransitionEnded() => { InTransition = false; Parent.State.TransitionEnded -= OnParentTransitionEnded; }
                // TODO do the same for the OnClose method (when instant)
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
                   child.State.IsOpened ||
                   child.OpenType != openTypeFilter) {
                    continue;
                }

                child.Open(skipAnimation, enable: false, focus: false);
            }
        }

        public void Close(bool skipAnimation = false, UIAnimation overrideAnimation = null) {
            if(!State.IsOpened) {
                return;
            }

            CloseInternal(skipAnimation, overrideAnimation);
        }

        protected virtual void CloseInternal(bool skipAnimation = false, UIAnimation overrideAnimation = null,
            bool closeChildren = true) {

            State.IsOpened = false;

            if(State.IsFocusedThis) {
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
        
        /// <summary>
        /// Called after the object has been closed and all close animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterClose() {
            if(HasAnimator) {
                Animator.ResetToSavedProperties();
                Renderer.enabled = false; // instantly hide this, the state will update on the next frame
            }
        }

        protected virtual void Update() {
            State.IsInteractable = State.IsOpened && !State.IsDisabled && !State.IsInTransition && InteractableSelf &&
                                   InteractableParent;

            State.IsInTransition = IsAnimating;

            State.IsFocusedThis = this == FocusedComponent;
        }

        protected virtual void OnStateChanged() {
            Renderer.enabled = ShouldRender;
        }

        private void OnChildStateChanged() {
            State.IsRenderingChild = false;
            State.IsFocusedChild = false;

            foreach(var child in ChildComponents) {
                if(!child.isActiveAndEnabled) {
                    continue;
                }

                if(child.ShouldRender) {
                    State.IsRenderingChild = true;
                }

                if(child.State.IsFocused) {
                    State.IsFocusedChild = true;
                }

                if(State.IsRenderingChild && State.IsFocusedChild) {
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
                    childComponent.State.StateChanged -= OnChildStateChanged;
                }
            }

            if(!gameObject.activeSelf) {
                // unhook from child events and return if the game object is disabled
                return;
            }

            ChildComponents = GetComponentsInChildren<UIComponent>(includeInactive: true)
                .Where(child => child.Parent == this).ToList();

            foreach(var childComponent in ChildComponents) {
                childComponent.State.StateChanged += OnChildStateChanged;
            }

            OnChildStateChanged();
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

            _firstFocused = gameObject.GetComponentsInChildren<UIComponent>().Where(c => c.Focusable && c != this)
                .Select(c => c.gameObject).FirstOrDefault();

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
                stringBuilder.AppendLine("<b>Opened: </b>" + State.IsOpened);
                stringBuilder.AppendLine("<b>Rendering: </b>" + ShouldRender);
                stringBuilder.AppendLine("<b>In Transition: </b>" + State.IsInTransition);
                stringBuilder.AppendLine("<b>Focused: </b>" + State.IsFocusedThis);
                stringBuilder.AppendLine("<b>Disabled: </b>" + State.IsDisabled);
                stringBuilder.AppendLine("<b>Interactable: </b>" + State.IsInteractable);
                stringBuilder.AppendLine("<b>Focused Child: </b>" + State.IsFocusedChild);
                stringBuilder.AppendLine("<b>Visible Child: </b>" + State.IsRenderingChild);
                return stringBuilder.ToString();
            }
        }

        // TODO get rid of that
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
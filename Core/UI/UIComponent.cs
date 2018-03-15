using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Helpers;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    
    // TODO simple loading helper - sets loading to true/false based on delegate/unity event
    // TODO simple hoverable/pressable helpers - set hovered/pressed based on unity events
    
    // TODO move the focus logic to the UIPanel; make an unfocusable UIElement (use it to animate inputs and similar shit)
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIState))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : BaseUIBehaviour, IPointerClickHandler {
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
        private List<UIComponent> _childComponents = new List<UIComponent>();

        public UIComponent ParentComponent { get; private set; }

        protected List<UIComponent> ChildComponents {
            get { return _childComponents; }
            set { _childComponents = value; }
        }

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

        public virtual bool IsRendering {
            get { return State.IsOpened || State.IsInTransition || State.IsRenderingChild; }
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
            get { return ParentComponent == null || ParentComponent.State.IsInteractable; }
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
                if(ParentComponent && (!ParentComponent.isActiveAndEnabled || !ParentComponent.State.IsOpened)) {
                    return false;
                }
                
                if(State.IsOpened || !isActiveAndEnabled) {
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
            if(eventData == null || eventData.button != PointerEventData.InputButton.Left) {
                return;
            }
            
            UIRoot.Select(gameObject);
            Focus(setSelection: false, autoFocus: false);
        }

        /// <summary>
        /// Focuses the next component. If autoFocus is set to true, tries to find the most appropriate component to focus next.
        /// </summary>
        /// <param name="setSelection">If set to true, tries to select a Selectable object via the builtin system.</param>
        /// <param name="autoFocus">If set to true, tries to find the most appropriate component to focus next.</param>
        public virtual void Focus(bool setSelection = false, bool autoFocus = true) {
            if(State.IsFocusedThis || !UIRoot || !Focusable) {
                return;
            }

            var focusedComponent = autoFocus ? FindNextFocusedComponent() : this;
            
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
            if(ParentComponent) {
                return ParentComponent.FindNextFocusedComponent();
            }

            if(UIScene.CurrentScene == this) {
                return null;
            }

            return UIScene.CurrentScene.FindNextFocusedComponent();

        }

        public static void Unfocus() {
            FocusedComponent = null;
        }
        
        protected virtual void BeforeOpen(bool skipAnimation) { }

        // TODO mimic this approach with the Close method
        public void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool focus = true,
            bool enable = true) {

            if(enable && !gameObject.activeSelf) {
                gameObject.SetActive(true);
            }

            if(!CanOpen) {
                // If opening an opened component - focus it instead
                if(State.IsOpened && focus) {
                    Focus(true);
                }

                return;
            }

            BeforeOpen(skipAnimation);

            State.IsOpened = true;

            OpenChildren(UIOpenType.OpenWithParent, skipAnimation);

            if(skipAnimation) {
                OpenChildren(UIOpenType.OpenAfterParent, true);
            }

            if(!HasAnimator || skipAnimation) {
                AfterOpen(focus);
                return;
            }

            Animator.ResetToSavedProperties();

            var animation = overrideAnimation;

            if(animation == null) {
                animation = Animator.GetAnimation(UIAnimationType.OnOpen);
            }

            Animator.Play(animation, callback: () => AfterOpen(focus));

            // components flash before disappearing
            // maybe tab navigation shouldn't jump between components; that means the animator should work decoupled with the UIComponent; maybe make a simple UIComponent that doesn't care about the focus (move the focus to the UIPanel)  
        }

        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        /// <param name="focus"></param>
        protected virtual void AfterOpen(bool focus) {
            if(focus) {
                Focus(true);
            }
            
            OpenChildren(UIOpenType.OpenAfterParent, false);

            // send another select event to the selected component; otherwise Unity is likely not to focus it 
            if(UIRoot.SelectedObject.transform.IsChildOf(transform)) {
                UIRoot.Select(UIRoot.SelectedObject);
            }
        }

        protected void OpenChildren(UIOpenType openTypeFilter, bool skipAnimation) {
            foreach(var child in ChildComponents) {
                if(!child.CanOpen ||
                   child.OpenType != openTypeFilter) {
                    continue;
                }

                child.Open(skipAnimation, enable: false, focus: false);
            }
        }

        // TODO mimic the Open method structure; add BeforeClose and AfterClose methods and hook them up
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
        protected void AfterClose() {
            if(HasAnimator) {
                Animator.ResetToSavedProperties();
                Renderer.enabled = false; // instantly hide this, the state will update on the next frame
            }
        }

        protected virtual void Update() {
            State.IsInteractable = State.IsOpened && !State.IsDisabled && !State.IsInTransition && InteractableSelf &&
                                   InteractableParent;

            // Set the state to InTransition if either this is animating or this can't animate and its' parent is animating
            State.IsInTransition = IsAnimating ||
                                   !HasAnimator && ParentComponent != null && ParentComponent.State.IsInTransition;

            State.IsFocusedThis = this == FocusedComponent;
            
            State.IsRenderingChild = ChildComponents.Any(child => child.IsRendering);
            State.IsFocusedChild = ChildComponents.Any(child => child.State.IsFocused);
        }

        protected virtual void OnStateChanged() {
            Renderer.enabled = IsRendering;
        }

        private void UpdateParent() {
            if(Transform.parent != null) {
                ParentComponent = Transform.parent.GetComponentsInParent<UIComponent>(includeInactive: true).FirstOrDefault();
                return;
            }

            ParentComponent = null;
        }

        private void UpdateChildren() {
            if(!gameObject.activeSelf) {
                // unhook from child events and return if the game object is disabled
                return;
            }

            ChildComponents = GetComponentsInChildren<UIComponent>(includeInactive: true)
                .Where(child => child.ParentComponent == this).ToList();
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

            // top level elements
            if(ParentComponent == null) {
                _openType = UIOpenType.OpenManually;
            }

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
                stringBuilder.AppendLine("<b>Rendering: </b>" + IsRendering);
                stringBuilder.AppendLine("<b>In Transition: </b>" + State.IsInTransition);
                stringBuilder.AppendLine("<b>Focused: </b>" + State.IsFocusedThis);
                stringBuilder.AppendLine("<b>Disabled: </b>" + State.IsDisabled);
                stringBuilder.AppendLine("<b>Interactable: </b>" + State.IsInteractable);
                stringBuilder.AppendLine("<b>Focused Child: </b>" + State.IsFocusedChild);
                stringBuilder.AppendLine("<b>Visible Child: </b>" + State.IsRenderingChild);
                return stringBuilder.ToString();
            }
        }

        // TODO get rid of this
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
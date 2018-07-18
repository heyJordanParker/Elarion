using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.Saved.Events.UnityEvents;
using Elarion.UI.Helpers;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    // TODO hook to the parent using events (UpdateChildren, Before/After Open/Close

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : UIState {
        [SerializeField, HideInInspector]
        [Tooltip(
            "When to open the component. Auto opens it based on element type. WithParent opens it at the same time as the parent opens (animations overlap). Open after parent waits for the parent animation to finish and then opens the element. Manual doesn't auto-open the component.")]
        private UIOpenType _openType = UIOpenType.WithParent;
        [SerializeField, HideInInspector]
        [Tooltip(
            "When to close the component. Auto closes it based on element type. WithParent closes it at the same time as the parent closes (animations overlap). Close after parent waits for the parent animation to finish and then closes the element. Manual doesn't auto-close the component.")]
        private UIOpenType _closeType = UIOpenType.WithParent;

        [SerializeField, HideInInspector]
        private UIComponent _overrideParentComponent;

        [SerializeField, HideInInspector]
        private BoolUnityEvent _beforeOpenEvent = new BoolUnityEvent();
        [SerializeField, HideInInspector]
        private UnityEvent _afterOpenEvent = new UnityEvent();
        
        [SerializeField, HideInInspector]
        private BoolUnityEvent _beforeCloseEvent = new BoolUnityEvent();
        [SerializeField, HideInInspector]
        private UnityEvent _afterCloseEvent = new UnityEvent();
        
        private UIAnimator _animator;
        private IAnimationController[] _animationControllers = { };
        private UIComponent _parentComponent;

        public event Action BeforeUpdate = () => { };

        public UIComponent ParentComponent {
            get => _parentComponent;
            private set {
                if(_parentComponent == value) {
                    return;
                }

                if(_parentComponent) {
                    _parentComponent.BeforeUpdate -= UpdateComponent;
                    _parentComponent.BeforeOpenEvent.RemoveListener(OnParentBeforeOpen);
                    _parentComponent.AfterOpenEvent.RemoveListener(OnParentAfterOpen);
                    _parentComponent.BeforeCloseEvent.RemoveListener(OnParentBeforeClose);
                    _parentComponent.AfterCloseEvent.RemoveListener(OnParentAfterClose);
                }
                // unhook
                
                _parentComponent = value;

                if(_parentComponent) {
                    _parentComponent.BeforeUpdate += UpdateComponent;
                    _parentComponent.BeforeOpenEvent.AddListener(OnParentBeforeOpen);
                    _parentComponent.AfterOpenEvent.AddListener(OnParentAfterOpen);
                    _parentComponent.BeforeCloseEvent.AddListener(OnParentBeforeClose);
                    _parentComponent.AfterCloseEvent.AddListener(OnParentAfterClose);
                }
                // rehook
            }
        }
        
                  
        protected void CloseChildren(UIOpenType closeTypeFilter, bool skipAnimation) {
//            foreach(var child in ChildComponents) {
//                
//                if(closeTypeFilter == UIOpenType.WithParent) {
//                    if(child.CloseType != UIOpenType.WithParent && child.CloseType != UIOpenType.Auto) {
//                        continue;
//                    }
//                } else if(child.CloseType != closeTypeFilter) {
//                    continue;
//                }
//                
//                var childAnimation = skipAnimation ? null : child.GetAnimation(UIAnimationType.OnClose);
//
//                child.CloseInternal(childAnimation, false);
//            }
        }

        private void OnParentBeforeClose(bool skipAnimation) {
            if(CloseType != UIOpenType.WithParent && CloseType != UIOpenType.Auto) {
                return;
            }
                
            var childAnimation = skipAnimation ? null : GetAnimation(UIAnimationType.OnClose);

            CloseInternal(childAnimation, false);
        }
        
        
        private void OnParentAfterClose() {
            if(CloseType != UIOpenType.AfterParent) {
                return;
            }
                
            CloseInternal(null, true);
        }


        protected void OpenChildren(UIOpenType openTypeFilter, bool skipAnimation) {
//            foreach(var child in ChildComponents) {
//                
//                if(!child.CanOpen) {
//                    continue;
//                }
//                
//                if(openTypeFilter == UIOpenType.WithParent) {
//                    if(child.OpenType != UIOpenType.WithParent && child.OpenType != UIOpenType.Auto) {
//                        continue;
//                    }
//                } else if(child.OpenType != openTypeFilter) {
//                    continue;
//                }
//                
//                var childAnimation = skipAnimation ? null : child.GetAnimation(UIAnimationType.OnOpen);
//
//                child.OpenInternal(childAnimation, false);
//            }
        }
        
        protected virtual void OnParentBeforeOpen(bool skipAnimation) {
            if(!CanOpen) {
                return;
            }
                
            if(OpenType != UIOpenType.WithParent && OpenType != UIOpenType.Auto) {
                return;
            }
                
            var animation = skipAnimation ? null : GetAnimation(UIAnimationType.OnOpen);

            OpenInternal(animation, false);
        }

        protected virtual void OnParentAfterOpen() {
            if(!CanOpen) {
                return;
            }
                
            if(OpenType != UIOpenType.AfterParent) {
                return;
            }
                
            var animation = GetAnimation(UIAnimationType.OnOpen);

            OpenInternal(animation, false);
        }

        public UIComponent OverrideParentComponent {
            get => _overrideParentComponent;
            private set => _overrideParentComponent = value;
        }

        protected List<UIComponent> ChildComponents { get; set; } = new List<UIComponent>();

        public UIAnimator Animator {
            get {
                if(_animator == null) {
                    _animator = GetComponent<UIAnimator>();
                }

                return _animator;
            }
        }

        public UIOpenConditions OpenConditions { get; protected set; }

        public UIOpenType OpenType {
            get => _openType;
            set => _openType = value;
        }

        public UIOpenType CloseType => _closeType;

        public bool IsRendering => IsOpened || IsInTransition;

        protected virtual bool InteractableSelf {
            get => true;
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        protected bool InteractableParent => ParentComponent == null || ParentComponent.IsInteractable;

        public bool HasAnimator => Animator != null && Animator.enabled;

        public bool IsAnimating {
            get {
                foreach(var animationController in _animationControllers) {
                    if(animationController.Animating) {
                        return true;
                    }
                }

                return false;
            }
        }

        public abstract float Alpha { get; set; }

        public abstract Behaviour Renderer { get; }

        protected virtual bool CanOpen {
            get {
                if(IsOpened || !isActiveAndEnabled) {
                    return false;
                }

                if(OpenConditions && !OpenConditions.CanOpen) {
                    return false;
                }

                return true;
            }
        }
        
        public BoolUnityEvent BeforeOpenEvent => _beforeOpenEvent;

        public UnityEvent AfterOpenEvent => _afterOpenEvent;

        public BoolUnityEvent BeforeCloseEvent => _beforeCloseEvent;

        public UnityEvent AfterCloseEvent => _afterCloseEvent;

        protected override void Awake() {
            base.Awake();
            _animator = GetComponent<UIAnimator>();
            _animationControllers = GetComponents<IAnimationController>();

            OpenConditions = GetComponent<UIOpenConditions>();
        }

        protected override void Start() {
            base.Start();

            if(ParentComponent == null && OpenType == UIOpenType.Auto) {
                Open();
            }
        }

        protected override void OnEnable() {
            base.OnEnable();

            UpdateParent();

            UpdateState();
        }

        protected override void OnDisable() {
            base.OnDisable();

            if(!IsOpened) return;

            Close(true);

            UpdateState();
        }

        /// <summary>
        /// Updates the topmost UIComponents. UIComponents then propagate the state update to their children.
        /// </summary>
        protected override void Update() {
            if(ParentComponent) {
                return;
            }

            UpdateComponent();
        }
        
        public void Open(bool skipAnimation = false) {
            var animation = skipAnimation ? null : GetAnimation(UIAnimationType.OnOpen);

            OpenInternal(animation, true);
        }

        public void Open(UIAnimation overrideAnimation) {
            var animation = overrideAnimation != null ? overrideAnimation : GetAnimation(UIAnimationType.OnOpen);
            
            OpenInternal(animation, true);
        }
        
        public void Close(bool skipAnimation = false) {
            var animation = skipAnimation ? null : GetAnimation(UIAnimationType.OnClose);
            CloseInternal(animation, true);
        }

        public void Close(UIAnimation overrideAnimation) {
            var animation = overrideAnimation != null ? overrideAnimation : GetAnimation(UIAnimationType.OnClose);
            
            CloseInternal(animation, true);
        }

        protected virtual void BeforeOpen(bool skipAnimation) {
            BeforeOpenEvent.Invoke(skipAnimation);
        }

        protected virtual void OpenInternal(UIAnimation animation, bool isEventOrigin) {
            if(isEventOrigin && !gameObject.activeSelf) {
                gameObject.SetActive(true);
            }

            if(!CanOpen) {
                return;
            }
            
            // Update children
            ChildComponents = GetComponentsInChildren<UIComponent>(includeInactive: true)
                .Where(child => child.ParentComponent == this).ToList();
            
            IsOpened = true;

            var noAnimation = animation == null;
            
            BeforeOpen(noAnimation);

            OpenChildren(UIOpenType.WithParent, noAnimation);

            if(!HasAnimator || noAnimation) {
                AfterOpen();
                return;
            }

            Animator.ResetToSavedProperties();

            Animator.Play(animation, callback: AfterOpen);
        }

        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterOpen() {
            AfterOpenEvent.Invoke();
            
            OpenChildren(UIOpenType.AfterParent, false);
        }

        protected virtual void BeforeClose(bool skipAnimation) {
            BeforeCloseEvent.Invoke(skipAnimation);
        }

        /// <summary>
        /// Close implementation. Override this to modify the base functionality.
        /// </summary>
        /// <param name="animation">The aimation to play while closing (can be null).</param>
        /// <param name="isEventOrigin">The initial closed element has isEventOrigin set to true.
        /// Its' child objects (that also get closed) have isEventOrigin set to false.</param>
        protected virtual void CloseInternal(UIAnimation animation, bool isEventOrigin) {
            if(!IsOpened) {
                return;
            }

            IsOpened = false;

            var noAnimation = animation == null;
            
            BeforeClose(noAnimation);
            
            CloseChildren(UIOpenType.WithParent, noAnimation);

            if(!HasAnimator || noAnimation) {
                AfterClose();
                return;
            }

            Animator.Play(animation, callback: AfterClose);
        }

        /// <summary>
        /// Called after the object has been closed and all close animations have finished playing (if any)
        /// </summary>
        protected virtual void AfterClose() {
            AfterCloseEvent.Invoke();
            
            CloseChildren(UIOpenType.AfterParent, true);
            
            if(HasAnimator) {
                Animator.ResetToSavedProperties();
                Renderer.enabled = false; // instantly hide this, the state will update on the next frame
            }
        }

        protected virtual void UpdateComponent() {
            BeforeUpdate();

            UpdateState();
        }

        protected override void UpdateState() {
            IsInteractable = IsOpened && !IsDisabled && !IsInTransition && InteractableSelf &&
                             InteractableParent;

            // Set the state to InTransition if either this is animating or this can't animate and its' parent is animating
            IsInTransition = IsAnimating ||
                             !HasAnimator && ParentComponent != null && ParentComponent.IsInTransition;

            // Finish updating the state
            base.UpdateState();
        }

        protected UIAnimation GetAnimation(UIAnimationType type) {
            return !HasAnimator ? null : Animator.GetAnimation(type);

        }

        protected override void OnStateChanged(States currentState, States previousState) {
            Renderer.enabled = IsRendering;

            base.OnStateChanged(currentState, previousState);
        }

        private void UpdateParent() {
            if(OverrideParentComponent) {
                ParentComponent = OverrideParentComponent;
                return;
            }
            
            if(Transform.parent != null) {
                ParentComponent = Transform.parent.GetComponentsInParent<UIComponent>(includeInactive: true)
                    .FirstOrDefault();
                return;
            }

            ParentComponent = null;
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();
            UpdateParent();
        }

        protected override void OnValidate() {
            base.OnValidate();

            UpdateParent();

            // top level elements
            if(ParentComponent == null) {
                if(_openType != UIOpenType.Manual && _openType != UIOpenType.Auto) {
                    _openType = UIOpenType.Manual;
                }
                
                if(_closeType != UIOpenType.Manual && _closeType != UIOpenType.Auto) {
                    _closeType = UIOpenType.Manual;
                }
            }
        }

        public string Description {
            get {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<b>" + GetType().Name + ": </b>" + name);
                stringBuilder.AppendLine("<b>Opened: </b>" + IsOpened);
                stringBuilder.AppendLine("<b>Rendering: </b>" + IsRendering);
                stringBuilder.AppendLine("<b>In Transition: </b>" + IsInTransition);
                stringBuilder.AppendLine("<b>Focused: </b>" + IsFocusedThis);
                stringBuilder.AppendLine("<b>Disabled: </b>" + IsDisabled);
                stringBuilder.AppendLine("<b>Interactable: </b>" + IsInteractable);
                return stringBuilder.ToString();
            }
        }

        protected static UIManager UIManager => UIManager.Instance;
    }
}
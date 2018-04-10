using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elarion.UI.Helpers;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    // TODO simple loading helper - sets loading to true/false based on delegate/unity event
    // TODO simple hoverable/pressable helpers - set hovered/pressed based on unity events
    // TODO simple tooltip

    // TODO move the child closing logic to the child UIComponents; Send ParentClosing; ParentOpening/ParentFinishedOpening events and let child objects open/close accordingly based on what the parent is doing

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : UIState {
        [SerializeField, HideInInspector]
        [Tooltip(
            "When to open the component. Auto opens it based on element type. OpenWithParent opens it at the same time as the parent opens (animations overlap). Open after parent waits for the parent animation to finish and then opens the element. Manual doesn't auto-open the component.")]
        private UIOpenType _openType = UIOpenType.OpenWithParent;

        private UIManager _uiManager;

        private UIAnimator _animator;
        private IAnimationController[] _animationControllers = { };
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

        public UIOpenType OpenType {
            get { return _openType; }
        }

        public virtual bool IsRendering {
            get { return IsOpened || IsInTransition || IsRenderingChild; }
        }

        public virtual bool IsRenderingChild {
            get { return ChildComponents.Any(child => child.IsRendering); }
        }

        protected virtual bool InteractableSelf {
            get { return true; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        protected bool InteractableParent {
            get { return ParentComponent == null || ParentComponent.IsInteractable; }
        }

        public bool HasAnimator {
            get { return Animator != null && Animator.enabled; }
        }

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
                if(ParentComponent && (!ParentComponent.isActiveAndEnabled || !ParentComponent.IsOpened)) {
                    return false;
                }

                if(IsOpened || !isActiveAndEnabled) {
                    return false;
                }

                if(OpenConditions && !OpenConditions.CanOpen) {
                    return false;
                }

                return true;
            }
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

            // TODO register all top-level components in the UIManager; Update them from there

            UpdateComponent();
        }

        protected virtual void BeforeOpen(bool skipAnimation) { }

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
            
            var noAnimation = animation == null;

            BeforeOpen(noAnimation);

            IsOpened = true;

            OpenChildren(UIOpenType.OpenWithParent, noAnimation);

            if(noAnimation) {
                OpenChildren(UIOpenType.OpenAfterParent, true);
            }

            if(!HasAnimator || noAnimation) {
                AfterOpen(isEventOrigin);
                return;
            }

            Animator.ResetToSavedProperties();

            Animator.Play(animation, callback: () => AfterOpen(isEventOrigin));
        }

        /// <summary>
        /// Called after the object has been opened and all open animations have finished playing (if any)
        /// </summary>
        /// <param name="isEventOrigin"></param>
        protected virtual void AfterOpen(bool isEventOrigin) {
            OpenChildren(UIOpenType.OpenAfterParent, false);

            // send another select event to the selected component; otherwise Unity is likely not to focus it; hack 
            if(UIManager && UIManager.SelectedObject && UIManager.SelectedObject.transform.IsChildOf(transform)) {
                UIManager.Select(UIManager.SelectedObject);
            }
        }

        protected void OpenChildren(UIOpenType openTypeFilter, bool skipAnimation) {
            foreach(var child in ChildComponents) {
                
                if(!child.CanOpen) {
                    continue;
                }
                
                if(openTypeFilter == UIOpenType.OpenWithParent) {
                    if(child.OpenType != UIOpenType.OpenWithParent && child.OpenType != UIOpenType.Auto) {
                        continue;
                    }
                } else if(child.OpenType != openTypeFilter) {
                    continue;
                }
                
                var childAnimation = skipAnimation ? null : child.GetAnimation(UIAnimationType.OnOpen);

                child.OpenInternal(childAnimation, false);
            }
        }

        protected virtual void BeforeClose() { }

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

            BeforeClose();

            IsOpened = false;

            var noAnimation = animation == null;

            foreach(var child in ChildComponents) {
                var childAnimation = noAnimation ? null : child.GetAnimation(UIAnimationType.OnClose);
                
                child.CloseInternal(childAnimation, false);
            }

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
            if(HasAnimator) {
                Animator.ResetToSavedProperties();
                Renderer.enabled = false; // instantly hide this, the state will update on the next frame
            }
        }

        protected virtual void UpdateComponent() {
            foreach(var childComponent in ChildComponents) {
                if(!childComponent) {
                    Debug.LogWarning("Trying to update a child component that's missing.", gameObject);
                }

                childComponent.UpdateComponent();
            }

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
                if(_openType != UIOpenType.OpenManually && _openType != UIOpenType.Auto) {
                    _openType = UIOpenType.OpenManually;
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
                stringBuilder.AppendLine("<b>Visible Child: </b>" + IsRenderingChild);
                return stringBuilder.ToString();
            }
        }

        protected UIManager UIManager {
            get { return UIManager.Instance; }
        }
    }
}
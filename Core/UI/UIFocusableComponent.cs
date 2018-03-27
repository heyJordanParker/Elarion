using System;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    public abstract class UIFocusableComponent : UIComponent, IPointerClickHandler {
        
        public event Action Focused = () => { };
        public event Action Blurred = () => { };
        
        [SerializeField]
        protected bool focusable = true;

        [SerializeField]
        [ConditionalVisibility("focusable")]
        private GameObject _firstFocused;
        
        private UIFocusableComponent _initialFocusedComponent;
        private Selectable _initialFocusedSelectable;

        public bool IsFocused { get; protected set; }
        
        public virtual bool IsFocusedChild {
            get { return ChildComponents.Any(child => child.IsFocusedThis); }
        }

        public virtual bool Focusable {
            get { return focusable && IsOpened && !IsDisabled; }
        }
        
        public UIFocusableComponent InitialFocusedComponent {
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

        protected override void Awake() {
            base.Awake();
            
            if(!_firstFocused) {
                return;
            }

            _initialFocusedComponent = _firstFocused.GetComponent<UIFocusableComponent>();
            _initialFocusedSelectable = _firstFocused.GetComponent<Selectable>();
        }

        protected override void UpdateState() {
            base.UpdateState();
            
            IsFocusedThis = this == FocusedComponent;

            var isFocused = IsFocusedThis || IsFocusedChild;

            if(isFocused != IsFocused) {
                if(isFocused) {
                    Focused();
                } else {
                    Blurred();
                }

                IsFocused = isFocused;
            }
        }

        protected override void OpenInternal(UIAnimation animation, bool isEventOrigin) {
            if(!CanOpen) {
                // If opening an opened component - focus it instead
                if(IsOpened && isEventOrigin) {
                    Focus(true);
                }
            }
            
            base.OpenInternal(animation, isEventOrigin);
        }

        protected override void AfterOpen(bool isEventOrigin) {
            if(isEventOrigin) {
                Focus(true);
            }
            
            base.AfterOpen(isEventOrigin);
        }

        protected override void CloseInternal(UIAnimation animation, bool isEventOrigin) {
            base.CloseInternal(animation, isEventOrigin);
            
            SwitchFocusOnClose();
        }

        protected virtual void SwitchFocusOnClose() {
            if(IsFocusedThis) {
                var nextFocused = FindNextFocusedComponent();
                if(nextFocused) {
                    nextFocused.Focus(true);
                }
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData) {
            if(eventData == null || eventData.button != PointerEventData.InputButton.Left) {
                return;
            }
            
            Focus(setSelection: false, autoFocus: false);
        }
        
        public void Unfocus() {
            if(FocusedComponent == this) {
                FocusedComponent = null;
            }
        }

        /// <summary>
        /// Focuses the next component. If autoFocus is set to true, tries to find the most appropriate component to focus next.
        /// </summary>
        /// <param name="setSelection">If set to true, tries to select a Selectable object via the builtin system.</param>
        /// <param name="autoFocus">If set to true, tries to find the most appropriate component to focus next.</param>
        public virtual void Focus(bool setSelection = false, bool autoFocus = true) {
            if(IsFocusedThis || !UIRoot || !Focusable) {
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

        private UIFocusableComponent FindNextFocusedComponent() {
            if(Focusable) {
                // recursively find a focusable child
                return InitialFocusedComponent;
            }

            var focusableParentComponent = GetFocusableParentComponent(ParentComponent);
            if(focusableParentComponent) {
                return focusableParentComponent.FindNextFocusedComponent();
            }

            if(UIScene.CurrentScene == this) {
                return null;
            }

            return UIScene.CurrentScene.FindNextFocusedComponent();
        }

        protected override void OnValidate() {
            base.OnValidate();
            
            // update first focused
            if(!focusable) {
                return;
            }

            if(_firstFocused != null) {
                if(!_firstFocused.transform.IsChildOf(transform) || !_firstFocused.activeInHierarchy) {
                    _firstFocused = null;
                } else {
                    return;
                }
            }

            _firstFocused = gameObject.GetComponentsInChildren<UIFocusableComponent>().Where(c => c.Focusable && c != this)
                .Select(c => c.gameObject).FirstOrDefault();

            if(_firstFocused != null) {
                return;
            }

            var selectable = GetComponentInChildren<Selectable>();
            if(selectable != null) {
                _firstFocused = selectable.gameObject;
            }
        }
        
        public static UIFocusableComponent GetFocusableParentComponent(UIComponent component) {
            if(!component) {
                return null;
            }
            
            var parent = component.ParentComponent;

            while(parent != null) {
                var focusableParent = parent as UIFocusableComponent;

                if(focusableParent != null) {
                    return focusableParent;
                }

                parent = parent.ParentComponent;
            }

            return null;
        }
        
        public static UIFocusableComponent FocusedComponent { get; set; }

    }
}
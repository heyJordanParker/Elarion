using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Helpers.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO make this a static invisible manager (initializeOnLoad + a hidden MonoBehaviour that updates it); add the initial scene as a checkbox to scenes and a readonly gameobject field (to easily find the actual one)

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UIRoot : BaseUIBehaviour {
        public bool enableTabNavigation = true;

        [SerializeField, ReadOnly]
        private GameObject _selectedObject;

        private BaseEventData _baseEventData;

        public GameObject SelectedObject {
            get { return _selectedObject; }
            private set {
                _selectedObject = value;
                EventSystem.SetSelectedGameObject(null);
                EventSystem.SetSelectedGameObject(_selectedObject);
            }
        }

        public UIComponent FocusedComponent {
            get { return UIComponent.FocusedComponent; }
        }

        protected override void Awake() {
            base.Awake();
            // This is necessary for blur effects - the shader can't work with the main render texture
            var uiCamera = UIHelper.CreateUICamera("UI Root Camera", transform);
            uiCamera.hideFlags = HideFlags.HideAndDontSave;
        }

        protected override void OnEnable() {
            base.OnEnable();

            if(_current == null) {
                _current = this;
            }
        }

        protected override void OnDisable() {
            base.OnDisable();

            if(_current == this) {
                _current = null;
            }
        }

        protected virtual void Update() {
            // process the events just once and only if focused
            if(_current != this || !EventSystem || !EventSystem.isFocused) {
                return;
            }

            HandleTabNavigation();

            // the user can change the selected game object - if he does, update the selected component accordingly
            if(EventSystem.currentSelectedGameObject == SelectedObject) {
                if(SelectedObject == null) {
                    // Send navigation events to the focused component as a fallback
                    SelectedObject = FocusedComponent ? FocusedComponent.gameObject : null;
                }

                return;
            }

            SelectedObject = EventSystem.currentSelectedGameObject;

            if(SelectedObject == null) {
                return;
            }

            var selectedComponent = SelectedObject.GetComponentInParent<UIComponent>();

            if(selectedComponent != null) {
                selectedComponent.Focus();
            }
        }

        private void HandleTabNavigation() {
            if(!enableTabNavigation ||
               !EventSystem ||
               !EventSystem.sendNavigationEvents ||
               !Input.GetKeyDown(KeyCode.Tab)) {
                return;
            }

            var startingTransform = EventSystem.currentSelectedGameObject
                ? EventSystem.currentSelectedGameObject.transform as RectTransform
                : null;
            var selectable = EventSystem.currentSelectedGameObject
                ? EventSystem.currentSelectedGameObject.GetComponent<Selectable>()
                : null;
            Selectable nextSelectable = null;

            if(selectable && selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None) {
                var reverseNavigationDirection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                bool horizontalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Horizontal);
                bool verticalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Vertical);

                if(verticalNavigation) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnUp
                        : selectable.navigation.selectOnDown;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? startingTransform.FindCloseSelectableOnUp()
                            : startingTransform.FindCloseSelectableOnDown();
                    }
                }

                if(horizontalNavigation && nextSelectable == null) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnLeft
                        : selectable.navigation.selectOnRight;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? startingTransform.FindCloseSelectableOnLeft()
                            : startingTransform.FindCloseSelectableOnRight();
                    }
                }
            }

            if(!selectable && startingTransform) {
                nextSelectable = startingTransform.gameObject.GetSelectableChildren().FirstOrDefault(child =>
                    child.IsInteractable() && child.navigation.mode != Navigation.Mode.None);
            }

            if(nextSelectable == null || !nextSelectable.IsInteractable() ||
               nextSelectable.navigation.mode == Navigation.Mode.None) return;

            var parentComponent = nextSelectable.GetComponentInParent<UIComponent>();

            if(parentComponent != null && !parentComponent.State.IsInteractable) {
                return;
            }

            if(parentComponent) {
                parentComponent.Focus();
            }

            Select(nextSelectable);
        }

        public void Select(Selectable selectable) {
            if(!EventSystem) {
                return;
            }

            // Deselect
            if(!selectable) {
                SelectedObject = null;
                return;
            }

            selectable.Select();

            var input = selectable as InputField;

            if(input != null) {
                // doesn't work if the object is disabled
                input.ActivateInputField();
            }

            // TODO test with TMP

            SelectedObject = selectable.gameObject;
        }

        // this is to prevent multiple update calls
        private static UIRoot _current;

        private static EventSystem EventSystem {
            get { return EventSystem.current; }
        }

        private static List<UIRoot> _uiRootCache;

        public static List<UIRoot> UIRootCache {
            get {
                if(_uiRootCache == null) {
                    _uiRootCache = new List<UIRoot>();
                }

                return _uiRootCache;
            }
        }
    }
}
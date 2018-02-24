using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO make this an invisible manager (initializeOnLoad + a hidden MonoBehaviour that updates it)
    
    // TODO add a fullscreen checkbox to panels (to easily set them to cover the full screen (only in editor.onValidate))

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIRoot : UIBehaviour {
        [SerializeField]
        [FormerlySerializedAs("_initialScene")]
        public UIScene initialScene;

        public bool enableTabNavigation = true;

        [SerializeField, ReadOnly]
        private UIScene[] _scenes;

        [SerializeField, ReadOnly]
        private GameObject _focusedObject;
        [SerializeField, ReadOnly]
        private UIComponent _focusedComponent;
        
        private BaseEventData _baseEventData;

        private UIScene _currentScene;

        protected BaseEventData BaseEventData {
            get {
                if(_baseEventData == null)
                    _baseEventData = new BaseEventData(EventSystem.current);
                _baseEventData.Reset();
                return _baseEventData;
            }
        }
        
        public UIScene CurrentScene {
            get { return _currentScene; }
        }

        public GameObject FocusedObject {
            get { return _focusedObject; }
        }

        public UIComponent FocusedComponent {
            get { return _focusedComponent; }
        }

        protected string SubmitButton {
            get {
                var button = "Submit";

                if(EventSystem == null) return button;
                
                var inputModule = EventSystem.currentInputModule == null
                    ? null
                    : EventSystem.currentInputModule as StandaloneInputModule;

                if(inputModule != null) {
                    button = inputModule.submitButton;
                }

                return button;
            }
        }
        
        protected string CancelButton {
            get {
                var button = "Cancel";

                if(EventSystem == null) return button;
                
                var inputModule = EventSystem.currentInputModule == null
                    ? null
                    : EventSystem.currentInputModule as StandaloneInputModule;

                if(inputModule != null) {
                    button = inputModule.cancelButton;
                }

                return button;
            }
        }

        public void OpenScene(UIScene scene, bool skipAnimation = false, UIAnimation overrideOpenAnimation = null, UIAnimation overrideCloseAnimation = null) {
            if(scene == null || scene == _currentScene) {
                return;
            }

            if(_currentScene != null) {
                _currentScene.Close(skipAnimation, overrideCloseAnimation);
            }

            _currentScene = scene;
            if(!_currentScene.Opened) {
                _currentScene.Open(skipAnimation, overrideOpenAnimation);
            }
        }

        // override all the unneeded fields (and make them useless); make fields that shouldn't be used here virtual and override them with empty values as well
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

        protected override void Start() {
            base.Start();

            if(_scenes == null) {
                _scenes = GetComponentsInChildren<UIScene>();
                if(_scenes == null) {
                    return;
                }
            }
            
            if(!initialScene) {
                initialScene = _scenes.Length > 0 ? _scenes[0] : null;
            }

            OpenScene(initialScene);
        }

        protected virtual void Update() {
            HandleTabNavigation();

            SendNavigationEventsToFocusedComponent();

            // process the events just once and only if focused
            if(_current != this || !EventSystem.isFocused) {
                return;
            }

            if(EventSystem.currentSelectedGameObject == _focusedObject) {
                return;
            }

            _focusedObject = EventSystem.currentSelectedGameObject;

            if(_focusedObject == null) {
                return;
            }

            UIComponent.UnfocusAll();
            
            _focusedComponent = null;

            var focusedTransform = _focusedObject.transform;

            while(focusedTransform != null) {
                var selectedComponent = UIComponent.UIComponentCache.SingleOrDefault(component =>
                    component.gameObject == focusedTransform.gameObject);

                if(selectedComponent != null) {
                    selectedComponent.Focus();
                    _focusedComponent = selectedComponent;
                    return;
                }

                focusedTransform = focusedTransform.parent;
            }
        }

        // off-clicks aren't consistent in unfocusing
        private void HandleTabNavigation() {
            if(!enableTabNavigation ||
               !EventSystem.isFocused ||
               !EventSystem.sendNavigationEvents) {
                return;
            }

            if(!Input.GetKeyDown(KeyCode.Tab)) return;

            var selectable = EventSystem.currentSelectedGameObject ? GetValidSelectableChild(EventSystem.currentSelectedGameObject) : null;
            Selectable nextSelectable = null;
            
            if(selectable) {
                var reverseNavigationDirection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                bool horizontalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Horizontal);
                bool verticalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Vertical);

                if(verticalNavigation) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnUp
                        : selectable.navigation.selectOnDown;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? selectable.FindSelectableOnUp()
                            : selectable.FindSelectableOnDown();
                    }
                }
                
                if(horizontalNavigation && nextSelectable == null) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnLeft
                        : selectable.navigation.selectOnRight;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? selectable.FindSelectableOnLeft()
                            : selectable.FindSelectableOnRight();
                    }
                }
            }

            if(!selectable) {
                if(!_focusedComponent) return;

                nextSelectable = GetValidSelectableChild(_focusedComponent.gameObject);
            }
            
            if(nextSelectable == null || !nextSelectable.IsInteractable() || nextSelectable.navigation.mode == Navigation.Mode.None) return;
            
            // check if the parent UIComponent is interactable
            var parentComponent = UIComponent.GetUIComponentParent(nextSelectable.transform);
                
            if(parentComponent != null && !parentComponent.Interactable) {
                return;
            }
            
            FocusComponent(parentComponent, true);
            SetSelection(nextSelectable);
        }

        private Selectable GetValidSelectableChild(GameObject go) {
            return go.GetSelectableChildren().FirstOrDefault(child => child.IsInteractable() && child.navigation.mode != Navigation.Mode.None);

        }
        
        protected bool SendNavigationEventsToFocusedComponent() {
            if(!EventSystem.isFocused ||
               !EventSystem.sendNavigationEvents ||
               _focusedComponent == null) {
                return false;
            }

            var baseEventData = BaseEventData;

            if(EventSystem.currentInputModule.input.GetButtonDown(SubmitButton)) {
                ExecuteEvents.Execute(_focusedComponent.gameObject, baseEventData,
                    ExecuteEvents.submitHandler);
            }

            if(EventSystem.currentInputModule.input.GetButtonDown(CancelButton)) {
                ExecuteEvents.Execute(_focusedComponent.gameObject, baseEventData,
                    ExecuteEvents.cancelHandler);
            }
            
            return baseEventData.used;
        }
        
        public void SetSelection(Selectable selectable) {
            if(!selectable || !EventSystem) {
                return;
            }
            
            selectable.Select();

            var input = selectable as InputField;

            if(input != null) {
                // doesn't work if the object is disabled
                input.ActivateInputField();
            }
            
            // TODO test with TMP

            _focusedObject = selectable.gameObject;
            EventSystem.SetSelectedGameObject(_focusedObject);
        }

        public void FocusComponent(UIComponent component, bool value) {
            if(!component) {
                return;
            }

            _focusedComponent = value ? component : null;
        }

        protected override void OnValidate() {
            base.OnValidate();

            _scenes = GetComponentsInChildren<UIScene>();

            var animator = GetComponent<UIAnimator>();
            if(animator) {
                Debug.LogWarning("UIRoot objects cannot be animated. Deleting Animator component.", this);
                DestroyImmediate(animator);
            }
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
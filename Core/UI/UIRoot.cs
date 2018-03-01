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
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    // TODO make this an invisible manager (initializeOnLoad + a hidden MonoBehaviour that updates it); add the initial scene as a checkbox to scenes and a readonly gameobject field (to easily find the actual one)
    
    // focus UIComponents in their OnClick function? (don't select anything, but still process tab navigation events & submit/cancel events)
    
    // focus doesn't focus first component (scene 2)
    // open button submits the newly opened component (add _nextFocusedComponent and set the current one to it after the input events)

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UIRoot : BaseUIBehaviour {
        [SerializeField]
        [FormerlySerializedAs("_initialScene")]
        public UIScene initialScene;

        public bool enableTabNavigation = true;

        [SerializeField, ReadOnly]
        private UIScene[] _scenes;

        [SerializeField, ReadOnly]
        private GameObject _selectedObject;
        
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

        // A properly scaled UIRoot is a better indication of the screen width than Screen.width
        public float Width {
            get { return Transform.sizeDelta.x; }
        }
        
        // A properly scaled UIRoot is a better indication of the screen height than Screen.height
        public float Height {
            get { return Transform.sizeDelta.y; }
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
            // process the events just once and only if focused
            if(_current != this || !EventSystem.isFocused) {
                return;
            }
            
            HandleTabNavigation();

            // the user can change the selected game object - if he does, update the selected component accodringly
            if(EventSystem.currentSelectedGameObject == SelectedObject) {
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

        // off-clicks aren't consistent in unfocusing
        private void HandleTabNavigation() {
            if(!enableTabNavigation ||
               !EventSystem.isFocused ||
               !EventSystem.sendNavigationEvents ||
               !Input.GetKeyDown(KeyCode.Tab)) {
                return;
            }

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
                if(!FocusedComponent) return;

                nextSelectable = GetValidSelectableChild(FocusedComponent.gameObject);
            }
            
            if(nextSelectable == null || !nextSelectable.IsInteractable() || nextSelectable.navigation.mode == Navigation.Mode.None) return;
            
            var parentComponent = nextSelectable.GetComponentInParent<UIComponent>();
                
            if(parentComponent != null && !parentComponent.Interactable) {
                return;
            }
            
            if(parentComponent) {
                parentComponent.Focus();
            }
            
            Select(nextSelectable);
        }

        private Selectable GetValidSelectableChild(GameObject go) {
            return go.GetSelectableChildren().FirstOrDefault(child => child.IsInteractable() && child.navigation.mode != Navigation.Mode.None);

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

        protected override void OnValidate() {
            base.OnValidate();
            _scenes = GetComponentsInChildren<UIScene>();
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
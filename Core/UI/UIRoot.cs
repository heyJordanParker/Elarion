using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO add canvas scaler when creating presets
    // TODO use this (as the converging and final point of the UI hierarchy) to handle the blur; when a UI element Opens, all other elements on the same hierarchical level can blur (optional open/close parameter); maybe blur by default, but only when calling Open manually (not when it's called automagically in the children components)

    // TODO add a fullscreen checkbox to panels (to easily set them to cover the full screen (only in editor.onValidate))

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIRoot : UIBehaviour {
        [SerializeField]
        [FormerlySerializedAs("_initialScene")]
        public UIScene initialScene;

        public bool enableTabNavigation = true;

        [SerializeField, ReadOnly]
        private UIScene[] _scenes;

        private GameObject _focusedObject;
        private UIComponent _focusedComponent;

        private UIScene _currentScene;

        public UIScene CurrentScene {
            get { return _currentScene; }
            set {
                if(value == null || value == _currentScene) {
                    return;
                }

                if(_currentScene != null) {
                    _currentScene.Close();
                }

                _currentScene = value;
                _currentScene.Open();
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
            }

            if(!initialScene) {
                initialScene = _scenes.Length > 0 ? _scenes[0] : null;
            }

            CurrentScene = initialScene;
        }

        protected virtual void Update() {
            HandleTabNavigation();
            
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
                    // cache this and unfocus this
                    selectedComponent.Focus();
                    _focusedComponent = selectedComponent;
                    return;
                }

                focusedTransform = focusedTransform.parent;
            }
        }

        private void HandleTabNavigation() {
            if(!enableTabNavigation) {
                return;
            }

            if(!Input.GetKeyDown(KeyCode.Tab)) return;

            Selectable selectable = null;
            Selectable nextSelectable = null;
            
            if(EventSystem.currentSelectedGameObject) {
                selectable = EventSystem.currentSelectedGameObject.GetComponentInChildren<Selectable>();
                
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    nextSelectable = selectable.FindSelectableOnUp();
                } else {
                    nextSelectable = selectable.FindSelectableOnDown();
                }
            }

            if(!selectable) {
                if(!_focusedComponent) return;

                nextSelectable = _focusedComponent.GetComponentInChildren<Selectable>();
            }

            if(nextSelectable == null || !nextSelectable.IsInteractable()) return;
            
            nextSelectable.Select();

            var inputfield = nextSelectable as InputField;
            
            if(inputfield != null)
                inputfield.ActivateInputField();
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
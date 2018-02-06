using System.Collections.Generic;
using System.Linq;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO add canvas scaler when creating presets
    // TODO use this (as the converging and final point of the UI hierarchy) to handle the blur; when a UI element Opens, all other elements on the same hierarchical level can blur (optional open/close parameter); maybe blur by default, but only when calling Open manually (not when it's called automagically in the children components)
    
    // TODO with the same hierarchical level thing, the fullscreen property will most likely be unnecessary
    // TODO remove the fullscreen property
    // TODO add a fullscreen checkbox to panels (to easily set them to cover the full screen (only in editor.onValidate))
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIRoot : UIBehaviour {

        [SerializeField]
        private UIScene _initialScene;
        
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

        protected override void Start() {
            base.Start();

            if(!_initialScene) {
                _initialScene = GetComponentInChildren<UIScene>();
            }
            
            CurrentScene = _initialScene;
        }

        protected override void OnValidate() {
            base.OnValidate();
            var animator = GetComponent<UIAnimator>();
            if(animator) {
                Debug.LogWarning("UIRoot objects cannot be animated. Deleting Animator component.", this);
                DestroyImmediate(animator);
            }
            
            
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
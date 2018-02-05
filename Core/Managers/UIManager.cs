using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elarion.Managers {
    public class UIManager : Singleton {
        
        // TODO a small button component that triggers a screen transition on click; it has fields to override both the opening and the closing animations of the current/next screen
        
        // TODO make sure that all UIScreens/all fullscreen elements live under a canvas; animations won't properly work if they don't
        
        // TODO block inputs during transitions/animations
        
        // TODO screen history - to support back actions
        
        // TODO basic loading screen - an intermediary screen that'll show between transitions and will stay open until a WaitFor function returns true (make it easily customizeable)
        
        // TODO move the currently rendered fullscreen elements to the main canvas (in order) screen then popups (close one after the other)
        
        // TODO default animation ease function should be picked the same way as in UIAnimation (enum dropwdown and a custom value)
        
        [FormerlySerializedAs("initialScreen")]
        public UIScene initialScene;

        [FormerlySerializedAs("uiScreens")]
        public UIScene[] uiScenes;

        public Color transitionBackground = Color.white;

        private Canvas _mainCanvas;
        
        // Blur effects can't operate with the main render texture - they need a camera
        private Camera _uiCamera;

        private UIScene _currentScreen;

        private int _lastScreenWidth;
        private int _lastScreenHeight;

        public UIScene CurrentScreen {
            get { return _currentScreen; }
            private set {
                if(_currentScreen != null) {
                    _currentScreen.Close();
                }
                _currentScreen = value;
                _currentScreen.Open();
            }
        }

        protected override void Awake() {
            base.Awake();
            _uiCamera = UIHelper.CreateUICamera("Main UI Camera");
            _uiCamera.clearFlags = CameraClearFlags.Color;
            _uiCamera.backgroundColor = transitionBackground;
            _uiCamera.transform.SetParent(transform, false);
            CacheScreenSize();

//            _uiElements = Tools.FindSceneObjectsOfType<UIElement>();
            _uiElements = new List<UIComponent>();

            // Enable all screens; Some might be disbled to make using the editor easier
            foreach(var screen in uiScenes) {
                screen.SetActive(true);
            }
            
            // convert the flat array to a more usable structure
            // maybe Dict<RectTransform, UIElement> to find parents quickly
        }

        protected IEnumerator Start() {
            yield return null;
            // wait for a frame
            CurrentScreen = initialScene;
        }
        
        // those are the top level elements
        // not sure if they're needed?
        private List<UIComponent> _uiElements;

        internal void RegisterUIElement(UIComponent component) {
            _uiElements.Add(component);
        }

        internal void UnregisterUIElement(UIComponent component) {
            _uiElements.Remove(component);
        }

        private void CacheScreenSize() {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }
        
        // TODO move that functionality to the the Open/Close functions of UIElements. This is confusing. 
        
        // TODO properties to keep track of visible and fullscreen elements; maybe handle making elements fullscreen here (and blur everything else); possibly handle all element state changes here

        public void Open(UIComponent uiComponent, UIAnimation overrideAnimation = null) {
            if(uiComponent.Opened) {
                Debug.LogWarning("Trying to open a visible element.", uiComponent);
                return;
            }
            
            // what to do when opening an element that's on another screen - not open it, open it and transfer to the screen?

            var uiScreen = uiComponent as UIScene;
            
            if(uiScreen != null) {
                uiScreen.Open();
                _currentScreen.Close();

                CurrentScreen = uiScreen;
                
                // close all other elements (unless the other screen has them as well)
            } else {
                // ui elements should always live inside another canvas - move it to the main canvas/screen canvas before showing; main canvas during transitions, screen canvas by any other time

                // fullscreen popups and menus
                if(!uiComponent.Fullscreen) {
                    // do not blur the rest of the screen
                } else {
                    // blur all other fullscreen elements
                }
                // animate the element into view; 
                // show it alongside the current screen
            }
        }

        public void Close(UIComponent uiComponent, UIAnimation overrideAnimation = null) {
            var uiScreen = uiComponent as UIScene;
            
            if(uiScreen != null) {
                // this is not logical - remove that
                Debug.LogWarning("Cannot manually close a UIScene. If you want an empty UI Open a blank UIScene instead.");
                return;
            }
            
            uiComponent.Close();   
        }

        public UIComponent testElement;
        
        public void Update() {
            if(_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height) {
                CacheScreenSize();
                // TODO use a ScreenSizeChanged event; UIElements visible on a specific resolution might use that
                foreach(var uiScreen in uiScenes) { }
            }

            if(Input.GetKeyDown(KeyCode.U)) {
                testElement.Close();
            }
            
            if(Input.GetKeyDown(KeyCode.Y)) {
                testElement.Open();
            }

            if(Input.GetKeyDown(KeyCode.O)) {
                if(testElement.ShouldRender) {
                    testElement.Close();
                } else {
                    testElement.Open();
                }
            }

            if(Input.GetKeyDown(KeyCode.I)) {
                testElement.Animator.Fade(0, UIAnimationDirection.From);
            }
            
            if(Input.GetKeyDown(KeyCode.J)) {
                CurrentScreen = uiScenes.First(s => s != CurrentScreen);
            }
            if(Input.GetKeyDown(KeyCode.K)) {
                Open(uiScenes.First(s => s != CurrentScreen));
            }
            if(Input.GetKeyDown(KeyCode.L)) { }
        }

#if UNITY_EDITOR
        
        // TODO menu item to enable/disable helper components in the inspector

        public static bool showUIHelperObjects = true;

        protected override void OnValidate() {
            base.OnValidate();
            
            
        }
        
        #endif

        // TODO UIScreens can have the same UI Elements as children - move those UI Elements to another canvas if the next screen has the same elements; otherwise - play their disable/enable animations in the time alloted

        // Play hide/show animations of UI elements when a transition happens (or when they appear); Don't play animations on a screen transition in which both screens contain the element (aka the element just stays)
        // If a screen transition is happening and there are static elements - move them to a third camera, so they don't move

        // TODO use the input blocker object while an type is playing to prevent the user from fidgeting around (but make animations quick to avoid frustration/waiting) (a transparent image + canvas with max priority)
    }
}
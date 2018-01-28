using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Managers {
    public class UIManager : Singleton {
        // TODO make sure that all UIScreens/all fullscreen elements live under a canvas; animations won't properly work if they don't
        
        // TODO block inputs during transitions/animations
        
        // TODO screen history - to support back actions
        
        // TODO basic loading screen - an intermediary screen that'll show between transitions and will stay open until a WaitFor function returns true (make it easily customizeable)
        
        // TODO move the currently rendered fullscreen elements to the main canvas (in order) screen then popups (close one after the other)
        
        // TODO default animation ease function should be picked the same way as in UIAnimation (enum dropwdown and a custom value)
        
        public UIScreen initialScreen;

        // TODO dynamically register elements in the UIManager
        public UIScreen[] uiScreens;
        
        // TODO remove default animations and durations; it's easy enough to mass set those via the inspector
        public UIAnimation defaultAnimation;

        [SerializeField]
        private UIAnimationDuration _defaultAnimationDuration = UIAnimationDuration.Smooth;
        
        [SerializeField, ConditionalVisibility("_defaultAnimationDuration == UIAnimationDuration.Custom")]
        private float _defaultAnimationCustomDuration = .75f;
        
        public Ease defaultAnimationEaseFunction = Ease.Linear;
        
        public Color transitionBackground = Color.white;

        public float DefaultAnimationDuration {
            get {
                if(_defaultAnimationDuration == UIAnimationDuration.Custom) {
                    return _defaultAnimationCustomDuration;
                }

                return (int) _defaultAnimationDuration / 100f;
            }
        }
        
        private Canvas _mainCanvas;
        // Blur effects can't operate with the main render texture - they need a camera
        private Camera _uiCamera;

        private UIScreen _currentScreen;

        private int _lastScreenWidth;
        private int _lastScreenHeight;
        
        public bool InTransition {
            get { return CurrentScreen.InTransition; }
        }

        private UIScreen TransitionToScreen { get; set; }

        public Canvas MainCanvas {
            get {
                if(_mainCanvas == null) {
                    var transitionCanvasGo = new GameObject("Main Canvas");
                    transitionCanvasGo.AddComponent<Image>().color = transitionBackground;
                    _mainCanvas = transitionCanvasGo.AddComponent<Canvas>();
                    _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    _mainCanvas.transform.SetParent(transform);
                }
                return _mainCanvas;
            }
        }

        // TODO open the screen and all child components
        // Option to wait for animations before closing?
        public UIScreen CurrentScreen {
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
            _uiCamera.transform.SetParent(MainCanvas.transform, false);
            CurrentScreen = initialScreen;
            CacheScreenSize();

            _uiElements = Tools.FindSceneObjectsOfType<UIElement>();

            // Enable all screens; Some might be disbled to make using the editor easier
            foreach(var screen in uiScreens) {
                screen.SetActive(true);
            }
            
            // convert the flat array to a more usable structure
            // maybe Dict<RectTransform, UIElement> to find parents quickly
        }

        private List<UIElement> _uiElements;

        internal void RegisterUIElement(UIElement element) {
            _uiElements.Add(element);
        }

        internal void UnregisterUIElement(UIElement element) {
            _uiElements.Remove(element);
        }

        private void CacheScreenSize() {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }
        
        // TODO buttons that change screens with the option to set open & close animations for both screens being changed
        
        // TODO properties to keep track of visible and fullscreen elements; maybe handle making elements fullscreen here (and blur everything else); possibly handle all element state changes here

        public void Open(UIElement uiElement, UIAnimation overrideAnimation = null) {
            if(uiElement.Visible) {
                Debug.LogWarning("Trying to open a visible element.", uiElement);
                return;
            }

            var uiScreen = uiElement as UIScreen;
            
            if(uiScreen != null) {
                uiScreen.Open();
                _currentScreen.Close();

                CurrentScreen = uiScreen;
                
                // close all other elements (unless the other screen has them as well)
            } else {
                // ui elements should always live inside another canvas - move it to the main canvas/screen canvas before showing; main canvas during transitions, screen canvas by any other time

                // fullscreen popups and menus
                if(!uiElement.Fullscreen) {
                    // do not blur the rest of the screen
                } else {
                    // blur all other fullscreen elements
                }
                // animate the element into view; 
                // show it alongside the current screen
            }
        }

        public void Close(UIElement uiElement, UIAnimation overrideAnimation = null) {
            var uiScreen = uiElement as UIScreen;
            
            if(uiScreen != null) {
                // this is not logical - remove that
                Debug.LogWarning("Cannot manually close a UIScreen. If you want an empty UI Open a blank UIScreen instead.");
                return;
            }
            
            uiElement.Close();   
        }

        public UIElement testElement;
        
        public void Update() {
            if(_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height) {
                CacheScreenSize();
                // TODO use a ScreenSizeChanged event; UIElements visible on a specific resolution might use that
                foreach(var uiScreen in uiScreens) {
                    uiScreen.UpdateTexture();
                }
            }

            if(Input.GetKeyDown(KeyCode.U)) {
                testElement.Close();
            }
            
            if(Input.GetKeyDown(KeyCode.Y)) {
                testElement.Open();
            }

            if(Input.GetKeyDown(KeyCode.O)) {
                if(testElement.Active) {
                    testElement.Close();
                } else {
                    testElement.Open();
                }
            }

//            if(Input.GetKeyUp(KeyCode.O)) {
//                element.Animator.Resize(new Vector2(-50, -50));
//            }

            if(Input.GetKeyDown(KeyCode.I)) {
                testElement.Animator.Fade(0, UIAnimationDirection.From);
            }
            
            if(Input.GetKeyDown(KeyCode.J)) {
                CurrentScreen = uiScreens.First(s => s != CurrentScreen);
            }
            if(Input.GetKeyDown(KeyCode.K)) {
                Open(uiScreens.First(s => s != CurrentScreen));
            }
            if(Input.GetKeyDown(KeyCode.L)) {
                StartTransition(uiScreens.First(s => s != CurrentScreen));
            }
        }

        public void StartTransition(UIScreen toScreen, bool autoUpdate = true) {
            if(InTransition) {
                return;
            }
            
            TransitionToScreen = toScreen;
                        
            // if autoUpdate == false - pause both animations
            var fromTransition = CurrentScreen.StartTransition(UIAnimationDirection.From);
            var toTransition = TransitionToScreen.StartTransition(UIAnimationDirection.To);
            
            var animationType = (fromTransition.type | toTransition.type);

            var noTransition = animationType == UITransitionType.None;

            if(noTransition) {
                EndTransition();
                return;
            }

            if(autoUpdate) {
                this.CreateCoroutine(TransitionCoroutine(toTransition.Duration));
            }
        }
        
        private IEnumerator TransitionCoroutine(float transitionDuration) {
            var transitionProgress = 0.0f;

            while(transitionProgress <= 1) {
                UpdateTransition(transitionProgress);
                
                transitionProgress += Time.deltaTime / transitionDuration;
                yield return null;
            }
            EndTransition();
        }

        /// <summary>
        /// Update the transition. This is either called automatically by the UIManager or could be manually called. Manual calls are useful when synchronization with an external system is required (e.g. slide the screen based on touch input).
        /// </summary>
        /// <param name="transitionProgress">Progress in percent</param>
        public void UpdateTransition(float transitionProgress) {
            CurrentScreen.UpdateTransition(transitionProgress);
            TransitionToScreen.UpdateTransition(transitionProgress);
        }

        public void EndTransition() {
            CurrentScreen.EndTransition();
            TransitionToScreen.EndTransition();
            
            CurrentScreen = TransitionToScreen;
            
            TransitionToScreen = null;
        }

        #if UNITY_EDITOR
        
        // TODO menu item to enable/disable helper components in the inspector

        public static bool showUIHelperObjects = true;

        private void OnValidate() {
            if(defaultAnimation == null) {
                defaultAnimation = Resources.Load<UIAnimation>("UI Animations/Default UI Animation");
            }
        }
        
        #endif

        // TODO UIScreens can have the same UI Elements as children - move those UI Elements to another canvas if the next screen has the same elements; otherwise - play their disable/enable animations in the time alloted

        // Play hide/show animations of UI elements when a transition happens (or when they appear); Don't play animations on a screen transition in which both screens contain the element (aka the element just stays)
        // If a screen transition is happening and there are static elements - move them to a third camera, so they don't move

        // TODO use the input blocker object while an type is playing to prevent the user from fidgeting around (but make animations quick to avoid frustration/waiting) (a transparent image + canvas with max priority)
    }
}
using System;
using System.Collections;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Managers {
    public class UIManager : Singleton {
        // TODO block inputs during transitions/animations
        
        // TODO screen history - to support back actions

        private enum TransitionMode {
            None,
            Simultaneous,
            Sequential
        }

        public UIScreen initialScreen;

        // TODO dynamically register panels in the UIManager
        public UIScreen[] uiScreens;

        public UIAnimation defaultAnimation;
        public float defaultAnimationDuration = .75f;
        public Ease defaultAnimationEaseFunction = Ease.Linear;
        public Color transitionBackground = Color.white;

        private Canvas _mainCanvas;
        // Blur effects can't operate with the main render texture - they need a camera
        private Camera _uiCamera;

        private UIScreen _currentScreen;

        private int _lastScreenWidth;
        private int _lastScreenHeight;
        
        public bool InTransition {
            get { return CurrentTransitionMode != TransitionMode.None; }
        }

        private TransitionMode CurrentTransitionMode { get; set; }

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

        public UIScreen CurrentScreen {
            get { return _currentScreen; }
            private set {
                if(_currentScreen != null) {
                    _currentScreen.Fullscreen = false;
                }
                _currentScreen = value;
                _currentScreen.Fullscreen = true;
            }
        }

        // If more than one screen is visible (e.g. a menu and main screen) - render their textures inside this UIScreen and animate it instead of the two separately
        // Add both screen's renders inside, switch their state
        // Calculate the CompoundScreen's dimensions

        // Use this for the Edge menu
        public UIScreen CompoundScreen {
            get {
                var compoundScreenGO = new GameObject("Compound Screen");
                compoundScreenGO.transform.parent = MainCanvas.transform;
                return compoundScreenGO.AddComponent<UIScreen>();
            }
        }

        protected override void Awake() {
            base.Awake();
            _uiCamera = UIHelper.CreateUICamera("Main UI Camera");
            _uiCamera.transform.SetParent(MainCanvas.transform, false);
            CurrentScreen = initialScreen;
            CacheScreenSize();
        }

        private void CacheScreenSize() {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }
        
        // TODO properties to keep track of visible and fullscreen elements; maybe handle making panels fullscreen here (and blur everything else); possibly handle all panel state changes here

        public void Show(UIPanel uiPanel, UIScriptedAnimation animation = null) {
            if(uiPanel.Visible) {
                return;
            }

            if(uiPanel is UIScreen) {
                uiPanel.Fullscreen = true;
                // transition to the screen
                // hide all other elements (unless the other screen has them as well)
            } else {
                // ui elements should always live inside another canvas - move it to the main canvas/screen canvas before showing; main canvas during transitions, screen canvas by any other time

                if(uiPanel is UIElement) {
                    // do not blur the rest of the screen
                } else {
                    // set the current screen focused to false (blur it)
                }
                // animate the panel into view; 
                // show it alongside the current screen
            }
        }

        public void Hide(UIPanel uiPanel) {
            
        }

        // TODO delete after testing 
        public UIEdgeMenu edgeMenu;

        public void Test(float f) {
        }
        
        public void Update() {
            if(_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height) {
                CacheScreenSize();
                // TODO use an event
                foreach(var uiScreen in uiScreens) {
                    uiScreen.UpdateTexture();
                }
            }
            
            if(Input.GetKeyDown(KeyCode.J)) {
                CurrentScreen = uiScreens.First(s => s != CurrentScreen);
            }
            if(Input.GetKeyDown(KeyCode.K)) {
                if(edgeMenu.Visible) {
                    edgeMenu.Hide();
                } else {
                    edgeMenu.Show();
                }
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
                        
            var fromTransition = CurrentScreen.StartTransition(UITransitionDirection.From);
            var toTransition = TransitionToScreen.StartTransition(UITransitionDirection.To);
            
            var animationType = (fromTransition.type | toTransition.type);
                        
            var isAlphaFade = animationType == UITransitionType.AplhaFade;

            var isSlide = (animationType & UITransitionType.Slide) == UITransitionType.Slide;

            if(isAlphaFade || isSlide) {
                CurrentTransitionMode = TransitionMode.Simultaneous;
            } else {
                CurrentTransitionMode = TransitionMode.Sequential;
            }
            
            var noTransition = animationType == UITransitionType.None;

            if(noTransition) {
                EndTransition();
                return;
            }

            if(autoUpdate) {
                var transition = toTransition;

                var transitionDuration = transition.defaultDuration ? defaultAnimationDuration : transition.duration;
                
                this.CreateCoroutine(TransitionCoroutine(transitionDuration));
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
            var fromProgress = transitionProgress;
            var toProgress = transitionProgress;

            if(CurrentTransitionMode == TransitionMode.Sequential) {
                fromProgress = transitionProgress * 2;
                toProgress = (transitionProgress - 0.5f) * 2;
            }

            CurrentScreen.UpdateTransition(fromProgress);
            TransitionToScreen.UpdateTransition(toProgress);
        }

        public void EndTransition() {
            CurrentScreen.EndTransition();
            TransitionToScreen.EndTransition();

            CurrentScreen = TransitionToScreen;

            CurrentTransitionMode = TransitionMode.None;
            TransitionToScreen = null;
        }

        #if UNITY_EDITOR
        
        // TODO menu item to enable/disable helper components in the inspector

        public static bool showUIHelperObjects = true;

        private void OnValidate() {
            if(defaultAnimation == null) {
                defaultAnimation = Resources.Load<UIAnimation>("UIAnimations/Default UIAnimation");
            }
        }
        
        #endif

        // TODO UIScreens can have the same UI Elements as children - move those UI Elements to another canvas if the next screen has the same elements; otherwise - play their disable/enable animations in the time alloted

        // Play hide/show animations of UI elements when a transition happens (or when they appear); Don't play animations on a screen transition in which both screens contain the element (aka the element just stays)
        // If a screen transition is happening and there are static elements - move them to a third camera, so they don't move

        // TODO use the input blocker object while an type is playing to prevent the user from fidgeting around (but make animations quick to avoid frustration/waiting) (a transparent image + canvas with max priority)
    }
}
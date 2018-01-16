using System;
using System.Collections;
using System.Collections.Generic;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animation;
using Elarion.UI.Animations;
using Elarion.Utility;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO make this a generic fullscreen element
    // TODO Add a canvas to the fullscreen element; they need to display properly over other things 
    
    public class UIScreen : UIPanel {
        [SerializeField, Tooltip("Transition effect to show while this screen is going out of view.")]
        protected UIAnimation fromAnimation;

        [SerializeField, Tooltip("Transition effect to show while this screen is coming into view.")]
        protected UIAnimation toAnimation;
        
        private Camera _camera;

        private UIAnimationDirection _activeTransitionDirection;

        private UIAnimation ActiveAnimation {
            get {
                if(_activeTransitionDirection == UIAnimationDirection.From) {
                    if(fromAnimation == null) {
                        return UIManager.defaultAnimation;
                    }
                    return fromAnimation;
                }

                if(toAnimation == null) {
                    return UIManager.defaultAnimation;
                }
                return toAnimation;
            }
        }

        // TODO make the cameras optional
        // TODO test if render images are necessary with the new approach to movement (they might not be); make them optional if they aren't needed
        protected RawImage RenderImage { get; private set; }

        public override RectTransform AnimationTarget {
            get {
                if(InTransition) {
                    return RenderImage.GetComponent<RectTransform>();
                }
                return base.AnimationTarget;
            }
        }

        protected override void Awake() {
            base.Awake();
            _camera = UIHelper.CreateUICamera(gameObject.name + " Camera");
            _camera.SetActive(false);
            
            canvas.worldCamera = _camera;
            
            RenderImage = UIHelper.Create<RawImage>(gameObject.name + " Render", _camera.transform);
            RenderImage.SetActive(false);
            
            // TODO make sure this is fullscreen
            
            UpdateTexture();
        }

        private void OnDestroy() {
            if(_camera != null) {
                if(_camera.targetTexture != null) {
                    Destroy(_camera.targetTexture);
                }
                Destroy(_camera.gameObject);
            }
            if(RenderImage != null) {
                Destroy(RenderImage.gameObject);
            }
        }

        protected override void OnStateChanged(UIState oldState, UIState newState) {
            base.OnStateChanged(oldState, newState);
            
            _camera.SetActive(InTransition);
            RenderImage.SetActive(InTransition);

            if(!Active) {
                return;
            }

            if(InTransition) {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
            } else {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }

        // TODO OnResolutionChanged function; hook it up to the UIManager event 
        public void UpdateTexture() {
            if(_camera.targetTexture != null) {
                Destroy(_camera.targetTexture);
            }

            _camera.targetTexture = UIHelper.CreateRendureTexture(Width, Height);

            RenderImage.texture = _camera.targetTexture;
            RenderImage.rectTransform.sizeDelta = new Vector2(Width, Height);
        }

        protected override void OnOpen() {
            base.OnOpen();
            Fullscreen = true;
        }

        protected override void OnClose() {
            base.OnClose();
            Fullscreen = false;
        }

        // TODO use show/hide methods (parameterless) instead of start/stop transition
        public UIAnimation StartTransition(UIAnimationDirection transitionDirection) {
            _activeTransitionDirection = transitionDirection;

            InTransition = true;
            RenderImage.transform.SetParent(UIManager.MainCanvas.transform, false);
            RenderImage.transform.localPosition = Vector3.zero;
            Alpha = 1;

            RenderImage.transform.SetAsFirstSibling();

            if(ActiveAnimation.type == UITransitionType.Slide) {
                RenderImage.transform.SetAsLastSibling();
            }

            return ActiveAnimation;
        }

        public void UpdateTransition(float transitionProgress) {
            if(!InTransition) {
                return;
            }

            if(transitionProgress < 0 || transitionProgress > 1) {
                return;
            }

            bool isFromTransition = _activeTransitionDirection == UIAnimationDirection.From;

            var easeFunction = ActiveAnimation.Ease;

            switch(ActiveAnimation.type) {
                case UITransitionType.None:
                    break;
                case UITransitionType.Slide:
                    var image = RenderImage;

                    Vector2 cameraPosition = Vector3.zero;
                    Vector2 offsetPosition;

                    switch(ActiveAnimation.slideDirection) {
                        case SlideDirection.Left:
                            offsetPosition = new Vector3(Width, 0);
                            break;
                        case SlideDirection.Right:
                            offsetPosition = new Vector3(-Width, 0);
                            break;
                        case SlideDirection.Up:
                            offsetPosition = new Vector3(0, -Height);
                            break;
                        case SlideDirection.Down:
                            offsetPosition = new Vector3(0, Height);
                            break;
                        default:
                            goto case SlideDirection.Left;
                    }

                    if(isFromTransition) {
                        image.rectTransform.localPosition =
                            cameraPosition.EaseTo(-offsetPosition, transitionProgress, easeFunction);
                    } else {
                        image.rectTransform.localPosition =
                            offsetPosition.EaseTo(cameraPosition, transitionProgress, easeFunction);
                    }
                    break;
                case UITransitionType.AplhaFade:
                    if(isFromTransition) {
                        Alpha = Easing.Ease(1, 0, transitionProgress, easeFunction);
                    } else {
                        Alpha = Easing.Ease(0, 1, transitionProgress, easeFunction);
                    }
                    break;
            }
        }

        public void EndTransition() {
            RenderImage.transform.SetParent(_camera.transform, false);
            
            InTransition = false;

            Fullscreen = _activeTransitionDirection == UIAnimationDirection.To;
            Visible = Fullscreen;
            
            Alpha = 1;
        }
    }
}
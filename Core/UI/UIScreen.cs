using System;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.Utility;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO check for nesting; display an error if UIScreens are nested (can mess up cameras)
    
    // TODO rename to UIScene (similar to android hierarchy)
    
    public class UIScreen : UIPanel {
        [SerializeField, Tooltip("Transition effect to show while this screen is going out of view.")]
        protected UIAnimation fromAnimation;

        [SerializeField, Tooltip("Transition effect to show while this screen is coming into view.")]
        protected UIAnimation toAnimation;
        
        // TODO a single transition - openTransition (or openAnimation when I refactor)
        // TODO also have another enum dropdown - closeTransitionType - what should happen to the screen during transition (Inherit, Mirror, Custom); this structure will move the inherit transition type one level up - it should be removed from the original enum 

        private Camera _camera;

        private UITransitionDirection _activeTransitionDirection;

        private UIAnimation ActiveAnimation {
            get {
                if(_activeTransitionDirection == UITransitionDirection.From) {
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

        protected RawImage RenderImage { get; private set; }

        protected override Transform EffectAnimationTarget {
            get {
                if(InTransition) {
                    return RenderImage.transform;
                }
                return base.EffectAnimationTarget;
            }
        }

        private Image ColorOverlay { get; set; }

        protected override void Awake() {
            base.Awake();
            _camera = UIHelper.CreateUICamera(gameObject.name + " Camera");
            _camera.SetActive(false);
            
            canvas.worldCamera = _camera;
            
            RenderImage = UIHelper.Create<RawImage>(gameObject.name + " Render", _camera.transform);
            RenderImage.SetActive(false);
            
            ColorOverlay = UIHelper.CreateOverlayImage(gameObject.name + " Transition Color", RenderImage.transform);
            ColorOverlay.enabled = false;
            
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
            
            _camera.SetActive(Active);
            RenderImage.SetActive(Active);

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

        public override void Show() {
            base.Show();
            
        }

        public override void Hide() {
            base.Hide();
            
        }

        // TODO use show/hide methods (parameterless) instead of start/stop transition
        public UIAnimation StartTransition(UITransitionDirection transitionDirection) {
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

        // TODO move this (and similar methods) to a common ancestor to the UIScreen & UIEdgeMenu (maybe make it abstract)
        public void UpdateTransition(float transitionProgress) {
            if(!InTransition) {
                return;
            }

            if(transitionProgress < 0 || transitionProgress > 1) {
                return;
            }

            bool isFromTransition = _activeTransitionDirection == UITransitionDirection.From;

            var easeFunction = ActiveAnimation.defaultEaseFunction ? UIManager.defaultAnimationEaseFunction : ActiveAnimation.easeFunction;

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
                case UITransitionType.ColorFade:
                    var color = ActiveAnimation.colorFadeColor;
                    ColorOverlay.enabled = Math.Abs(transitionProgress - 1) > Mathf.Epsilon;
                    if(isFromTransition) {
                        color.a = Easing.Ease(0, ActiveAnimation.colorFadeColor.a, transitionProgress, easeFunction);
                    } else {
                        if(RenderImage.transform.GetSiblingIndex() != RenderImage.transform.parent.childCount - 1) {
                            RenderImage.transform.SetAsLastSibling();
                        }
                        color.a = Easing.Ease(ActiveAnimation.colorFadeColor.a, 0, transitionProgress, easeFunction);
                    }
                    ColorOverlay.color = color;
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

            if(_activeTransitionDirection == UITransitionDirection.To) {
                Fullscreen = true;
            } else {
                Fullscreen = false;
            }

            Alpha = 1;
            ColorOverlay.enabled = false;
        }
    }
}
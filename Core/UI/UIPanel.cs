using System;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animation;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIPanel : BasicUIElement {
        
        // TODO UIForm inheritor - add error checking submitting and so on builtin
        
        // TODO rename this to UIElement and delete the UIElement class (UIElements need both canvas and state - to keep track of them and to disappear)
        
        // TODO more generic UI states - focused, in transition, full screen, compound screen, disabled (panels can be visible and disabled)
        
        // TODO remove the canvas
        
        // if it has a parent - let the user select AppearWithParent, ApearAfterParent, Manual
        // to achieve that create a hidden variable (has parent) and update it with the OnValidate method - don't include it outside the editor

        public UIEffect[] effects;

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;
        protected UIAnimator animator;

        private UIState _state;

        protected UIState State {
            get { return _state; }
            set {
                var oldState = _state;
                
                _state = value;
                OnStateChanged(oldState, _state);
            }
        }
        
        public bool Active {
            get { return State != UIState.Disabled; }
        }

        public bool InTransition {
            get { return State.HasFlag(UIState.InTransition); }
            set { State = State.SetFlag(UIState.InTransition, value); }
        }
        
        public bool Visible {
            get { return State.HasFlag(UIState.Visible); }
            set { State = State.SetFlag(UIState.Visible, value); }
        }
        
        public bool Fullscreen {
            get { return State.HasFlag(UIState.Fullscreen); }
            set { State = State.SetFlag(UIState.Fullscreen, value); }
        }

        protected virtual int Width {
            get { return Screen.width; }
        }

        protected virtual int Height {
            get { return Screen.height; }
        }

        // TODO make the cameras optional
        public virtual RectTransform AnimationTarget {
            get { return Transform; }
        }

        public UIAnimator Animator {
            get { return animator; }
        }

        public CanvasGroup CanvasGroup {
            get { return canvasGroup; }
        }

        public float Alpha {
            get { return canvasGroup.alpha; }
            set { canvasGroup.alpha = Mathf.Clamp01(value); }
        }

        public UIPanel Parent {
            get {
                // check if cached parent is still valid (check for null and traverse parent hierarchy and compare gameobjects with cached parent's gameobject)
                // if not - find the new parent
                return null;
            }
        }

        protected override void Awake() {
            base.Awake();
            
            animator = GetComponent<UIAnimator>();

            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
            
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        protected virtual void Start() {
            if(UIManager == null) {
                Debug.LogWarning("Enabling a UIPanel without a UIManager on the scene.", gameObject);
            }
         
            // TODO register in the UIManager
            // TODO unregister on destroy
        }

        // TODO option to override animation
        public void Open(UIAnimation overrideAnimation = null) {
            if(animator == null) {
                OnOpen();
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, true, OnOpen);
            }
            
            animator.Play(UIAnimationType.OnOpen, true, OnOpen);
            
            // TODO handle child elements
            // turn the element off after animations, effects and child elements' effects and animations
        }

        public void Close(UIAnimation overrideAnimation = null) {
            if(animator == null) {
                OnClose();
                return;
            }

            if(overrideAnimation != null) {
                animator.Play(overrideAnimation, false, OnClose);
            }
            
            animator.Play(UIAnimationType.OnClose, false, OnClose);

            // after all the effects
            // Unblur screen only after the elements have finished their animations; don't blur the screen if the elements haven't changed
        }

        protected virtual void OnOpen() {
            Visible = true;
        }

        protected virtual void OnClose() {
            Visible = false;
        }


        protected virtual void OnStateChanged(UIState oldState, UIState newState) {
            if(oldState == newState) {
                return;
            }
            
            canvas.enabled = Active;
            
            foreach(var effect in effects) {
                if(oldState.HasFlag(effect.state) &&
                   !newState.HasFlag(effect.state)) {
                    
                    effect.Stop(this);
                }
                if(!oldState.HasFlag(effect.state) &&
                   newState.HasFlag(effect.state)) {
                    
                    effect.Start(this);
                }
            }
        }
        
        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
        
    }
}
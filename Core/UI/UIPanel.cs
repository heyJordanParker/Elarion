using System;
using Elarion.Extensions;
using Elarion.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(UIAnimator))]
    public class UIPanel : BasicUIElement {
        
        // TODO UIForm inheritor - add error checking submitting and so on builtin
        
        // TODO rename this to UIElement and delete the UIElement class (UIElements need both canvas and state - to keep track of them and to disappear)
        
        // TODO more generic UI states - focused, in transition, full screen, compound screen, disabled (panels can be visible and disabled)
        
        // TODO remove the canvas

        public UIEffect[] effects;

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;
        protected UIAnimator animator;

        private UIState _state;

        protected UIState State {
            get { return _state; }
            set {
                var oldState = _state;
                
                if(oldState != value) {
                    _state = value;
                    OnStateChanged(oldState, _state);
                }
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

        public float Alpha {
            get { return canvasGroup.alpha; }
            set { canvasGroup.alpha = Mathf.Clamp01(value); }
        }

        protected override void Awake() {
            base.Awake();
            
            animator = GetComponent<UIAnimator>();
            animator.Target = Transform;

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

        public virtual void Close() {
            Visible = false;
            
            // after all the effects
            // Unblur screen only after the elements have finished their animations; don't blur the screen if the elements haven't changed
        }

        public virtual void Open() {
            Visible = true;
            // start hide transition
            // turn the element off after animations, effects and child elements' effects and animations
        }
        
        protected virtual void OnStateChanged(UIState oldState, UIState newState) {
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

            if(oldState == UIState.Disabled && Active) {
                animator.Reset();
            }
        }
        
        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
        
    }
}
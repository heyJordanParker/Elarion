using System.Collections;
using Elarion.Extensions;
using Elarion.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIPanel : BasicUIElement {
        
        // TODO maybe make this abstract (no element would be just a panel?)

        // TODO maybe inherit UIElement (or UIElement inherit this?) and allow panels to have appear/disappear animations and similar things
        
        // TODO more generic UI states - focused, in transition, full screen, compound screen, disabled (panels can be visible and disabled)

        public UIEffect[] effects;
        
        protected Canvas canvas;
        protected CanvasGroup canvasGroup;

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

        protected virtual Transform EffectAnimationTarget {
            get { return transform; }
        }

        public float Alpha {
            get { return canvasGroup.alpha; }
            set { canvasGroup.alpha = Mathf.Clamp01(value); }
        }

        protected override void Awake() {
            base.Awake();
            
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

        public virtual void Show() {
            Visible = true;
            
            // after all the effects
            // Unblur screen only after the elements have finished their animations; don't blur the screen if the elements haven't changed
        }

        public virtual void Hide() {
            Visible = false;
            Fullscreen = false;
            // start hide transition
            // turn the element off after animations, effects and child elements' effects and animations
        }
        
        protected virtual void OnStateChanged(UIState oldState, UIState newState) {
            canvas.enabled = Active;
            
            foreach(var effect in effects) {
                if(oldState.HasFlag(effect.state) &&
                   !newState.HasFlag(effect.state)) {
                    
                    effect.Stop(this, EffectAnimationTarget);
                }
                if(!oldState.HasFlag(effect.state) &&
                   newState.HasFlag(effect.state)) {
                    
                    effect.Start(this, EffectAnimationTarget);
                }
            }
        }
        
        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
        
    }
}
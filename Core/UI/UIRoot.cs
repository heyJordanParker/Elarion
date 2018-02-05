using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    // TODO add canvas scaler when creating presets
    // TODO use this (as the converging and final point of the UI hierarchy) to handle the blur; when a UI element Opens, all other elements on the same hierarchical level can blur (optional open/close parameter); maybe blur by default, but only when calling Open manually (not when it's called automagically in the children components)
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIRoot : UIPanel {
        
        // override all the unneeded fields (and make them useless); make fields that shouldn't be used here virtual and override them with empty values as well
        
        public override bool ShouldRender {
            get { return true; }
        }

        protected override void Awake() {
            base.Awake();
            canvas.enabled = true;
        }

        protected override void OnValidate() {
            base.OnValidate();
            var animator = GetComponent<UIAnimator>();
            if(animator) {
                Debug.LogWarning("UIRoot objects cannot be animated. Deleting Animator component.", this);
                DestroyImmediate(animator);
            }
        }
    }
}
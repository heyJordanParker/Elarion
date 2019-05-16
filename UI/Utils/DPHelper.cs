using Elarion.Common.Attributes;
using Elarion.UI.Helpers.Animation;
using UnityEngine;

namespace Elarion.UI.Utils {
    // TODO Touchscreen keyboard helper - moves a UI panel so that the currently focused thing is visible above the keyboard (if not visible - move it to the top of the screen - that way 1-2-3 inputs can be visible with just a single resize; maybe always resize - we'll see)
    
    // TODO Touchscreen keyboard navigator - a small bar above the touchscreen keyboard containing buttons for the previous element, done, and next element (for lists of input fields. e.g. the note editor)

    /// <summary>
    /// Density-independent pixel helper. Calculates sizes based on DP (works only in the editor).
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class DPHelper : MonoBehaviour {

        public int referenceDPI = 400;
        
        [ConditionalVisibility(enableConditions: "!autoWidth")]
        public bool controlWidth;

        [ConditionalVisibility("controlWidth","!autoWidth")]
        public float widthDP;

        public bool controlHeight;

        [ConditionalVisibility("controlHeight")]
        public bool addStatusBarOffsetOnAndroid;    

        [ConditionalVisibility("controlHeight")]
        public float heightDP;

        [Tooltip("Auto calculate width based on height. Aim for a square.")]
        [ConditionalVisibility("controlHeight")]
        public bool autoWidth;

        public bool controlPositionX;

        [ConditionalVisibility("controlPositionX")]
        public float positionXinDP;
        
        public bool controlPositionY;
        
        [ConditionalVisibility("controlPositionY")]
        public float positionYinDP;

        [SerializeField, HideInInspector]
        private UIAnimator _animator;
        
        [SerializeField, HideInInspector]
        private RectTransform _transform;
        
        private RectTransform Transform {
            get {
                if(!_transform) {
                    _transform = transform as RectTransform;
                }
                return _transform;
            }
        }

        private void Awake() {
            hideFlags = HideFlags.DontSave;
        }

        private void Resize() {
            var sizeDelta = Transform.sizeDelta;
            var position = Transform.anchoredPosition;

            if(controlWidth && !autoWidth) {
                sizeDelta.x = ToPixels(widthDP);
            }

            if(controlHeight) {
                sizeDelta.y = ToPixels(heightDP);

                if(autoWidth) {
                    sizeDelta.x = sizeDelta.y;
                }
            }

            if(controlPositionX) {
                position.x = ToPixels(positionXinDP);
            }

            if(controlPositionY) {
                position.y = ToPixels(positionYinDP);
            }

            Transform.sizeDelta = sizeDelta;
            Transform.anchoredPosition = position;
            
            if(_animator) {
                _animator.SaveProperties();
            }
        }

        private void OnValidate() {
            Resize();
        }

        public float ToPixels(float densityDependantPixels) {
            return densityDependantPixels / (160f / referenceDPI);
        }
    }
}
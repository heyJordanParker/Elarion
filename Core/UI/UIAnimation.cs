using System;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.UI {
    [Serializable]
    public class UIAnimation : ExtendedScriptableObject {
        // TODO handle the duration the same way as in the UIEffect - a few predefined options (slow, normal, fast) and a custom one (with arbitrary value field)
        
        // TODO User-friendly easing - use user-friendly names and a fewer easing functions to make picking one simpler
        
        // TODO maximize type - starts as 1x1 square in one of the corners (or custom position) and maximizes to the whole screen; optionally use a circular mask
        
        // TODO Maximize pick a position from an enum - TopLeft, TopCenter, TopRight... BottomRight, Custom; Custom reveals a Vector2 field to input the correct position
        // TODO resize + fade (resize from an icon to fullscreen & fade between icon & screen)
        
        // cap off-screen movement to Screen size? (this will allow you to setup vectors for movement without worrying about the varying screen size - if you want the object to move offscreen, just setup a pretty large number) 
        
        [Tooltip("Transition Configuration")]
        public UITransitionType type = UITransitionType.AplhaFade;

        [ConditionalVisibility("type == UITransitionType.Slide")]
        public SlideDirection slideDirection = SlideDirection.Left;
        
        [ConditionalVisibility("type == UITransitionType.ColorFade")]
        public Color colorFadeColor = Color.white;

        [ConditionalVisibility("type != UITransitionType.Inherit, type != UITransitionType.None")]
        public bool defaultDuration = true;
        
        [ConditionalVisibility("defaultDuration != true, type != UITransitionType.Inherit, type != UITransitionType.None")]
        public float duration = .75f;

        [ConditionalVisibility("type != UITransitionType.Inherit, type != UITransitionType.None")]
        public bool defaultEaseFunction = true;
        
        [ConditionalVisibility("!defaultEaseFunction, type != UITransitionType.Inherit, type != UITransitionType.None")]
        public Ease easeFunction = Ease.Linear;

        public void StartAnimation(RectTransform target, Vector3? newPosition = null, Vector3? newSize = null, Vector3? newScale = null) {
            
        }

        public void UpdateAnimation(float progress) {
            
        }

        public void StopAnimation() {
            
        }
        
        #if UNITY_EDITOR

        [UnityEditor.MenuItem("Assets/Create/UIAnimation")]
        public static void Save() {
            Save<UIAnimation>();
        }

        #endif

    }
}
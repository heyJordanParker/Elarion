using System;
using Elarion.Attributes;
using Elarion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(UIComponent))]
    public class UIConditionalVisibility : UIBehaviour {
        protected const int MaxScreenSize = 2000;
        
        [Serializable, Flags]
        public enum PlatformFilter {
            Standalone = 1 << 0,
            Mobile = 1 << 1,
            MobileIOS = 1 << 2,
            MobileAndroid = 1 << 3,
            Editor = 1 << 4,
        }
        
        [Serializable]
        [Flags]
        public enum StateFilter {
            Visible = 1 << 0,
            NotVisible = 1 << 1,
            Opened = 1 << 2,
            NotOpened = 1 << 3,
            InTransition = 1 << 4,
            NotInTransition = 1 << 5,
            Focused = 1 << 6,
            NotFocused = 1 << 7
        }

        [Serializable]
        public enum OrientationFiler {
            Any = 0,
            Portrait,
            Landscape
        }

        [HideInInspector]
        public bool platformFilter = false;
        
        [HideInInspector]
        [EnumMultipleDropdown]
        public PlatformFilter platform = PlatformFilter.Standalone | PlatformFilter.Mobile | PlatformFilter.MobileIOS | PlatformFilter.MobileAndroid | PlatformFilter.Editor;

        [HideInInspector]
        public bool screenSizeFilter = false;
        
        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted", order = 2)]
        public Vector2 screenWidth = new Vector2(0, MaxScreenSize);
        
        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted", order = 0)]
        public Vector2 screenHeight = new Vector2(0, MaxScreenSize);
        
        [HideInInspector]
        public bool parentSizeFilter = false;
        
        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted")]
        public Vector2 parentWidth = new Vector2(0, MaxScreenSize);
        
        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted")]
        public Vector2 parentHeight = new Vector2(0, MaxScreenSize);
        
        [HideInInspector]
        public bool parentStateFilter = false;

        [HideInInspector]
        [EnumMultipleDropdown]
        public StateFilter parentState;

        [HideInInspector]
        [Tooltip("Horizontal returns true when Screen.height < Screen.Width. Vertical returns true when Screen.height > Screen.width. Any always returns true.")]
        public bool orientationFilter = false;

        [HideInInspector]
        public OrientationFiler orientation = OrientationFiler.Any;

        public bool ShouldBeVisible {
            get {
                if(!CurrentPlatform.HasFlag(platform)) {
                    return false;
                }

                return true;
            }
        }

        private static PlatformFilter? _currentPlatform;

        protected static PlatformFilter CurrentPlatform {
            get {
                if(!_currentPlatform.HasValue) {
                    _currentPlatform = 0;
                    
#if UNITY_EDITOR
                    _currentPlatform = _currentPlatform.SetFlag(PlatformFilter.Editor, true);
#endif

#if UNITY_STANDALONE
                    _currentPlatform = _currentPlatform.SetFlag(Platform.Standalone, true);
#endif

#if UNITY_ANDROID
                    _currentPlatform = _currentPlatform.SetFlag(Platform.Mobile, true);
                    _currentPlatform = _currentPlatform.SetFlag(Platform.MobileAndroid, true);
#endif

#if UNITY_IOS
                    _currentPlatform = _currentPlatform.SetFlag(PlatformFilter.Mobile, true);
                    _currentPlatform = _currentPlatform.SetFlag(PlatformFilter.MobileIOS, true);
#endif
                    
                }

                return _currentPlatform.Value;
            }
        }


        // manually check for the conditional visibility flag before opening any element (and don't if it's not met)
        
        // Open and close invisible elements as usual, but don't render, focus, or animate them

        // add a descriptive label to the UI Element about the visibility (below the Open Type; include a description for the open type as well)
        
        // add a ToString method that describes the conditions in human-readable form.
    }
}
using System;
using System.Linq;
using System.Text;
using Elarion.Attributes;
using Elarion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    // TODO break this down - it's way too big of a mess
    [RequireComponent(typeof(UIComponent))]
    public class UIOpenConditions : BaseUIBehaviour {
        protected const int MaxScreenSize = 2000;

        [Serializable, Flags]
        public enum PlatformCondition {
            All = -1,
            None = 0 << 0,
            Standalone = 1 << 0,
            Mobile = 1 << 1,
            MobileIOS = 1 << 2,
            MobileAndroid = 1 << 3,
            Editor = 1 << 4,
        }

        [Serializable]
        [Flags]
        public enum StateCondition {
            All = -1,
            None = 0 << 0,
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
        public enum OrientationCondition {
            Portrait = 0,
            Landscape
        }

        [HideInInspector]
        public bool platformCondition = false;

        [HideInInspector]
        [EnumMultipleDropdown]
        public PlatformCondition platform = PlatformCondition.Standalone | PlatformCondition.Mobile | PlatformCondition.MobileIOS |
                                         PlatformCondition.MobileAndroid | PlatformCondition.Editor;

        [HideInInspector]
        public bool screenSizeCondition = false;

        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted", order = 2)]
        public Vector2 screenWidth = new Vector2(0, MaxScreenSize);

        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted", order = 0)]
        public Vector2 screenHeight = new Vector2(0, MaxScreenSize);

        [HideInInspector]
        public bool parentSizeCondition = false;

        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted")]
        public Vector2 parentWidth = new Vector2(0, MaxScreenSize);

        [HideInInspector]
        [MinMaxSlider(0, MaxScreenSize, "Unrestricted", "Unrestricted")]
        public Vector2 parentHeight = new Vector2(0, MaxScreenSize);

        [HideInInspector]
        public bool parentStateCondition = false;

        [HideInInspector]
        [EnumMultipleDropdown]
        public StateCondition parentState;

        [HideInInspector]
        [Tooltip(
            "Horizontal returns true when Screen.height < Screen.Width. Vertical returns true when Screen.height > Screen.width. Any always returns true.")]
        public bool orientationCondition = false;

        [HideInInspector]
        public OrientationCondition orientation = OrientationCondition.Portrait;

        private UIComponent _component;

        protected override void Awake() {
            base.Awake();

            _component = GetComponent<UIComponent>();
        }

        protected void Update() {
            // should be a method in UIComponent
            var componentCanOpen = _component && _component.gameObject.activeSelf && !_component.Opened;
            
            if(componentCanOpen && CanOpen && _component.OpenType != UIOpenType.OpenManually) {
                _component.Open();
            }

            if(_component && _component.Opened && !CanOpen) {
                _component.Close();
            }
        }

        public bool CanOpen {
            get {
                if(platformCondition && !CurrentPlatform.HasFlag(platform)) {
                    return false;
                }

                if(screenSizeCondition) {
                    if((screenWidth.x != 0 && CurrentScreenWidth < screenWidth.x) ||
                       (screenWidth.y != MaxScreenSize && CurrentScreenWidth > screenWidth.y)) {
                        return false;
                    }

                    if((screenHeight.x != 0 && CurrentScreenHeight < screenHeight.x) ||
                       (screenHeight.y != MaxScreenSize && CurrentScreenHeight > screenHeight.y)) {
                        return false;
                    }
                }

                if(parentSizeCondition) {
                    if(!_component || !_component.Parent) {
                        return false;
                    }

                    var width = _component.Parent.Transform.rect.width;
                    var height = _component.Parent.Transform.rect.height;

                    if((parentWidth.x != 0 && width < parentWidth.x) ||
                       (parentWidth.y != MaxScreenSize && width > parentWidth.y)) {
                        return false;
                    }

                    if((parentHeight.x != 0 && height < parentHeight.x) ||
                       (parentHeight.y != MaxScreenSize && height > parentHeight.y)) {
                        return false;
                    }
                }

                if(parentStateCondition) {
                    if(!_component || !_component.Parent) {
                        return false;
                    }

                    if(!ParentStateValid) {
                        return false;
                    }
                }

                if(orientationCondition && orientation != CurrentOrientation) {
                    return false;
                }

                return true;
            }
        }

        private bool ParentStateValid {
            get {
                var parent = _component.Parent as UIComponent;

                if(parent == null) {
                    return false;
                }

                if(parentState == StateCondition.All) {
                    return true;
                }

                var currentState = parent.State;

                foreach(var state in Enum.GetValues(typeof(StateCondition))) {
                    if(!parentState.HasFlag(state)) continue;


                    switch((StateCondition) state) {
                        case StateCondition.Focused:
                            if(currentState.HasFlag(UIState.FocusedThis) ||
                               currentState.HasFlag(UIState.FocusedChild)) {
                                return true;
                            }

                            break;
                        case StateCondition.NotFocused:
                            if(!currentState.HasFlag(UIState.FocusedThis) &&
                               !currentState.HasFlag(UIState.FocusedChild)) {
                                return true;
                            }

                            break;
                        case StateCondition.InTransition:
                            if(currentState.HasFlag(UIState.InTransition)) {
                                return true;
                            }

                            break;
                        case StateCondition.NotInTransition:
                            if(!currentState.HasFlag(UIState.InTransition)) {
                                return true;
                            }

                            break;
                        case StateCondition.Opened:
                            if(currentState.HasFlag(UIState.Opened)) {
                                return true;
                            }

                            break;
                        case StateCondition.NotOpened:
                            if(!currentState.HasFlag(UIState.Opened)) {
                                return true;
                            }

                            break;
                        case StateCondition.Visible:
                            if(_component.ShouldRender) {
                                return true;
                            }

                            break;
                        case StateCondition.NotVisible:
                            if(!_component.ShouldRender) {
                                return true;
                            }

                            break;
                    }
                }

                return false;
            }
        }

        protected float CurrentScreenWidth {
            get {
                if(_component && _component.UIRoot) {
                    return _component.UIRoot.Width;
                }

                return Screen.width;
            }
        }

        protected float CurrentScreenHeight {
            get {
                if(_component && _component.UIRoot) {
                    return _component.UIRoot.Height;
                }

                return Screen.height;
            }
        }

        protected OrientationCondition CurrentOrientation {
            get {
                return CurrentScreenWidth > CurrentScreenHeight
                    ? OrientationCondition.Landscape
                    : OrientationCondition.Portrait;
            }
        }

        private static PlatformCondition? _currentPlatform;

        protected static PlatformCondition CurrentPlatform {
            get {
                if(!_currentPlatform.HasValue) {
                    _currentPlatform = 0;

#if UNITY_EDITOR
                    _currentPlatform = _currentPlatform.SetFlag(PlatformCondition.Editor, true);
#endif

#if UNITY_STANDALONE
                    _currentPlatform = _currentPlatform.SetFlag(Platform.Standalone, true);
#endif

#if UNITY_ANDROID
                    _currentPlatform = _currentPlatform.SetFlag(Platform.Mobile, true);
                    _currentPlatform = _currentPlatform.SetFlag(Platform.MobileAndroid, true);
#endif

#if UNITY_IOS
                    _currentPlatform = _currentPlatform.SetFlag(PlatformCondition.Mobile, true);
                    _currentPlatform = _currentPlatform.SetFlag(PlatformCondition.MobileIOS, true);
#endif

                }

                return _currentPlatform.Value;
            }
        }
    }
}
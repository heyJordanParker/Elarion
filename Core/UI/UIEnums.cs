using System;

namespace Elarion.UI {
    
    [Serializable]
    public enum UIAnimationType {
        OnOpen,
        OnClose,
        OnFocus,
        OnBlur,
        OnHover,
        OnUnhover,
    }

    [Serializable]
    public enum UIAnimationEase {
        Linear = Ease.Linear,
        Smooth = Ease.InOutCubic,
        Fast = Ease.OutExpo,
        Slow = Ease.InExpo,
        Bounce = Ease.InOutBack,
        Custom = int.MaxValue
    }

    [Serializable]
    public enum UIAnimationDuration {
        Fastest = 20,
        Fast = 25,
        Normal = 34,
        Smooth = 40,
        Slow = 50,
        Custom = 0,
    }

    [Serializable]
    public enum UIEffectType {
        Overlay,
        Blur,
        Shadow
        // TODO fade effect (make semi-transparent for the duration)
        // show something effect (spawn a prefab)
        // move, resize, rotate effects
    }
    
    
    // loading state?
    [Serializable, Flags]
    public enum UIState {
        NotInitialized = 0 << 0, // the element is off (in the hierarchy)
        Opened = 1 << 0, // is this element supposed to be visible on screen
        Fullscreen = 1 << 1, // is this element fullscreen
        InTransition = 1 << 2, // is this element in transition (maybe rename to InAnimation)
        HasFocus = 1 << 3, // is this element focused - usually yes, but might not be if there's an edgemenu for example
        Hovered = 1 << 4,
        Clicked = 1 << 5,
        Disabled = 1 << 6, // not interactable while visible; hook UI effects to make it sexy
        VisibleChild = 1 << 7, // a child element is currently active
    }

    [Serializable]
    public enum UIAnimationDirection {
        From,
        To,
        RelativeFrom,
        RelativeTo
    }
    
    [Serializable, Flags]
    public enum ResizeDirection {
        Left = 1 << 0,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right,
    }
}
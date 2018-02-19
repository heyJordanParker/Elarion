using System;

namespace Elarion.UI {

    [Serializable]
    public enum UIOpenType {
        OpenWithParent,
        OpenAfterParent,
        OpenManually
    }
    
    [Serializable]
    public enum UIAnimationType {
        OnOpen,
        OnClose,
        OnFocus,
        OnUnfocus,
    }

    [Serializable]
    [Flags]
    public enum UIEffectTrigger {
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
        NotInitialized = -1,
        None = 0 << 0, // the element is off (in the hierarchy)
        Opened = 1 << 0, // is this element supposed to be visible on screen
        InTransition = 1 << 1, // is this element in transition (maybe rename to InAnimation)
        FocusedThis = 1 << 2, // is this element focused - usually yes, but might not be if there's an edgemenu for example
        FocusedChild = 1 << 3, // a child element is currently active
        Disabled = 1 << 4, // not interactable while visible; hook UI effects to make it sexy
        VisibleChild = 1 << 5, // a child element is currently active
        Interactable = 1 << 6, // is this element interactable (accepting input events)
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
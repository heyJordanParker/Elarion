using System;

namespace Elarion.UI {

    [Serializable]
    public enum UIEffectType {
        Overlay,
        Blur,
        Shadow
    }
    
    [Serializable, Flags]
    public enum UIState {
        Disabled = 0 << 0, // the element is off (in the hierarchy)
        Visible = 1 << 0, // is this element supposed to be visible on screen
        Fullscreen = 1 << 1, // is this element fullscreen
        InTransition = 1 << 2, // is this element in transition (maybe rename to InAnimation)
        HasFocus = 1 << 3, // is this element focused - usually yes, but might not be if there's an edgemenu for example
    }

    [Serializable]
    public enum UITransitionDirection {
        From,
        To
    }
    
    [Serializable]
    public enum UITransitionType {
        Inherit = 0,
        None = 1 << 0,
        Slide = 1 << 1,
        AplhaFade = 1 << 2,
        ColorFade = 1 << 3,
        // TODO maximize
        // maybe zoom
    }
    
    [Serializable]
    public enum SlideDirection {
        Left,
        Right,
        Up,
        Down
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
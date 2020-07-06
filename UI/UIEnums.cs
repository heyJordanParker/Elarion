using System;

namespace Elarion.UI {

    [Serializable]
    public enum UIOpenType {
        Auto,
        WithParent,
        AfterParent,
        Manual
    }
    
    [Serializable]
    public enum UIAnimationType {
        OnOpen,
        OnClose,
        OnFocus,
        OnUnfocus,
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
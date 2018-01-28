using Elarion.Managers;

namespace Elarion.UI.Animation {
    public class UIAnimationOptions {
        private readonly bool _savePosition;
        private readonly bool _instant;
        private readonly Ease? _easeFunction;
        private readonly float _duration;

        public bool SavePosition {
            get { return _savePosition; }
        }

        public bool Instant {
            get { return _instant; }
        }

        public Ease EaseFunction {
            get {
                if(!_easeFunction.HasValue) {
                    if(UIManager == null) {
                        return Ease.Linear;
                    }
                    
                    return UIManager.defaultAnimationEaseFunction;
                }

                return _easeFunction.Value;
            }
        }

        public float Duration {
            get {
                if(_duration <= 0) {
                    if(UIManager == null) {
                        return 1;
                    }
                    
                    return UIManager.DefaultAnimationDuration;
                }

                return _duration;
            }
        }

        public UIAnimationOptions(bool savePosition = false, bool instant = false, Ease? easeFunction = null,
            float duration = -1) {
            _savePosition = savePosition;
            _instant = instant;
            _duration = duration;
            _easeFunction = easeFunction;
        }

        private static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
    }
}
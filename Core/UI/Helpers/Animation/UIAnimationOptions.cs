namespace Elarion.UI.Helpers.Animation {
    public class UIAnimationOptions {
        private readonly bool _savePosition;
        private readonly bool _instant;
        private readonly Ease? _easeFunction;
        private readonly float _duration;
        private readonly float _delay;

        public bool SavePosition {
            get { return _savePosition; }
        }

        public bool Instant {
            get { return _instant; }
        }

        public Ease EaseFunction {
            get {
                if(!_easeFunction.HasValue) {
                    return Ease.Linear;
                }

                return _easeFunction.Value;
            }
        }

        public float Duration {
            get {
                if(_duration <= 0) {
                    return (float) UIAnimationDuration.Normal / 100;
                }

                return _duration;
            }
        }

        public float Delay {
            get { return _delay; }
        }

        public UIAnimationOptions(bool savePosition = false, bool instant = false, Ease? easeFunction = null,
            float duration = 1, float delay = 0) {
            _savePosition = savePosition;
            _instant = instant;
            _duration = duration;
            _easeFunction = easeFunction;
            _delay = delay;
        }
    }
}
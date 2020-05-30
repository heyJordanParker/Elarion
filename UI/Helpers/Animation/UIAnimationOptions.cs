using Elarion;

namespace Elarion.UI.Helpers.Animation {
    public class UIAnimationOptions {
        private readonly bool _savePosition;
        private readonly bool _instant;
        private readonly Ease? _ease;
        private readonly float _duration;
        private readonly float _delay;

        public bool SavePosition => _savePosition;

        public bool Instant => _instant;

        public Ease Ease {
            get {
                if(!_ease.HasValue) {
                    return Ease.Linear;
                }

                return _ease.Value;
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

        public float Delay => _delay;

        public UIAnimationOptions(bool savePosition = false, bool instant = false, Ease? ease = null,
            float duration = 1, float delay = 0) {
            _savePosition = savePosition;
            _instant = instant;
            _duration = duration;
            _ease = ease;
            _delay = delay;
        }
    }
}
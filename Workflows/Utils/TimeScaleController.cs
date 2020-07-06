using Elarion.Attributes;
using UnityEngine;

namespace Elarion.Workflows.Utils {
    [CreateAssetMenu(menuName = "Utils/TimeScale Controller", order = 51)]
    public class TimeScaleController : ScriptableObject {

        [SerializeField, ReadOnly, Header("Current TimeScale")]
        private float _timeScale = 1f;

        public float TimeScale {
            get => _timeScale;
            private set {
                _timeScale = value;
                Time.timeScale = _timeScale;
            }
        }

        public void SetTimeScale(float value) {
            TimeScale = value;
        }

        public void ResetTimeScale() {
            SetTimeScale(1);
        }

        public void StopTime() {
            SetTimeScale(0);
        }

        public void StartTime() {
            ResetTimeScale();
        }
        
    }
}
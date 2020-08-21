using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Events.Triggers.Simple {
    public class TimeTrigger : SimpleTrigger {
        [SerializeField]
        private FloatReference _timeDelay = new FloatReference(5);
        
        [SerializeField]
        private BoolReference _repeat = new BoolReference(false);
        
        [SerializeField]
        private BoolReference _autoStart = new BoolReference(true);
        
        [SerializeField]
        private BoolReference _useUnscaledTime = new BoolReference(false);

        private float _timeStarted;

        private bool _fired;
        
        private float TimePassed => _useUnscaledTime ? Time.realtimeSinceStartup : Time.timeSinceLevelLoad;
        
        private void OnEnable() {
            if(_autoStart) {
                StartTimer();
            }
        }

        private void Update() {
            if(_fired || !(TimePassed > _timeStarted + _timeDelay)) {
                return;
            }

            _fired = true;
            FireEvent();
            
            if(_repeat) {
                StartTimer();
            }
        }

        public void StartTimer() {
            _timeStarted = TimePassed;
            _fired = false;
        }
    }
}
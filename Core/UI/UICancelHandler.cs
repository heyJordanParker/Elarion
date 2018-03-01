using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UICancelHandler : BaseUIBehaviour {
        
        [SerializeField]
        private UnityEvent _onCancel;

        protected override void Awake() {
            base.Awake();
        }

        public void Cancel() {
            _onCancel.Invoke();
        }
    }
}
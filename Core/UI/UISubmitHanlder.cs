using UnityEngine;
using UnityEngine.Events;

namespace Elarion.UI {
    public class UISubmitHanlder : BaseUIBehaviour {
        [SerializeField]
        private UnityEvent _onSubmit;

        protected override void Awake() {
            base.Awake();
        }

        public void Submit() {
            _onSubmit.Invoke();
        }
    }
}
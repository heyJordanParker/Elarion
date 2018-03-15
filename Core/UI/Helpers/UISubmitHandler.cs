using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    public class UISubmitHandler : BaseUIBehaviour, ISubmitHandler {
        [SerializeField]
        private UnityEvent _onSubmit;
        
        public event Action Submit = () => { };

        public void OnSubmit(BaseEventData eventData) {
            _onSubmit.Invoke();
            Submit();
        }
    }
}
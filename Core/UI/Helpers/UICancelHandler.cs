using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    public class UICancelHandler : BaseUIBehaviour, ICancelHandler {
        [SerializeField]
        private UnityEvent _onCancel;

        public event Action Cancel = () => { };

        public void OnCancel(BaseEventData eventData) {
            _onCancel.Invoke();
            Cancel();
        }
    }
}
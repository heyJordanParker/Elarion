using System;
using Elarion.Common.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Elarion.UI.Helpers {
    /// <summary>
    /// Utility class. Useful for focusing components on click or any other actions.
    /// </summary>
    [UIComponentHelper]
    public class UIOnClickHandler : BaseUIBehaviour, IPointerClickHandler {
        
        [SerializeField]
        private UnityEvent _onClick;

        public event Action Click = () => { };
        
        public void OnPointerClick(PointerEventData eventData) {
            _onClick?.Invoke();

            Click();
        }
    }
}
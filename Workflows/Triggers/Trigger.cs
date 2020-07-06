using Elarion.Attributes;
using Elarion.Workflows.Conditions;
using Elarion.Workflows.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.Triggers {
    public abstract class Trigger<TSavedValue, TUnityEvent, TParameter> : ExtendedBehaviour
        where TSavedValue : SavedValue<TParameter>
        where TUnityEvent : UnityEvent<TParameter> {

        [SerializeField]
        protected TSavedValue trigger;
        [SerializeField]
        protected TUnityEvent action;
        [SerializeField]
        protected bool useCondition;
        [SerializeField, ConditionalVisibility(enableConditions: nameof(useCondition))]
        protected Condition condition;

        private void OnEnable() {
            trigger.Subscribe(OnValueChanged);
        }
        
        private void OnDisable() {
            trigger.Unsubscribe(OnValueChanged);
        }
        
        private void OnValueChanged(TParameter newValue) {
            if(useCondition && (condition == null || !condition.IsSatisfied)) {
                return;
            }
            
            action.Invoke(newValue);
        }
    }
}
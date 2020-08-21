using Elarion.Attributes;
using Elarion.Workflows.Conditions;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.Events.Triggers.Simple {
    public abstract class SimpleTrigger : MonoBehaviour {

        [SerializeField]
        private UnityEvent _triggeredEvent;
        
        [SerializeField]
        protected bool useCondition;
        [SerializeField, ConditionalVisibility(enableConditions: nameof(useCondition))]
        protected Condition condition;

        protected void FireEvent() {
            if(useCondition && (condition == null || !condition.IsSatisfied)) {
                return;
            }
            
            _triggeredEvent.Invoke();
        } 
        
    }
}
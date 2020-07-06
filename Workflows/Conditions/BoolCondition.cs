using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    [CreateAssetMenu(menuName = "Conditions/Bool Condition", order = 36)]
    public class BoolCondition : Condition {
        
        protected enum Behaviors { IsTrue, IsFalse }

        [SerializeField]
        private Behaviors _behavior = Behaviors.IsTrue;
        
        [SerializeField]
        private BoolReference boolValue;

        public BoolReference BoolValue => boolValue;

        public override bool IsSatisfied => _behavior == Behaviors.IsTrue ? BoolValue.Value : !BoolValue.Value;
    }
}
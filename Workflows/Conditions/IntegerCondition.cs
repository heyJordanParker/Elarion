using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    [CreateAssetMenu(menuName = "Conditions/Integer Condition", order = 36)]
    public class IntegerCondition : ComparisonCondition<IntReference, IntReference> {
        public override bool IsSatisfied => IsSatisfiedCompare(leftVariable.Value, behavior, rightVariable.Value);
    }
}
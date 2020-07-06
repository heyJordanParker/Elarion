using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    [CreateAssetMenu(menuName = "Conditions/Float Condition", order = 36)]
    public class FloatCondition : ComparisonCondition<FloatReference, FloatReference> {
        public override bool IsSatisfied => IsSatisfiedCompare(leftVariable.Value, behavior, rightVariable.Value);
    }
}
using System;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    public abstract class ComparisonCondition<TLeft, TRight> : Condition {
        protected enum Operators { Equal, NotEqual, Bigger, Smaller, BiggerOrEqual, SmallerOrEqual }

        [SerializeField]
        protected TLeft leftVariable;
        
        [SerializeField]
        protected Operators behavior = Operators.Equal;
        
        [SerializeField]
        protected TRight rightVariable;

        protected static bool IsSatisfiedCompare(IComparable LeftVariableValue, Operators Behavior,
            IComparable RightVariableValue) {
            switch(Behavior) {
                case Operators.Equal:
                    return LeftVariableValue.CompareTo(RightVariableValue) == 0;
                case Operators.NotEqual:
                    return LeftVariableValue.CompareTo(RightVariableValue) != 0;
                case Operators.Bigger:
                    return LeftVariableValue.CompareTo(RightVariableValue) > 0;
                case Operators.Smaller:
                    return LeftVariableValue.CompareTo(RightVariableValue) < 0;
                case Operators.BiggerOrEqual:
                    return LeftVariableValue.CompareTo(RightVariableValue) >= 0;
                case Operators.SmallerOrEqual:
                    return LeftVariableValue.CompareTo(RightVariableValue) <= 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    [CreateAssetMenu(menuName = "Conditions/Or Condition", order = 36)]
    public class OrCondition : Condition {

        [SerializeField, Reorderable]
        private List<Condition> _conditions = new List<Condition>();

        public List<Condition> Conditions => _conditions;

        public override bool IsSatisfied => _conditions.Any(c => c.IsSatisfied);
    }
}
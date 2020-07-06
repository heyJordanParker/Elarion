using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    [CreateAssetMenu(menuName = "Conditions/And Condition", order = 36)]
    public class AndCondition : Condition {

        [SerializeField, Reorderable]
        private List<Condition> _conditions = new List<Condition>();

        public List<Condition> Conditions => _conditions;

        public override bool IsSatisfied => _conditions.All(c => c.IsSatisfied);
    }
}
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.Workflows.Conditions {
    public abstract class Condition : SavedObject {
        public abstract bool IsSatisfied { get; }

        [InspectorButton]
        public void TestCondition() {
            var isSatisfied = IsSatisfied ? "satisfied" : "not satisfied";
            Debug.Log($"{name} is {isSatisfied}");
        }
    }
}
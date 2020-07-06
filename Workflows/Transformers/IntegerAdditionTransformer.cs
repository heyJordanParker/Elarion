using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Transformers {
    public class IntegerAdditionTransformer : ExtendedBehaviour {
        [Reorderable, SerializeField, PlayModeVisibility(lockInPlayMode: true)]
        private List<IntReference> _integers = new List<IntReference>();

        [SerializeField]
        private IntReference _sum;

        private void OnEnable() {
            foreach(var intReference in _integers) {
                intReference.Subscribe(OnIntChanged);
            }
        }

        private void OnDisable() {
            foreach(var intReference in _integers) {
                intReference.Unsubscribe(OnIntChanged);
            }
        }

        private void OnIntChanged(int newInt) {
            _sum.Value = _integers.Select(reference => reference.Value).Sum();
        }
    }
}
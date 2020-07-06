using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Transformers {
    public class FloatAdditionTransformer : ExtendedBehaviour {
        [Reorderable, SerializeField, PlayModeVisibility(lockInPlayMode: true)]
        private List<FloatReference> _floats = new List<FloatReference>();

        [SerializeField]
        private FloatReference _sum;
        
        private void OnEnable() {
            foreach(var floatReference in _floats) {
                floatReference.Subscribe(OnFloatChanged);
            }
        }

        private void OnDisable() {
            foreach(var floatReference in _floats) {
                floatReference.Unsubscribe(OnFloatChanged);
            }
        }

        private void OnFloatChanged(float newFloat) {
            _sum.Value = _floats.Select(reference => reference.Value).Sum();
        }
    }
}
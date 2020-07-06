using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI.PropertyTweeners.RectTransform {
    public class ScaleTweener : PropertyTweener<Vector3, UnityEngine.RectTransform> {
        
        public ScaleTweener(MonoBehaviour owner) : base(owner) { }

        public override Vector3 CurrentValue {
            get { return Target.localScale; }
            protected set { Target.localScale = value; }
        }
        
        protected override Vector3 UpdateValue(Vector3 startingValue, float progress, Ease ease) {
            return startingValue.EaseTo(TargetValue, progress, ease);
        }

        protected override Vector3 AddValues(Vector3 value1, Vector3 value2) {
            return value1 + value2;
        }
    }
}
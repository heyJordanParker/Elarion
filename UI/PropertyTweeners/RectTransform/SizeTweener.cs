using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI.PropertyTweeners.RectTransform {
    public class SizeTweener : PropertyTweener<Vector2, UnityEngine.RectTransform> {
        
        public SizeTweener(MonoBehaviour owner) : base(owner) { }

        public override Vector2 CurrentValue {
            get { return Target.sizeDelta; }
            protected set { Target.sizeDelta = value; }
        }
        
        protected override Vector2 UpdateValue(Vector2 startingValue, float progress, Ease ease) {
            return startingValue.EaseTo(TargetValue, progress, ease);
        }

        protected override Vector2 AddValues(Vector2 value1, Vector2 value2) {
            return value1 + value2;
        }
    }
}
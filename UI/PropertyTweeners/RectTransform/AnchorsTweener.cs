using Elarion;
using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI.PropertyTweeners.RectTransform {
    public class AnchorsTweener : PropertyTweener<Vector4, UnityEngine.RectTransform> {
        
        public AnchorsTweener(MonoBehaviour owner) : base(owner) { }

        public override Vector4 CurrentValue {
            get {
                return new Vector4(Target.anchorMin.x, Target.anchorMin.y, Target.anchorMax.x, Target.anchorMax.y);
            }
            protected set {
                Target.anchorMin = new Vector2(value.x, value.y);
                Target.anchorMax = new Vector2(value.z, value.w);
            }
        }
        
        protected override Vector4 UpdateValue(Vector4 startingValue, float progress, Ease ease) {
            return startingValue.EaseTo(TargetValue, progress, ease);
        }

        protected override Vector4 AddValues(Vector4 value1, Vector4 value2) {
            return value1 + value2;
        }
    }
}
using Elarion.Extensions;
using UnityEngine;

namespace Elarion.Utility.PropertyTweeners.CanvasGroup {
    public class AlphaTweener : PropertyTweener<float, UnityEngine.CanvasGroup> {
        
        public AlphaTweener(MonoBehaviour owner) : base(owner) { }

        public override float CurrentValue {
            get { return Target.alpha;}
            protected set { Target.alpha = value; }
        }
        
        protected override float UpdateValue(float startingValue, float progress, Ease ease) {
            return startingValue.EaseTo(TargetValue, progress, ease);
        }

        protected override float AddValues(float value1, float value2) {
            return value1 + value2;
        }
    }
}
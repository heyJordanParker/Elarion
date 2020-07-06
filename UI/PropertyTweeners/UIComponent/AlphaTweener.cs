using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI.PropertyTweeners.UIComponent {
    // TODO make this a GameObject property tweener (make all the tweeners work with a GO)
    // TODO automatically cache the proper component needed for tweening in the constructor
    // TODO try getting a canvasgroup and if that's not present, a Graphic to perform the tweening on
    // think of an interface to simplify the couple of proxy methods that have 6-7 tweener calls in the UIAnimator
    
    
    // TODO create a separate component that handles the alpha; get tweeners to work with gameobjects
    public class AlphaTweener : PropertyTweener<float, UI.UIComponent> {
        
        public AlphaTweener(MonoBehaviour owner) : base(owner) { }

        public override float CurrentValue {
            get { return Target.Alpha;}
            protected set { Target.Alpha = value; }
        }
        
        protected override float UpdateValue(float startingValue, float progress, Ease ease) {
            return startingValue.EaseTo(TargetValue, progress, ease);
        }

        protected override float AddValues(float value1, float value2) {
            return value1 + value2;
        }
    }
}
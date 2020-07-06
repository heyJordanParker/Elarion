using Elarion.Workflows.Variables.References;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Workflows.Variables.Setters {
    [ExecuteInEditMode]
    public class SliderSetterPercentage : MonoBehaviour {
        public Slider slider;

        public FloatReference currentValue = new FloatReference(0.5f);
        public FloatReference maxValue = new FloatReference(1);
        
        private void Update() {
            if(slider != null) {
                slider.value = Mathf.Clamp01(currentValue / maxValue);
            }
        }
    }
}
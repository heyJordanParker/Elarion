using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Workflows.Variables.Setters {
    [ExecuteInEditMode]
    public class SliderSetter : MonoBehaviour {
        public Slider slider;
        public SavedFloat variable;

        private void Update() {
            if(slider != null && variable != null)
                slider.value = variable.Value;
        }
    }
}
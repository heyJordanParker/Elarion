using Elarion.Saved.Variables.References;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Saved.Variables.Setters {
    /// <summary>
    /// Sets an Image component's fill amount to represent how far Variable is
    /// between Min and Max.
    /// </summary>
    public class ImageFillSetter : MonoBehaviour {
        [Tooltip("Value to use as the current ")]
        public FloatReference variable;

        [Tooltip("Min value that Variable to have no fill on Image.")]
        public FloatReference min;

        [Tooltip("Max value that Variable can be to fill Image.")]
        public FloatReference max;

        [Tooltip("Image to set the fill amount on.")]
        public Image image;

        private void Update() {
            image.fillAmount = Mathf.Clamp01(
                Mathf.InverseLerp(min, max, variable));
        }
    }
}
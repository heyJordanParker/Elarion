using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI.Utils {
    [RequireComponent(typeof(Graphic))]
    public class ColorCopier : MonoBehaviour {

        public Graphic target;

        private Graphic _graphic;

        private void Awake() {
            _graphic = GetComponent<Graphic>();
        }

        private void LateUpdate() {
            _graphic.color = target.color;
        }
    }
}
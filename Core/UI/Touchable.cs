using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    /// <summary>
    /// Use this instead of a graphic object to create an invisible button without the overhead of a transparent layer
    /// </summary>
    public class Touchable : Text {
        protected override void Awake() {
            base.Awake();
        }
    }
}
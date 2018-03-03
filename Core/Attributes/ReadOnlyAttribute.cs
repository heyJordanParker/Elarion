using UnityEngine;

namespace Elarion.Attributes {
    public class ReadOnlyAttribute : PropertyAttribute {
        public readonly bool showOnlyWhenPlaying;

        public ReadOnlyAttribute(bool showOnlyWhenPlaying = false) {
            this.showOnlyWhenPlaying = showOnlyWhenPlaying;
        }
        
    }
}

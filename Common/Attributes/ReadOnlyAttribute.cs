using UnityEngine;

namespace Elarion.Common.Attributes {
    public class ReadOnlyAttribute : PropertyAttribute {
        public readonly bool showOnlyWhenPlaying;

        public ReadOnlyAttribute(bool showOnlyWhenPlaying = false) {
            this.showOnlyWhenPlaying = showOnlyWhenPlaying;
        }
    }
}

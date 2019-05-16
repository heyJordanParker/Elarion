using Elarion.Common.Attributes;
using UnityEngine.UI;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    public class UIIgnoreLayout : LayoutElement {
        public override bool ignoreLayout {
            get { return true; }
            set {  }
        }
    }
}
using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Selectable))]
    public class UINavigation : BaseUIBehaviour {

        [Serializable, Flags]
        private enum NavigationMode {
            None = 0,
            Horizontal = 1,
            Vertical = 2,
            Auto = Vertical | Horizontal,
        }

        [SerializeField]
        private NavigationMode _navigationMode = NavigationMode.Auto;

        [ConditionalVisibility(enableConditions: "_navigationMode == NavigationMode.Auto || NavigationMode.Vertical")]
        public Selectable selectOnUp;
        [ConditionalVisibility(enableConditions: "_navigationMode == NavigationMode.Auto || NavigationMode.Vertical")]
        public Selectable selectOnDown;
        [ConditionalVisibility(enableConditions: "_navigationMode == NavigationMode.Auto || NavigationMode.Horizontal")]
        public Selectable selectOnLeft;
        [ConditionalVisibility(enableConditions: "_navigationMode == NavigationMode.Auto || NavigationMode.Horizontal")]
        public Selectable selectOnRight;

        [SerializeField, HideInInspector]
        private Selectable _selectable;
        
        public Selectable Selectable {
            get {
                if(_selectable == null) {
                    _selectable = GetComponent<Selectable>();
                }

                return _selectable;
            }
        }

        private Navigation _navigation;

        protected override void OnEnable() {
            Selectable.navigation = new Navigation() {
                    mode = (Navigation.Mode) _navigationMode,
                    selectOnUp = selectOnUp,
                    selectOnDown = selectOnDown,
                    selectOnLeft = selectOnLeft,
                    selectOnRight = selectOnRight
                };
        }
    }
}
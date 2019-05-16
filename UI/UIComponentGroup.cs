using System.Collections.Generic;
using Elarion.Common.Attributes;
using UnityEngine;

namespace Elarion.UI {
    /// <summary>
    /// Utility class that ensures that only a single group component is active at any one time.
    /// </summary>
    [UIComponentHelper]
    [RequireComponent(typeof(UIComponent))]
    public class UIComponentGroup : BaseUIBehaviour {

        private class GroupData {
            public UIComponent CurrentOpenComponent { get; set; }
            public UIComponent PreviousOpenComponent { get; set; }
        }
        
        [SerializeField]
        [Tooltip("Group ID. No two components that share a group ID can be active at the same time.")]
        private int _groupId = 0;

        [SerializeField]
        [Tooltip("Move to top of render queue to show animations properly.")]
        private bool _setAsLastChildOnOpen;

        [SerializeField, HideInInspector]
        private UIComponent _component;
        
        public int GroupId => _groupId;

        public UIComponent Component => _component ? _component : (_component = GetComponent<UIComponent>());

        public UIComponent CurrentOpenComponent {
            get => GetGroup(GroupId).CurrentOpenComponent;
            private set => GetGroup(GroupId).CurrentOpenComponent = value;
        }

        private UIComponent PreviousOpenComponent {
            get => GetGroup(GroupId).PreviousOpenComponent;
            set => GetGroup(GroupId).PreviousOpenComponent = value;
        }

        protected override void Awake() {
            _component = GetComponent<UIComponent>();
        }

        protected override void OnEnable() {
            Component.BeforeOpenEvent.AddListener(BeforeOpen);
            Component.AfterOpenEvent.AddListener(AfterOpen);
        }

        protected override void OnDisable() {
            Component.BeforeOpenEvent.RemoveListener(BeforeOpen);
            Component.AfterOpenEvent.RemoveListener(AfterOpen);
        }

        protected void BeforeOpen(bool skipAnimation) {
            if(CurrentOpenComponent != null && CurrentOpenComponent != Component) {
                if(HasAnimation(CurrentOpenComponent, UIAnimationType.OnClose)) {
                    // close and let both animations play at the same time
                    CurrentOpenComponent.Close();
                } else {
                    // close after this finished animating
                    PreviousOpenComponent = CurrentOpenComponent;
                }
            }

            CurrentOpenComponent = Component;

            if(_setAsLastChildOnOpen) {
                transform.SetAsLastSibling();
            }
        }

        protected void AfterOpen() {
            if(PreviousOpenComponent != null) {
                PreviousOpenComponent.Close();
                PreviousOpenComponent = null;
            }
        }

        private bool HasAnimation(UIComponent component, UIAnimationType animation) {
            return component.HasAnimator && component.Animator.GetAnimation(animation) != null;
        }

        private static GroupData GetGroup(int groupId) {
            if(!ComponentGroups.ContainsKey(groupId)) {
                ComponentGroups.Add(groupId, new GroupData());
            }

            return ComponentGroups[groupId];
        }

        private static Dictionary<int, GroupData> ComponentGroups { get; } = new Dictionary<int, GroupData>();
    }
}
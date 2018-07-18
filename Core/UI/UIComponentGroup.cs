using System.Collections.Generic;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO use unity events to open/close child components (instead of the parent doing that for them); e.g. register the events in the code
    
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

        [SerializeField, HideInInspector]
        private UIComponent _component;
        
        public int GroupId => _groupId;

        public UIComponent Component => _component ? _component : (_component = GetComponent<UIComponent>());

        private UIComponent CurrentOpenComponent {
            get {
                if(!ComponentGroups.ContainsKey(GroupId)) {
                    ComponentGroups.Add(GroupId, new GroupData());
                }

                return ComponentGroups[GroupId].CurrentOpenComponent;
            }
            set {
                if(!ComponentGroups.ContainsKey(GroupId)) {
                    ComponentGroups.Add(GroupId, new GroupData());
                }
                
                ComponentGroups[GroupId].CurrentOpenComponent = value;
            }
        }

        private UIComponent PreviousOpenComponent {
            get {
                if(!ComponentGroups.ContainsKey(GroupId)) {
                    ComponentGroups.Add(GroupId, new GroupData());
                }

                return ComponentGroups[GroupId].PreviousOpenComponent;
            }
            set {
                if(!ComponentGroups.ContainsKey(GroupId)) {
                    ComponentGroups.Add(GroupId, new GroupData());
                }
                
                ComponentGroups[GroupId].PreviousOpenComponent = value;
            }
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

            if(HasAnimation(CurrentOpenComponent, UIAnimationType.OnOpen)) {
                // set to the top of the render queue if it'll animate
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

        private static Dictionary<int, GroupData> ComponentGroups { get; } = new Dictionary<int, GroupData>();
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Selectable))]
    public class UINavigationElement : UIBehaviour {

        public Selectable previousSelectable;
        public Selectable nextSelectable;

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

        protected override void OnEnable() {
            NavigationElementsCache.Add(gameObject, this);
        }

        protected override void OnDisable() {
            NavigationElementsCache.Remove(gameObject);
        }

        private static Dictionary<GameObject, UINavigationElement> _navigationElementsCache;

        public static Dictionary<GameObject, UINavigationElement> NavigationElementsCache {
            get {
                if(_navigationElementsCache == null) {
                    _navigationElementsCache = new Dictionary<GameObject, UINavigationElement>();
                }

                return _navigationElementsCache;
            }
        }

        protected override void OnValidate() {
            if(previousSelectable != null && nextSelectable != null) {
                return;
            }
            
            var navigationElements = FindObjectsOfType<UINavigationElement>();
            
            if(previousSelectable == null) {
                var result = navigationElements.SingleOrDefault(navElement => navElement.nextSelectable == Selectable);

                if(result != null) {
                    previousSelectable = result.Selectable;
                }
            }
            
            if(nextSelectable == null) {
                var result = navigationElements.SingleOrDefault(navElement => navElement.previousSelectable == Selectable);

                if(result != null) {
                    nextSelectable = result.Selectable;
                }
            }
        }
    }
}
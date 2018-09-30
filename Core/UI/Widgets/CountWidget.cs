using System.Collections;
using Elarion.Attributes;
using Elarion.Coroutines;
using Elarion.Extensions;
using Elarion.Saved.Arrays;
using TMPro;
using UnityEngine;

namespace Elarion.UI.Widgets {
    public abstract class CountWidget<TSavedList, TSavedListItem> : MonoBehaviour where TSavedList : SavedList<TSavedListItem> {
        protected const string DefaultText = "{0}";

        public TSavedList list;
        
        public TMP_Text text;

        [SerializeField]
        [Tooltip("Should the text be formatted?")]
        protected bool formatText;
        
        [SerializeField]
        [ConditionalVisibility("formatText")]
        [Tooltip("Base text before formatting.")]
        protected string baseText = "{0} Items";
        
        private ECoroutine _updateCoroutine;

        private void OnEnable() {
            list.Subscribe(OnListChanged);
        }
        
        private void OnDisable() {
            list.Unsubscribe(OnListChanged);
        }

        private IEnumerator UpdateVisibility() {
            yield return EWait.ForEndOfFrame;
            
            text.SetText(string.Format(baseText, list.Count));

            _updateCoroutine = null;
        }

        private void OnListChanged(SavedList<TSavedListItem> savedListItems) {
            if(_updateCoroutine == null) {
                _updateCoroutine = this.CreateCoroutine(UpdateVisibility());
            }
        }

        private void OnValidate() {
            if(!baseText.Contains("{0}")) {
                baseText += "{0}";
            }
        }
    }
}
using Elarion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI.TMP {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDuplicator : BasicUIElement {

        private TMP_Text _tmpText;

        public Text targetText;

        protected override void Awake() {
            base.Awake();
            _tmpText = GetComponent<TMP_Text>();
        }

        public void OnEnable() {
            if(targetText != null)
                return;

            Debug.LogWarning("The TextDuplicator component requires a text field to duplicate. Disabling.", this);
            this.SetActive(false);
        }

        public void Update() {
            _tmpText.text = targetText.text;
        }
    }
}
using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformWatcher : TransformWatcher {
        [SerializeField, GetComponent]
        private RectTransform _rectTransform;
        
        [SerializeField]
        private Vector3Reference _anchoredPosition;

        [SerializeField]
        private Vector2Reference _sizeDelta;

        public Vector3Reference AnchoredPosition => _anchoredPosition;
        public Vector2Reference SizeDelta => _sizeDelta;

        protected override void Update() {
            base.Update();

            _anchoredPosition.Value = _rectTransform.anchoredPosition;
            _sizeDelta.Value = _rectTransform.sizeDelta;
        }
    }
}
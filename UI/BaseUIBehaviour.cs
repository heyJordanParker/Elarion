using Elarion.Common;
using UnityEngine;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseUIBehaviour : ExtendedBehaviour {
        public RectTransform Transform {
            get {
                if(_transform == null) {
                    _transform = transform as RectTransform;
                }

                return _transform;
            }
        }

        private RectTransform _transform;

        /// <summary>
        ///   <para>See MonoBehaviour.Awake.</para>
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnEnable.</para>
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        ///   <para>See MonoBehaviour.Start.</para>
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDestroy.</para>
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        ///   <para>This callback is called if an associated RectTransform has its dimensions changed. The call is also made to all child rect transforms, even if the child transform itself doesn't change - as it could have, depending on its anchoring.</para>
        /// </summary>
        protected virtual void OnRectTransformDimensionsChange() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnBeforeTransformParentChanged.</para>
        /// </summary>
        protected virtual void OnBeforeTransformParentChanged() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnRectTransformParentChanged.</para>
        /// </summary>
        protected virtual void OnTransformParentChanged() { }

        /// <summary>
        ///   <para>See UI.LayoutGroup.OnDidApplyAnimationProperties.</para>
        /// </summary>
        protected virtual void OnDidApplyAnimationProperties() { }

        /// <summary>
        ///   <para>See MonoBehaviour.OnCanvasGroupChanged.</para>
        /// </summary>
        protected virtual void OnCanvasGroupChanged() { }

        /// <summary>
        ///   <para>Called when the state of the parent Canvas is changed.</para>
        /// </summary>
        protected virtual void OnCanvasHierarchyChanged() { }
    }
}
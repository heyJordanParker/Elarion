using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO update parent (and everything else) when the hierarchy changes, but allow this to be static (either an advanced option or just the static GameObject flag)
        
        [SerializeField]
        private bool _interactable = true;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public override float Alpha {
            get => CanvasGroup.alpha;
            set => CanvasGroup.alpha = Mathf.Clamp01(value);
        }

        protected override bool InteractableSelf {
            get => _interactable;
            set => _interactable = value;
        }
        
        protected Canvas Canvas => _canvas;

        public override Behaviour Renderer {
            get {
                if(_canvas == null) {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }

        public CanvasGroup CanvasGroup {
            get {
                if(_canvasGroup == null) {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        protected override void OnStateChanged(States currentState, States previousState) {
            base.OnStateChanged(currentState, previousState);

            // disable disabled child's interaction to simplify navigation events
            CanvasGroup.interactable = IsRendering && !IsDisabled;

            CanvasGroup.blocksRaycasts = IsInteractable;
        }
    }
}
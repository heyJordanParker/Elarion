using System.Linq;
using Elarion.Extensions;
using Elarion.Utility;
using TypeSafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using yaSingleton;

namespace Elarion.UI {
    
    [CreateAssetMenu(fileName = "UI Manager.asset", menuName = "Manager/UI Manager", order = 81)]
    public class UIManager : Singleton<UIManager> {
        
        // TODO ui camera configuration - autoCreate, culling mask, hide, background, etc
        
        public bool enableTabNavigation = true;

        private BaseEventData _baseEventData;

        protected override void Initialize() {
            base.Initialize();
            // This is necessary for blur effects - the shader can't work with the main render texture
            var uiCamera = UIHelper.CreateUICamera("UI Root Camera");
            uiCamera.hideFlags = HideFlags.HideAndDontSave;
        }

        // Update after the event system; Ensure that 
        public override void OnLateUpdate() {
            base.OnLateUpdate();
            
            if(!EventSystem || !EventSystem.isFocused) {
                // process the events just once and only if focused
                return;
            }

            HandleTabNavigation();
        }

        private void HandleTabNavigation() {
            if(!enableTabNavigation ||
               !EventSystem ||
               !EventSystem.sendNavigationEvents ||
               !Input.GetKeyDown(KeyCode.Tab)) {
                return;
            }

            var startingTransform = EventSystem.currentSelectedGameObject
                ? EventSystem.currentSelectedGameObject.transform as RectTransform
                : null;
            var selectable = EventSystem.currentSelectedGameObject
                ? EventSystem.currentSelectedGameObject.GetComponent<Selectable>()
                : null;
            Selectable nextSelectable = null;

            if(selectable && selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None) {
                var reverseNavigationDirection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                bool horizontalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Horizontal);
                bool verticalNavigation = selectable.navigation.mode.HasFlag(Navigation.Mode.Vertical);

                if(verticalNavigation) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnUp
                        : selectable.navigation.selectOnDown;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? startingTransform.FindCloseSelectableOnUp()
                            : startingTransform.FindCloseSelectableOnDown();
                    }
                }

                if(horizontalNavigation && nextSelectable == null) {
                    nextSelectable = reverseNavigationDirection
                        ? selectable.navigation.selectOnLeft
                        : selectable.navigation.selectOnRight;

                    if(nextSelectable == null) {
                        nextSelectable = reverseNavigationDirection
                            ? startingTransform.FindCloseSelectableOnLeft()
                            : startingTransform.FindCloseSelectableOnRight();
                    }
                }
            }

            if(!selectable && startingTransform) {
                nextSelectable = startingTransform.gameObject.GetSelectableChildren().FirstOrDefault(child =>
                    child.IsInteractable() && child.navigation.mode != Navigation.Mode.None);
            }

            if(nextSelectable == null || !nextSelectable.IsInteractable() ||
               nextSelectable.navigation.mode == Navigation.Mode.None) return;

            Select(nextSelectable);
        }

        protected bool Select(Selectable selectable) {
            if(!EventSystem ||
               EventSystem.alreadySelecting ||
               !selectable) {
                return false;
            }
            
            ExecuteEvents.Execute(selectable.gameObject, null, ExecuteEvents.selectHandler);
            return true;
        }

        private static EventSystem EventSystem => EventSystem.current;
    }
}
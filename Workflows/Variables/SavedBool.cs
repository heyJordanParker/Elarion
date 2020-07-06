using UnityEngine;

namespace Elarion.Workflows.Variables {
    [CreateAssetMenu(menuName = "Saved Values/Bool", order = 32)]
    public class SavedBool : SavedValue<bool> {

        #region Unity Actions
        public void Toggle() {
            Value = !Value;
        }

        public void SetToTrue() {
            Value = true;
        }
        
        public void SetToFalse() {
            Value = true;
        }
        #endregion
    }
}
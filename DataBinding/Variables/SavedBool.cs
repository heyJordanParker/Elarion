using UnityEngine;

namespace Elarion.DataBinding.Variables {
    [CreateAssetMenu(menuName = "Saved/Bool", order = 32)]
    public class SavedBool : SavedVariable<bool> {

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
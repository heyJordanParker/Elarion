using UnityEngine;

namespace Elarion.Workflows.Variables {
    [CreateAssetMenu(menuName = "Saved Values/Int", order = 32)]
    public class SavedInt : SavedValue<int> {

        public void Increment() {
            ++Value;
        }

        public void Decrement() {
            --Value;
        }
        
    }
}
using UnityEngine;

namespace Elarion.Saved.Variables {
    [CreateAssetMenu(menuName = "Saved/Int", order = 32)]
    public class SavedInt : SavedVariable<int> {

        public void Increment() {
            ++Value;
        }

        public void Decrement() {
            --Value;
        }
        
    }
}
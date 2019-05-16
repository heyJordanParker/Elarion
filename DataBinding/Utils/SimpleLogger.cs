using UnityEngine;

namespace Elarion.DataBinding.Utils {
    /// <summary>
    /// Simple logger. Use with Unity Events.
    /// </summary>
    [CreateAssetMenu(menuName = "Utils/Simple Logger", order = 51)]
    public class SimpleLogger : ScriptableObject {

        public void Log(string message) {
            Debug.Log(message);
        }
        
    }
}
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion {
	
	/// <summary>
	/// Class used to store scene specific configurations
	/// </summary>
	public class SceneConfig : MonoBehaviour {

		public virtual void ConfigureScene(GameState gameState) { }

		public static SceneConfig Get() {
			var props = FindObjectsOfType(typeof(SceneConfig));
			if(props.Length == 0) return null;
			if(props.Length > 1) Debug.LogError("Finding scene properties resulted more than one entry");
			return (SceneConfig) props[0];
		}

	}

}
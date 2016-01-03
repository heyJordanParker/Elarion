using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Elarion.Editor {
	public static class Utils {
		public static AddToBuildSettingsResult AddSceneToBuildSettings(string newScenePath) {
			if(Path.GetExtension(newScenePath) != ".unity") {
				return AddToBuildSettingsResult.InvalidScene;
			}
			var currentScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			if(currentScenes.Any(scene => scene.path.Equals(newScenePath))) {
				return AddToBuildSettingsResult.DuplicateScene;
			}
			currentScenes.Add(new EditorBuildSettingsScene(newScenePath, true));
			EditorBuildSettings.scenes = currentScenes.ToArray();
			return AddToBuildSettingsResult.Successful;
		}

	}

}
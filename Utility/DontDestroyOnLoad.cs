namespace Elarion {

	public class DontDestroyOnLoad : ExtendedBehaviour {

		protected override void Initialize() {
			base.Initialize();
			DontDestroyOnLoad(gameObject);
		}

	}

}
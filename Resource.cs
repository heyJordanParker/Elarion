using System;

namespace Elarion {

	/// <summary>
	/// Base class for info files.
	/// Info files should be used for storing information.
	/// Info files should use only serializable fields, thus qualifying them for unity serialization.
	/// </summary>
	[Serializable]
	public abstract class Resource : ExtendedBehaviour { }

}
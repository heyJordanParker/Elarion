using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elarion.Managers {
	interface ILoading {

		//use to define loading coroutine which would be automatically called on every object while the scene loads
		//loadorder ? or not relative ?

		//loading manager which when triggered triggers some events ( pre load ? ) than broadcasts the load event and finally does some final processing ( post load )
		IEnumerator Load();

	}
}

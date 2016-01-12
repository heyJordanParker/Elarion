﻿using UnityEngine;

namespace Elarion.EventSystem {
	public class EventArguments {
		private readonly GameObject _sender;

		public EventArguments(GameObject sender) {
			_sender = sender;
		}

		public GameObject Sender { get { return _sender; } }
	}
}
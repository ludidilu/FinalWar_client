using UnityEngine;
using System.Collections;

namespace xy3d.tstd.lib.superFunction{

	public struct SuperEvent {

		public GameObject target;
		public string eventName;
		public object[] data;
		public int index;

		public SuperEvent(string _eventName){

			eventName = _eventName;
			index = 0;
			data = null;
			target = null;
		}
	}
}
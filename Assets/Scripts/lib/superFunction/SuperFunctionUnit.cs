using UnityEngine;
using System.Collections;
using System;

namespace xy3d.tstd.lib.superFunction{

	public class SuperFunctionUnit : SuperFunctionUnitBase{

		public Action<SuperEvent> callBack;

		public SuperFunctionUnit(GameObject _target,string _eventName,Action<SuperEvent> _callBack,int _index){

			target = _target;
			eventName = _eventName;
			callBack = _callBack;
			index = _index;
		}
	}
}
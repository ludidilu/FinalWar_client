using UnityEngine;
using System.Collections;
using System;

namespace superFunction{

	public class SuperFunctionUnit : SuperFunctionUnitBase{

		public SuperFunction.SuperFunctionCallBack callBack;

		public SuperFunctionUnit(GameObject _target,string _eventName,SuperFunction.SuperFunctionCallBack _callBack,int _index,bool _isOnce){

			target = _target;
			eventName = _eventName;
			callBack = _callBack;
			index = _index;
			isOnce = _isOnce;
		}
	}
}
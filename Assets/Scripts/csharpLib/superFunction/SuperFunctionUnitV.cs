using UnityEngine;
using System.Collections;
using System;

namespace superFunction{
	
	public class SuperFunctionUnitV<T> : SuperFunctionUnitBase where T : struct{
		
		public SuperFunction.SuperFunctionCallBackV<T> callBack;
		
		public SuperFunctionUnitV(GameObject _target,string _eventName,SuperFunction.SuperFunctionCallBackV<T> _callBack,int _index,bool _isOnce){
			
			target = _target;
			eventName = _eventName;
			callBack = _callBack;
			index = _index;
			isOnce = _isOnce;
		}
	}
}
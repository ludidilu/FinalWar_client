using UnityEngine;
using System.Collections;
using System;

namespace xy3d.tstd.lib.superFunction{
	
	public class SuperFunctionUnitV<T> : SuperFunctionUnitBase where T : struct{
		
		public SuperFunction.callbackV<T> callBack;
		
		public SuperFunctionUnitV(GameObject _target,string _eventName,SuperFunction.callbackV<T> _callBack,int _index){
			
			callBack = _callBack;
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using xy3d.tstd.lib.superFunction;

namespace xy3d.tstd.lib.superFunction{

	public class SuperFunction{

		private static SuperFunction _Instance;

		public static SuperFunction Instance{

			get{

				if(_Instance == null){

					_Instance = new SuperFunction();
				}

				return _Instance;
			}
		}

		public delegate void callbackV<T>(SuperEvent e,ref T v)where T : struct;

		private Dictionary<int,SuperFunctionUnitBase> dic;
		private Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnitBase>>> dic2;

		private int index = 0;

		public SuperFunction(){

			dic = new Dictionary<int, SuperFunctionUnitBase>();
			dic2 = new Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnitBase>>>();
		}

		public int AddEventListener(GameObject _target,string _eventName,Action<SuperEvent> _callBack){

			int result = GetIndex();

			SuperFunctionUnit unit = new SuperFunctionUnit(_target,_eventName,_callBack,result);

			dic.Add(result,unit);

			Dictionary<string,List<SuperFunctionUnitBase>> tmpDic;

			if(dic2.ContainsKey(_target)){

				tmpDic = dic2[_target];

			}else{

				_target.AddComponent<SuperFunctionControl>();

				tmpDic = new Dictionary<string,List<SuperFunctionUnitBase>>();

				dic2.Add(_target,tmpDic);
			}

			List<SuperFunctionUnitBase> tmpList;

			if(tmpDic.ContainsKey(_eventName)){

				tmpList = tmpDic[_eventName];

			}else{

				tmpList = new List<SuperFunctionUnitBase>();

				tmpDic.Add(_eventName,tmpList);
			}

			tmpList.Add(unit);

			return result;
		}

		public int AddEventListener<T>(GameObject _target,string _eventName,callbackV<T> _callBack) where T : struct{
			
			int result = GetIndex();
			
			SuperFunctionUnitV<T> unit = new SuperFunctionUnitV<T>(_target,_eventName,_callBack,result);
			
			dic.Add(result,unit);
			
			Dictionary<string,List<SuperFunctionUnitBase>> tmpDic;
			
			if(dic2.ContainsKey(_target)){
				
				tmpDic = dic2[_target];
				
			}else{
				
				_target.AddComponent<SuperFunctionControl>();
				
				tmpDic = new Dictionary<string,List<SuperFunctionUnitBase>>();
				
				dic2.Add(_target,tmpDic);
			}
			
			List<SuperFunctionUnitBase> tmpList;
			
			if(tmpDic.ContainsKey(_eventName)){
				
				tmpList = tmpDic[_eventName];
				
			}else{
				
				tmpList = new List<SuperFunctionUnitBase>();
				
				tmpDic.Add(_eventName,tmpList);
			}
			
			tmpList.Add(unit);
			
			return result;
		}

		public void RemoveEventListener(int _index){

			if(dic.ContainsKey(_index)){

				SuperFunctionUnitBase unit = dic[_index];

				dic.Remove(_index);

				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[unit.target];

				List<SuperFunctionUnitBase> tmpList = tmpDic[unit.eventName];

				tmpList.Remove(unit);

				if(tmpList.Count == 0){

					tmpDic.Remove(unit.eventName);

					if(tmpDic.Count == 0){

						DestroyControl(unit.target);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target){

			if(dic2.ContainsKey(_target)){

				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				DestroyControl(_target);

				Dictionary<string,List<SuperFunctionUnitBase>>.ValueCollection.Enumerator enumerator = tmpDic.Values.GetEnumerator();

				while(enumerator.MoveNext()){

					List<SuperFunctionUnitBase> tmpList = enumerator.Current;

					for(int i = 0 ; i < tmpList.Count ; i++){

						dic.Remove(tmpList[i].index);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target,string _eventName){

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnitBase> list = tmpDic[_eventName];

					for(int i = 0 ; i < list.Count ; i++){

						dic.Remove(list[i].index);
					}

					tmpDic.Remove(_eventName);

					if(tmpDic.Count == 0){

						DestroyControl(_target);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target,string _eventName,Action<SuperEvent> _callBack){

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnitBase> list = tmpDic[_eventName];

					for(int i = 0 ; i < list.Count ; i++){

						if(list[i] is SuperFunctionUnit){

							SuperFunctionUnit unit = list[i] as SuperFunctionUnit;

							if(unit.callBack == _callBack){

								dic.Remove(unit.index);

								list.RemoveAt(i);

								break;
							}
						}
					}

					if(list.Count == 0){

						tmpDic.Remove(_eventName);

						if(tmpDic.Count == 0){

							DestroyControl(_target);
						}
					}
				}
			}
		}

		public void RemoveEventListener<T>(GameObject _target,string _eventName,callbackV<T> _callBack)where T : struct{

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnitBase> list = tmpDic[_eventName];
					
					for(int i = 0 ; i < list.Count ; i++){
						
						if(list[i] is SuperFunctionUnitV<T>){
							
							SuperFunctionUnitV<T> unit = list[i] as SuperFunctionUnitV<T>;
							
							if(unit.callBack == _callBack){
								
								dic.Remove(unit.index);
								
								list.RemoveAt(i);
								
								break;
							}
						}
					}
					
					if(list.Count == 0){
						
						tmpDic.Remove(_eventName);
						
						if(tmpDic.Count == 0){
							
							DestroyControl(_target);
						}
					}
				}
			}
		}

		public void DispatchEvent(GameObject _target,SuperEvent _event){

			if (dic2.ContainsKey (_target)) {
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_event.eventName)){

					List<SuperFunctionUnitBase> tmpList = tmpDic[_event.eventName];
					
					SuperEvent[] eventList = null;

					SuperFunctionUnit[] unitList = null;

					int num = 0;

					for(int i = 0 ; i < tmpList.Count ; i++){

						if(tmpList[i] is SuperFunctionUnit){

							SuperFunctionUnit unit = tmpList[i] as SuperFunctionUnit;

							SuperEvent tmpEvent = new SuperEvent(_event.eventName);

							tmpEvent.target = _target;

							tmpEvent.data = _event.data;

							tmpEvent.index = unit.index;

							if(eventList == null){

								eventList = new SuperEvent[tmpList.Count];

								unitList = new SuperFunctionUnit[tmpList.Count];
							}

							eventList[num] = tmpEvent;

							unitList[num] = unit;

							num++;
						}
					}

					if(eventList != null){

						for(int i = 0 ; i < num ; i++){

							unitList[i].callBack(eventList[i]);
						}
					}
				}
			}
		}

		public void DispatchEvent<T>(GameObject _target,SuperEvent _event,ref T _v) where T : struct{
			
			if (dic2.ContainsKey (_target)) {
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_event.eventName)){
					
					List<SuperFunctionUnitBase> tmpList = tmpDic[_event.eventName];
					
					SuperEvent[] eventList = null;
					
					SuperFunctionUnitV<T>[] unitList = null;

					int num = 0;
					
					for(int i = 0 ; i < tmpList.Count ; i++){

						if(tmpList[i] is SuperFunctionUnitV<T>){
						
							SuperFunctionUnitV<T> unit = tmpList[i] as SuperFunctionUnitV<T>;
							
							SuperEvent tmpEvent = new SuperEvent(_event.eventName);
							
							tmpEvent.target = _target;
							
							tmpEvent.data = _event.data;
							
							tmpEvent.index = unit.index;

							if(eventList == null){

								eventList = new SuperEvent[tmpList.Count];

								unitList = new SuperFunctionUnitV<T>[tmpList.Count];
							}

							eventList[num] = tmpEvent;

							unitList[num] = unit;

							num++;
						}
					}

					if(eventList != null){
					
						for(int i = 0 ; i < num ; i++){
							
							unitList[i].callBack(eventList[i],ref _v);
						}
					}
				}
			}
		}

		public void DestroyGameObject(GameObject _target){

			if(dic2.ContainsKey(_target)){

				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				dic2.Remove(_target);

				Dictionary<string,List<SuperFunctionUnitBase>>.ValueCollection.Enumerator enumerator = tmpDic.Values.GetEnumerator();
				
				while(enumerator.MoveNext()){
					
					List<SuperFunctionUnitBase> tmpList = enumerator.Current;
					
					for(int i = 0 ; i < tmpList.Count ; i++){
						
						dic.Remove(tmpList[i].index);
					}
				}
			}
		}

		private void DestroyControl(GameObject _target){

			SuperFunctionControl[] controls = _target.GetComponents<SuperFunctionControl>();

			for(int i = 0 ; i < controls.Length ; i++){
			
				SuperFunctionControl control = controls[i];

				if(!control.isDestroy){

					control.isDestroy = true;
					
					GameObject.Destroy(control);
				}
			}

			dic2.Remove(_target);
		}

		private int GetIndex(){

			index++;

			int result = index;

			return result;
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using superFunction;

namespace superFunction{

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

		public delegate void SuperFunctionCallBackV<T>(int _index,ref T v,params object[] _datas)where T : struct;

		public delegate void SuperFunctionCallBack(int _index,params object[] _datas);

		private Dictionary<int,SuperFunctionUnitBase> dic;
		private Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnitBase>>> dic2;

		private int index = 0;

		private Action<int> removeDelegate;

		public SuperFunction(){

			dic = new Dictionary<int, SuperFunctionUnitBase>();
			dic2 = new Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnitBase>>>();
		}

		public void AddRemoveDelegate(Action<int> _dele){

			removeDelegate += _dele;
		}

		public void RemoveRemoveDelegate(Action<int> _dele){

			removeDelegate -= _dele;
		}

		public int AddOnceEventListener(GameObject _target,string _eventName,SuperFunctionCallBack _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener(GameObject _target,string _eventName,SuperFunctionCallBack _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		private int AddEventListener(GameObject _target,string _eventName,SuperFunctionCallBack _callBack,bool _isOnce){

			int result = GetIndex();

			SuperFunctionUnit unit = new SuperFunctionUnit(_target,_eventName,_callBack,result,_isOnce);

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

		public int AddOnceEventListener<T>(GameObject _target,string _eventName,SuperFunctionCallBackV<T> _callBack) where T : struct{

			return AddEventListener<T>(_target,_eventName,_callBack,true);
		}

		public int AddEventListener<T>(GameObject _target,string _eventName,SuperFunctionCallBackV<T> _callBack) where T : struct{
			
			return AddEventListener<T>(_target,_eventName,_callBack,false);
		}

		public int AddEventListener<T>(GameObject _target,string _eventName,SuperFunctionCallBackV<T> _callBack,bool _isOnce) where T : struct{

			int result = GetIndex();

			SuperFunctionUnitV<T> unit = new SuperFunctionUnitV<T>(_target,_eventName,_callBack,result,_isOnce);

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

				if(removeDelegate != null){

					removeDelegate(_index);
				}

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

						SuperFunctionUnitBase unit = tmpList[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
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

						SuperFunctionUnitBase unit = list[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
					}

					tmpDic.Remove(_eventName);

					if(tmpDic.Count == 0){

						DestroyControl(_target);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target,string _eventName,SuperFunctionCallBack _callBack){

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnitBase> list = tmpDic[_eventName];

					for(int i = 0 ; i < list.Count ; i++){

						if(list[i] is SuperFunctionUnit){

							SuperFunctionUnit unit = list[i] as SuperFunctionUnit;

							if(unit.callBack == _callBack){

								dic.Remove(unit.index);

								if(removeDelegate != null){

									removeDelegate(unit.index);
								}

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

		public void RemoveEventListener<T>(GameObject _target,string _eventName,SuperFunctionCallBackV<T> _callBack)where T : struct{

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnitBase> list = tmpDic[_eventName];
					
					for(int i = 0 ; i < list.Count ; i++){
						
						if(list[i] is SuperFunctionUnitV<T>){
							
							SuperFunctionUnitV<T> unit = list[i] as SuperFunctionUnitV<T>;
							
							if(unit.callBack == _callBack){
								
								dic.Remove(unit.index);

								if(removeDelegate != null){

									removeDelegate(unit.index);
								}
								
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

		public void DispatchEvent(GameObject _target,string _eventName,params object[] _datas){

			if (dic2.ContainsKey (_target)) {
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnitBase> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						if(tmpList[i] is SuperFunctionUnit){

							SuperFunctionUnit unit = tmpList[i] as SuperFunctionUnit;

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							enumerator.Current.callBack(enumerator.Current.index,_datas);
						}
					}
				}
			}
		}

		public void DispatchEvent<T>(GameObject _target,string _eventName,ref T _v,params object[] _datas) where T : struct{
			
			if (dic2.ContainsKey (_target)) {
				
				Dictionary<string,List<SuperFunctionUnitBase>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnitBase> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnitV<T>> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						if(tmpList[i] is SuperFunctionUnitV<T>){
						
							SuperFunctionUnitV<T> unit = tmpList[i] as SuperFunctionUnitV<T>;

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnitV<T>>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnitV<T>>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							enumerator.Current.callBack(enumerator.Current.index,ref _v,_datas);
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

						SuperFunctionUnitBase unit = tmpList[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
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

		public int GetNum(){

			return dic.Count;
		}
	}
}
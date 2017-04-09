using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using gameObjectFactory;
using publicTools;
using assetManager;

using System;
using superTween;

namespace gameObjectFactory
{

	public class GameObjectFactoryUnit
	{
		private string name;

		private GameObject data;

		private int type = -1;

		public int useNum{private set;get;}

		private LinkedList<Action<GameObject,string>> callBackList = new LinkedList<Action<GameObject, string>>();

		private LinkedList<Action<string>> callBackList2 = new LinkedList<Action<string>>();

		public GameObjectFactoryUnit (string _name)
		{
			name = _name;
		}

		public void PreloadGameObject(Action<string> _callBack){

			Action<string> callBack = delegate(string _msg) {

				_callBack(_msg);
			};

			GetGameObject(callBack);
		}

		private void GetGameObject (Action<string> _callBack){
			
			if (type == -1) {
				
				type = 0;
				
				callBackList2.AddLast (_callBack);
				
				AssetManager.Instance.GetAsset<GameObject> (name, GetResouece);
				
			} else if (type == 0) {
				
				callBackList2.AddLast (_callBack);
				
			} else {
				
				_callBack (string.Empty);
			}
		}

		public GameObject GetGameObject (Action<GameObject,string> _callBack){

			if (type == -1) {

				type = 0;

				callBackList.AddLast (_callBack);

				AssetManager.Instance.GetAsset<GameObject> (name, GetResouece);

				return null;

			} else if (type == 0) {

				callBackList.AddLast (_callBack);

				return null;

			} else {

				GameObject result = GameObject.Instantiate<GameObject> (data);

				if (_callBack != null) {

					_callBack (result,string.Empty);
				}

				return result;
			}
		}

		private void GetResouece (GameObject _go,string _msg)
		{
			data = _go;

			type = 1;

			LinkedList<Action<GameObject,string>>.Enumerator enumerator = callBackList.GetEnumerator();

			while(enumerator.MoveNext()){

				Action<GameObject,string> callBack = enumerator.Current;

				if(callBack != null){

					if(_go != null){

						GameObject result = GameObject.Instantiate (data);
						
						callBack (result,string.Empty);

					}else{

						callBack(null,_msg);
					}
				}
			}

			callBackList.Clear ();

			LinkedList<Action<string>>.Enumerator enumerator2 = callBackList2.GetEnumerator();
			
			while(enumerator2.MoveNext()){
				
				Action<string> callBack = enumerator2.Current;
				
				if(callBack != null){
					
					callBack (string.Empty);
				}
			}
			
			callBackList2.Clear ();
		}

		public void AddUseNum ()
		{
			useNum++;
		}

		public void DelUseNum ()
		{
			useNum--;
		}

		public void Dispose ()
		{
			if (data != null) {

				data = null;
			}
		}
	}
}
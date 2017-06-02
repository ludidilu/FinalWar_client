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

		private List<Action<GameObject,string>> callBackList = new List<Action<GameObject, string>>();

		private List<Action<string>> callBackList2 = new List<Action<string>>();

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
				
				callBackList2.Add (_callBack);
				
				AssetManager.Instance.GetAsset<GameObject> (name, GetResouece);
				
			} else if (type == 0) {
				
				callBackList2.Add (_callBack);
				
			} else {
				
				_callBack (string.Empty);
			}
		}

		public GameObject GetGameObject (Action<GameObject,string> _callBack){

			if (type == -1) {

				type = 0;

				callBackList.Add (_callBack);

				AssetManager.Instance.GetAsset<GameObject> (name, GetResouece);

				return null;

			} else if (type == 0) {

				callBackList.Add (_callBack);

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

            for (int i = 0; i < callBackList.Count; i++)
            {
                Action<GameObject, string> callBack = callBackList[i];

                if (callBack != null)
                {
                    if (_go != null)
                    {
                        GameObject result = GameObject.Instantiate(data);

                        callBack(result, string.Empty);
                    }
                    else {

                        callBack(null, _msg);
                    }
                }
            }

			callBackList.Clear ();

            for (int i = 0; i < callBackList2.Count; i++)
            {
                Action<string> callBack = callBackList2[i];

                if (callBack != null)
                {
                    callBack(string.Empty);
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
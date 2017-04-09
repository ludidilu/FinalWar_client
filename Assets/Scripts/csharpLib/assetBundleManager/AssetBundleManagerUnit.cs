using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using wwwManager;
using System;

namespace assetBundleManager{

	public class AssetBundleManagerUnit{

		private AssetBundle assetBundle;
		private string name;
		private int type = -1;
		private LinkedList<Action<AssetBundle,string>> callBackList;
		private int useTimes;

		public AssetBundleManagerUnit(string _name){

			name = _name;

			callBackList = new LinkedList<Action<AssetBundle,string>> ();
		}

		public void Load(Action<AssetBundle,string> _callBack){

			useTimes++;

//			SuperDebug.Log ("LoadAssetBundle:" + name);

			if (type == -1) {

				type = 0;

				callBackList.AddLast (_callBack);

				WWWManager.Instance.Load(AssetBundleManager.path + name,GetAssetBundle);

			} else if (type == 0) {

				callBackList.AddLast (_callBack);

			} else {

				_callBack (assetBundle,string.Empty);
			}
		}

		private void GetAssetBundle(WWW _www){

			type = 1;

			LinkedList<Action<AssetBundle,string>>.Enumerator enumerator = callBackList.GetEnumerator();

			if(string.IsNullOrEmpty(_www.error)){

				assetBundle = _www.assetBundle;

				while(enumerator.MoveNext()){

					enumerator.Current(assetBundle,string.Empty);
				}

			}else{

				while(enumerator.MoveNext()){
					
					enumerator.Current(null,"AssetBundle can not be found:" + name);
				}
			}

			callBackList.Clear ();
		}

		public void Unload(){

			useTimes--;

			if (useTimes == 0) {

//				SuperDebug.Log ("dispose assetBundle:" + name);

				assetBundle.Unload (false);

				AssetBundleManager.Instance.Remove (name);
			}
		}
	}
}

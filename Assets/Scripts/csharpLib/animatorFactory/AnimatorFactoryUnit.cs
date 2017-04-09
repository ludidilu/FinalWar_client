using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using assetManager;

namespace animatorFactoty{

	public class AnimatorFactoryUnit{

		private string name;
		private RuntimeAnimatorController data;

		private int type = -1;

		private LinkedList<Action<RuntimeAnimatorController,string>> callBackList = new LinkedList<Action<RuntimeAnimatorController,string>> ();

		public int useNum;

		public AnimatorFactoryUnit (string _name)
		{
			name = _name;
		}

		public RuntimeAnimatorController GetAnimator (Action<RuntimeAnimatorController,string> _callBack)
		{
			if(type == -1){

				type = 0;

				callBackList.AddLast(_callBack);

				return AssetManager.Instance.GetAsset<RuntimeAnimatorController>(name,GetAsset);

			}else if(type == 0){

				callBackList.AddLast(_callBack);

				return null;

			}else{

				if(_callBack != null){

					if(data != null){

						_callBack(data,string.Empty);

					}else{

						_callBack(null,"AnimatorController load fail:" + name);
					}
				}

				return data;
			}
		}

		private void GetAsset(RuntimeAnimatorController _data,string _msg){

			data = _data;

			data.name = name;

			type = 1;

			LinkedList<Action<RuntimeAnimatorController,string>>.Enumerator enumerator = callBackList.GetEnumerator();

			if(_data != null){

				while(enumerator.MoveNext()){

					enumerator.Current(data,string.Empty);
				}

			}else{

				while(enumerator.MoveNext()){
					
					enumerator.Current(null,"AnimatorController load fail:" + name);
				}
			}

			callBackList.Clear();
		}

		public void DelUseNum(){

			useNum--;
		}

		public void AddUseNum(){
			
			useNum++;
		}

		public void Dispose(){

			Resources.UnloadAsset (data);
		}
	}
}
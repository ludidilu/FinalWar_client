using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using assetManager;
using System;

namespace textureFactory{

	public class TextureFactoryUnit<T> :ITextureFactoryUnit where T:UnityEngine.Object {

		private string name;
		private T data;
		private int type = -1;

		private bool isDispose = false;

		private List<Action<T,string>> callBackList = new List<Action<T, string>>();

		public TextureFactoryUnit(string _name){
			
			name = _name;
		}

		public T GetTexture(Action<T,string> _callBack){

			if (type == -1) {
				
				type = 0;
				
				callBackList.Add (_callBack);
				
				return AssetManager.Instance.GetAsset<T> (name,GetAsset);

			} else if (type == 0) {
				
				callBackList.Add (_callBack);

				return default(T);
				
			} else {

				if(_callBack != null){

					_callBack(data,string.Empty);
				}

				return data;
			}
		}

		private void GetAsset(T _data,string _msg){

			if(isDispose){

				Resources.UnloadAsset(_data);

				return;
			}

			data = _data;

			type = 1;

            for (int i = 0; i < callBackList.Count; i++)
            {
                Action<T, string> callBack = callBackList[i];

                if (callBack != null)
                {

                    callBack(data, string.Empty);
                }
            }

			callBackList.Clear();
		}

		public void Dispose(){

			if (type == 1) {
				
				Resources.UnloadAsset(data);

				data = null;

			}else{

				isDispose = true;
			}
		}
	}
}

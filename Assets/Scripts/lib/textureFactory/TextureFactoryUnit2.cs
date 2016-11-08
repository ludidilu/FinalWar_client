using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xy3d.tstd.lib.assetManager;
using System;

namespace xy3d.tstd.lib.textureFactory{
	
	public class TextureFactoryUnit2<T> :ITextureFactoryUnit where T:UnityEngine.Object {
		
		private string name;
		private T[] data;
		private int type = -1;
		
		private bool isDispose = false;

		private List<int> indexList = new List<int>();
		
		private List<Action<T>> callBackList = new List<Action<T>>();
		
		public TextureFactoryUnit2(string _name){
			
			name = _name;
		}
		
		public T GetTexture(int _index,Action<T> _callBack){
			
			if (type == -1) {
				
				type = 0;

				indexList.Add(_index);
				
				callBackList.Add (_callBack);

				T[] result = AssetManager.Instance.GetAsset<T> (name,GetAsset);

				if(result == null){

					return default(T);

				}else{

					return AssetManager.Instance.GetAsset<T> (name,GetAsset)[_index];
				}
				
			} else if (type == 0) {

				indexList.Add(_index);
				
				callBackList.Add (_callBack);
				
				return default(T);
				
			} else {
				
				if(_callBack != null){
					
					_callBack(data[_index]);
				}
				
				return data[_index];
			}
		}
		
		private void GetAsset(T[] _data){
			
			if(isDispose){

				for(int i = 0 ; i < _data.Length ; i++){
				
					Resources.UnloadAsset(_data[i]);
				}

				return;
			}
			
			data = _data;
			
			type = 1;

			for(int i = 0 ; i < callBackList.Count ; i++){
			
				Action<T> callBack = callBackList[i];

				if(callBack != null){
					
					callBack(data[indexList[i]]);
				}
			}

			indexList.Clear();
			
			callBackList.Clear();
		}
		
		public void Dispose(){
			
			if (type == 1) {
				
				for(int i = 0 ; i < data.Length ; i++){
					
					Resources.UnloadAsset(data[i]);
				}
				
				data = null;
				
			}else{
				
				isDispose = true;
			}
		}
	}
}

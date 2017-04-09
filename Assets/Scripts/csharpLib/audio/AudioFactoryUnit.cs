using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using assetManager;

namespace audio{

	public class AudioFactoryUnit {

		private string name;

		private AudioClip data;

		private int type = -1;

		public bool willDispose = true;

		private LinkedList<Action<AudioClip,string>> callBackList = new LinkedList<Action<AudioClip,string>>();

		public AudioFactoryUnit(string _name){

			name = _name;
		}

		public AudioClip GetClip(Action<AudioClip,string> _callBack,bool _willDispose){

			willDispose = willDispose && _willDispose;

			if (type == -1) {
				
				type = 0;
				
				callBackList.AddLast (_callBack);
				
				return AssetManager.Instance.GetAsset<AudioClip> (name,GetAsset);
				
			} else if (type == 0) {
				
				callBackList.AddLast (_callBack);
				
				return null;
				
			} else {
				
				if(_callBack != null){
					
					_callBack(data,string.Empty);
				}
				
				return data;
			}
		}

		private void GetAsset(AudioClip _data,string _msg){

			data = _data;

			data.name = name;
			
			type = 1;

			LinkedList<Action<AudioClip,string>>.Enumerator enumerator = callBackList.GetEnumerator();

			while(enumerator.MoveNext()){

				Action<AudioClip,string> callBack = enumerator.Current;

				if(callBack != null){

					callBack(data,_msg);
				}
			}
			
			callBackList.Clear();
		}

		public void Dispose(){

			Resources.UnloadAsset(data);
		}
	}
}
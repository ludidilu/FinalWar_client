using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using assetBundleManager;

using System;

namespace assetManager{
	
	public class AssetManagerUnit2<T> : IAssetManagerUnit where T:UnityEngine.Object {
		
		private AssetManagerData data;
		
		private int type = -1;
		
		private List<Action<T[],string>> callBackList = new List<Action<T[],string>>();
		private string name;
		
		public AssetManagerUnit2(string _name){
			
			name = _name;
			
			data = AssetManager.Instance.GetData (name);
		}
		
		public void Load(Action<T[],string> _callBack){
			
			callBackList.Add (_callBack);
			
			if (type == -1) {
				
				type = 0;
				
				StartLoad();
			}
		}
		
		private void StartLoad(){
			
			int loadNum = 2;
			
			AssetBundle assetBundle = null;

			string msg = string.Empty;
			
			Action<AssetBundle,string> callBack = delegate(AssetBundle _assetBundle,string _msg) {
				
				assetBundle = _assetBundle;

				if(!string.IsNullOrEmpty(_msg)){

					msg += _msg;
				}
				
				GetAssetBundle(ref loadNum,assetBundle,msg);
			};
			
			AssetBundleManager.Instance.Load (data.assetBundle, callBack);
			
			if (data.assetBundleDep != null) {
				
				callBack = delegate(AssetBundle _assetBundle,string _msg) {

					if(!string.IsNullOrEmpty(_msg)){
						
						msg += _msg;
					}
					
					GetAssetBundle(ref loadNum,assetBundle,msg);
				};

				for(int i = 0 ; i < data.assetBundleDep.Length ; i++){
				
					loadNum++;
					
					AssetBundleManager.Instance.Load (data.assetBundleDep[i], callBack);
				}
			}
			
			GetAssetBundle(ref loadNum,assetBundle,msg);
		}
		
		private void GetAssetBundle(ref int _loadNum,AssetBundle _assetBundle,string _msg){
			
			_loadNum--;
			
			if (_loadNum == 0) {

				if(string.IsNullOrEmpty(_msg)){
				
					if(AssetManager.LOADASYNC){
						
						AssetManager.Instance.script.Load<T>(name,_assetBundle,LoadOver);
						
					}else{

						string msg = string.Empty;
						
						T[] asset = null;
						
						try{

							asset = _assetBundle.LoadAssetWithSubAssets<T>(name);
							
						}catch(Exception e){

							msg = "Asset加载错误  name:" + name + "   msg:" + e.Message;
							
						}finally{
							
							LoadOver(asset,msg);
						}
					}

				}else{

					LoadOver(null,_msg);
				}
			}
		}
		
		private void LoadOver(T[] _asset,string _msg){
			
			AssetBundleManager.Instance.Unload(data.assetBundle);
			
			if(data.assetBundleDep != null){

				for(int i = 0 ; i < data.assetBundleDep.Length ; i++){
				
					AssetBundleManager.Instance.Unload(data.assetBundleDep[i]);
				}
			}
			
			AssetManager.Instance.RemoveUnit(name);

            for (int i = 0; i < callBackList.Count; i++)
            {
                callBackList[i](_asset, _msg);
            }
			
			callBackList.Clear();
		}
	}
}
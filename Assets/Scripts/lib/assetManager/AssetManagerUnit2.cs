using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xy3d.tstd.lib.assetBundleManager;

using System;

namespace xy3d.tstd.lib.assetManager{
	
	public class AssetManagerUnit2<T> : IAssetManagerUnit where T:UnityEngine.Object {
		
		private AssetManagerData data;
		
		private int type = -1;
		
		private List<Action<T[]>> callBackList = new List<Action<T[]>>();
		private string name;
		
		public AssetManagerUnit2(string _name){
			
			name = _name;
			
			data = AssetManager.Instance.GetData (name);
		}
		
		public void Load(Action<T[]> _callBack){
			
			callBackList.Add (_callBack);
			
			if (type == -1) {
				
				type = 0;
				
				StartLoad();
			}
		}
		
		private void StartLoad(){
			
			int loadNum = 2;
			
			AssetBundle assetBundle = null;
			
			Action<AssetBundle> callBack = delegate(AssetBundle _assetBundle) {
				
				assetBundle = _assetBundle;
				
				GetAssetBundle(ref loadNum,assetBundle);
			};
			
			AssetBundleManager.Instance.Load (data.assetBundle, callBack);
			
			if (data.assetBundleDep != null) {
				
				callBack = delegate(AssetBundle _assetBundle) {
					
					GetAssetBundle(ref loadNum,assetBundle);
				};

				for(int i = 0 ; i < data.assetBundleDep.Length ; i++){
				
					loadNum++;
					
					AssetBundleManager.Instance.Load (data.assetBundleDep[i], callBack);
				}
			}
			
			GetAssetBundle(ref loadNum,assetBundle);
		}
		
		private void GetAssetBundle(ref int _loadNum,AssetBundle _assetBundle){
			
			_loadNum--;
			
			if (_loadNum == 0) {
				
				if(AssetManager.LOADASYNC){
					
					AssetManager.Instance.script.Load<T>(name,_assetBundle,LoadOver);
					
				}else{
					
					T[] asset = null;
					
					try{

						asset = _assetBundle.LoadAssetWithSubAssets<T>(name);
						
					}catch(Exception e){
						
						SuperDebug.Log("Asset加载错误:" + e.Message);
						
					}finally{
						
						LoadOver(asset);
					}
				}
			}
		}
		
		private void LoadOver(T[] _asset){
			
			AssetBundleManager.Instance.Unload(data.assetBundle);
			
			if(data.assetBundleDep != null){

				for(int i = 0 ; i < data.assetBundleDep.Length ; i++){
				
					AssetBundleManager.Instance.Unload(data.assetBundleDep[i]);
				}
			}
			
			AssetManager.Instance.RemoveUnit(name);

			for(int i = 0 ; i < callBackList.Count ; i++){

				callBackList[i](_asset);
			}
			
			callBackList.Clear();
		}
	}
}
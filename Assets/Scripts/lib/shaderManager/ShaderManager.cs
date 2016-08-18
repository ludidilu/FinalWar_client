#if USE_ASSETBUNDLE
#else
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using xy3d.tstd.lib.wwwManager;
using System.IO;
using xy3d.tstd.lib.assetManager;
using xy3d.tstd.lib.assetBundleManager;

public class ShaderManager{
	
	private static ShaderManager _Instance;
	
	public static ShaderManager Instance{
		
		get{
			
			if(_Instance == null){
				
				_Instance = new ShaderManager();
			}
			
			return _Instance;
		}
	}
	
	public const string assetBundleName = "shader";
	
	private AssetBundle assetBundle;
	
	public void Init(Action _callBack){
		
		Action<AssetBundle> del = delegate(AssetBundle obj) {
			
			assetBundle = obj;
			
			_callBack();
		};
		
		AssetBundleManager.Instance.Load(assetBundleName,del);
	}
	
	public Shader GetShader(string _name){
		
		#if USE_ASSETBUNDLE
		return assetBundle.LoadAsset<Shader>(_name);
		#else
		return AssetDatabase.LoadAssetAtPath<Shader>(_name);
		#endif
	}
}

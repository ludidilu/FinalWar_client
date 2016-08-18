using UnityEngine;
using System.Collections;
using System;
using xy3d.tstd.lib.superTween;
using xy3d.tstd.lib.publicTools;

public class BallisticControl : EffectControl {

	[SerializeField]
	private AnimationCurve curve;

	private Vector3 start;

	private Vector3 end;

	private float startTime;

	private float time;

	private Action callBack;

	private bool show;

	public void Fly(Vector3 _start,Vector3 _end,float _time,Action _callBack){

		start = _start;

		end = _end;

		startTime = Time.time;

		time = _time;

		callBack = _callBack;

		PublicTools.SetGameObjectVisible(gameObject,false);
	}
	
	// Update is called once per frame
	void Update () {
	
		float percent = (Time.time - startTime) / time;

		if(percent > 1){

			callBack();
			
			GameObject.Destroy(gameObject);

		}else{

			if(!show){

				show = true;

				PublicTools.SetGameObjectVisible(gameObject,true);
			}

			Vector3 v = Vector3.Lerp(start,end,percent);

			v.y += curve.Evaluate(percent);

			transform.LookAt(v);

			transform.position = v;
		}
	}
}

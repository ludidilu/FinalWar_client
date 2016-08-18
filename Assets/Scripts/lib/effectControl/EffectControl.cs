using UnityEngine;
using System.Collections;

public class EffectControl : MonoBehaviour {

	public void EffectEnd(){

		GameObject.Destroy(gameObject);
	}

	public void EffectHitPoint(){

		SendMessageUpwards("HitPoint",SendMessageOptions.DontRequireReceiver);
	}

	public void EffectPlayEffectPoint(int _index){

		SendMessageUpwards("PlayEffectPoint",_index,SendMessageOptions.DontRequireReceiver);
	}
}

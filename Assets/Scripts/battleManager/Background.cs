using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superFunction;
using xy3d.tstd.lib.superRaycast;

public class Background : MonoBehaviour {

	void Awake(){

		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseClick, GetMouseClick);
	}

	private void GetMouseClick(SuperEvent e){

		if ((int)e.data [1] == 0) {

			SendMessageUpwards ("BackgroundClick");
		}
	}
}

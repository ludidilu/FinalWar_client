using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using xy3d.tstd.lib.superFunction;
using xy3d.tstd.lib.superRaycast;

public class MapUnit : MonoBehaviour {

//	public static bool touchable = true;

	[SerializeField]
	private MeshRenderer mainMr;

	public int index;

	// Use this for initialization
	void Awake () {
	
		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseButtonDown, GetMouseDown);

		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseButtonUp, GetMouseUp);

		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseEnter, GetMouseEnter);

		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseExit, GetMouseExit);

		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseClick, GetMouseClick);
	}

	public void SetMainColor(Color _color){

		mainMr.material.SetColor ("_Color", _color);
	}

	private void GetMouseDown(SuperEvent e){

		SendMessageUpwards ("MapUnitDown", this, SendMessageOptions.DontRequireReceiver);
	}

//	public void OnMouseDown(){
//
//		if (touchable) {
//
//			SendMessageUpwards ("MapUnitDown", this, SendMessageOptions.DontRequireReceiver);
//		}
//	}

	private void GetMouseUp(SuperEvent e){

		SendMessageUpwards ("MapUnitUp", this, SendMessageOptions.DontRequireReceiver);
	}

//	public void OnMouseUp(){
//
//		if (touchable) {
//		
//			SendMessageUpwards ("MapUnitUp", this, SendMessageOptions.DontRequireReceiver);
//		}
//	}

	private void GetMouseEnter(SuperEvent e){

		SendMessageUpwards ("MapUnitEnter", this, SendMessageOptions.DontRequireReceiver);
	}

//	public void OnMouseEnter(){
//
//		if (touchable) {
//		
//			SendMessageUpwards ("MapUnitEnter", this, SendMessageOptions.DontRequireReceiver);
//		}
//	}

	private void GetMouseExit(SuperEvent e){

		SendMessageUpwards ("MapUnitExit", this, SendMessageOptions.DontRequireReceiver);
	}
	
//	public void OnMouseExit(){
//
//		if (touchable) {
//		
//			SendMessageUpwards ("MapUnitExit", this, SendMessageOptions.DontRequireReceiver);
//		}
//	}

	private void GetMouseClick(SuperEvent e){

		SendMessageUpwards ("MapUnitUpAsButton", this, SendMessageOptions.DontRequireReceiver);
	}

//	public void OnMouseUpAsButton(){
//
//		if (touchable) {
//
//			SendMessageUpwards ("MapUnitUpAsButton", this, SendMessageOptions.DontRequireReceiver);
//		}
//	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}

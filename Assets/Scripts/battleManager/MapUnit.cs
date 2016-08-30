using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MapUnit : MonoBehaviour {

	public static bool touchable = true;

	[SerializeField]
	private MeshRenderer mainMr;

	public int index;

	// Use this for initialization
	void Start () {
	
	}

	public void SetMainColor(Color _color){

		mainMr.material.SetColor ("_Color", _color);
	}

	public void OnMouseDown(){

		if (touchable) {

			SendMessageUpwards ("MapUnitDown", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnMouseUp(){

		if (touchable) {
		
			SendMessageUpwards ("MapUnitUp", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnMouseEnter(){

		if (touchable) {
		
			SendMessageUpwards ("MapUnitEnter", this, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void OnMouseExit(){

		if (touchable) {
		
			SendMessageUpwards ("MapUnitExit", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnMouseUpAsButton(){

		if (touchable) {

			SendMessageUpwards ("MapUnitUpAsButton", this, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using xy3d.tstd.lib.superRaycast;

public class AlertPanel : MonoBehaviour,IPointerClickHandler {

	[SerializeField]
	private Text alertText;

	public void Alert(string _str){

		if (!gameObject.activeSelf) {

			gameObject.SetActive (true);
			
			SuperRaycast.SetIsOpen(false,"a");
		}

		alertText.text = _str;
	}

	public void OnPointerClick(PointerEventData _data){

		gameObject.SetActive (false);

		SuperRaycast.SetIsOpen(true,"a");
	}
}

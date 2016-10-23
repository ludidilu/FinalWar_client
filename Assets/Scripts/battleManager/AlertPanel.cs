using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using xy3d.tstd.lib.superRaycast;
using System;

public class AlertPanel : MonoBehaviour,IPointerClickHandler {

	[SerializeField]
	private Text alertText;

	private Action callBack;

	public void Alert(string _str,Action _callBack){

		if (!gameObject.activeSelf) {

			gameObject.SetActive (true);
			
			SuperRaycast.SetIsOpen(false,"a");
		}

		callBack = _callBack;

		alertText.text = _str;
	}

	public void OnPointerClick(PointerEventData _data){

		gameObject.SetActive (false);

		SuperRaycast.SetIsOpen(true,"a");

		if (callBack != null) {

			callBack();
		}
	}
}

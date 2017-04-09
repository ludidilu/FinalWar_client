using UnityEngine;
using System.Collections;
using superFunction;

public class EventDispatcher : MonoBehaviour {

	public void Dispatch(string _event){

		SuperFunction.Instance.DispatchEvent(gameObject,_event);
	}
}

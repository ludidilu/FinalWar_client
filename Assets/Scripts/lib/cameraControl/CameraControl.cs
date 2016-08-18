using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField]
	private Transform renderCamera;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private float moveSpeed = 1f;

	[SerializeField]
	private float rotateSpeed = 2f;

	[SerializeField]
	private float scrollSpeed = 0.1f;

	[SerializeField]
	private float maxAngle = 45;

	[SerializeField]
	private float minAngle = 10;

	[SerializeField]
	private float distance = 3;

	// Use this for initialization
	void Start () {
	
		RefreshCameraAngle();

		RefreshCameraDistance();

		RefreshCameraPosition();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 p = Vector3.zero;

		if(Input.GetKey(KeyCode.UpArrow)){

			Vector3 forward = renderCamera.forward;
			
			forward.y = 0;
			
			p = p + forward.normalized;
		}

		if(Input.GetKey(KeyCode.DownArrow)){

			Vector3 forward = renderCamera.forward;
			
			forward.y = 0;
			
			p = p - forward.normalized;
		}

		if(Input.GetKey(KeyCode.LeftArrow)){

			Vector3 right = renderCamera.right;
			
			right.y = 0;
			
			p = p - right.normalized;
		}

		if(Input.GetKey(KeyCode.RightArrow)){

			Vector3 right = renderCamera.right;
			
			right.y = 0;
			
			p = p + right.normalized;
		}

		if(p != Vector3.zero){

			transform.position += p.normalized * Time.deltaTime * moveSpeed;
		}

		bool hasChange = false;

		if(!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0)){

			RefreshCameraAngle();

			hasChange = true;
		}

		if(Input.mouseScrollDelta.y != 0){

			RefreshCameraDistance();

			hasChange = true;
		}

		if(hasChange){

			RefreshCameraPosition();
		}
	}

	private void RefreshCameraAngle(){

		Vector3 q = target.localEulerAngles;

		q.x += GetInputDeltaX() * rotateSpeed;
		q.y += GetInputDeltaY() * rotateSpeed;
		
		if(q.x > 360 - minAngle){
			
			q.x = 360 - minAngle;
			
		}else if(q.x < 360 - maxAngle){
			
			q.x = 360 - maxAngle;
		}
		
		target.localEulerAngles = q;
	}

	private void RefreshCameraDistance(){

		distance -= Input.mouseScrollDelta.y * scrollSpeed;
	}

	private void RefreshCameraPosition(){

		renderCamera.localPosition = target.forward * distance;

		renderCamera.LookAt (target);
	}

	private float GetInputDeltaX(){

		#if PLATFORM_PC
		
		return Input.GetAxis("Mouse Y");
		
		#else
		
		return Input.GetTouch(0).deltaPosition.y;
		
		#endif
	}

	private float GetInputDeltaY(){
		
		#if PLATFORM_PC
		
		return Input.GetAxis("Mouse X");
		
		#else

		return Input.GetTouch(0).deltaPosition.x;
		
		#endif
	}
}

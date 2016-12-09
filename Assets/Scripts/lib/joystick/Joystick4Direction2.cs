using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using xy3d.tstd.lib.superFunction;

public class Joystick4Direction2 : MonoBehaviour {

	[SerializeField]
	private Canvas canvas;
	
	[SerializeField]
	private RectTransform rect;

	private Rect clickArea;
	
	private Vector2 downPos;
	
	private bool isDown = false;

	private float maxValue;
	
	void Awake(){

	}
	
	void Start(){

		maxValue = JoystickData.moveMaxValue * canvas.scaleFactor;
		
		clickArea = new Rect(rect.rect.x + rect.anchoredPosition.x,rect.rect.y + rect.anchoredPosition.y,rect.rect.width,rect.rect.height);
	}

	private void Down(){
		
		isDown = true;
		
		downPos = Input.mousePosition;

		SuperEvent downEvent = new SuperEvent(JoystickData.DOWN);
		
		downEvent.data = new object[2];
		
		downEvent.data[0] = this;
		
		downEvent.data[1] = downPos;
		
		SuperFunction.Instance.DispatchEvent(gameObject,downEvent);
	}
	
	private void Up(){
		
		isDown = false;

		SuperEvent upEvent = new SuperEvent(JoystickData.UP);
		
		upEvent.data = new object[1];
		
		upEvent.data[0] = this;
		
		SuperFunction.Instance.DispatchEvent(gameObject,upEvent);
	}
	
	void Update(){
		
		if(isDown){
			
			if(!Input.GetMouseButton(0)){
				
				Up ();
				
				return;
			}
			
			float dx = Input.mousePosition.x - downPos.x;
			
			float dy = Input.mousePosition.y - downPos.y;
			
			if(Mathf.Abs(dx) > Mathf.Abs(dy)){

				if(Mathf.Abs(dx) > maxValue){

					JoystickData.Direction direction;
				
					if(dx > 0){
						
						direction = JoystickData.Direction.RIGHT;
						
					}else{
						
						direction = JoystickData.Direction.LEFT;
					}

					SuperEvent moveEvent = new SuperEvent(JoystickData.MOVE);
					
					moveEvent.data = new object[2];
					
					moveEvent.data[0] = this;

					moveEvent.data[1] = direction;
					
					SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);

					downPos = Input.mousePosition;
				}
				
			}else{

				if(Mathf.Abs(dy) > maxValue){

					JoystickData.Direction direction;

					if(dy > 0){
						
						direction = JoystickData.Direction.UP;
						
					}else{
						
						direction = JoystickData.Direction.DOWN;
					}

					SuperEvent moveEvent = new SuperEvent(JoystickData.MOVE);
					
					moveEvent.data = new object[2];
					
					moveEvent.data[0] = this;

					moveEvent.data[1] = direction;
					
					SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);

					downPos = Input.mousePosition;
				}
			}

		}else if(Input.GetMouseButtonDown(0)){
				
			Vector3 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
			
			if(clickArea.Contains(v)){
				
				Down();
			}
		}
	}
	
	void OnDisable(){
		
		isDown = false;
	}
}

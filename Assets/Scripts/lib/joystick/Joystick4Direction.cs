using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using xy3d.tstd.lib.superFunction;

public class Joystick4Direction : MonoBehaviour {

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private RectTransform rect;

	[SerializeField]
	private float maxValue;

	private Rect clickArea;

	private Vector2 downPos;

	private bool isDown = false;

	private SuperEvent downEvent;

	private SuperEvent moveEvent;

	private SuperEvent upEvent;

	void Awake(){

		downEvent = new SuperEvent(Joystick4DirectionData.DOWN);

		downEvent.data = new object[2];

		downEvent.data[0] = this;

		moveEvent = new SuperEvent(Joystick4DirectionData.MOVE);

		moveEvent.data = new object[4];

		moveEvent.data[0] = this;

		upEvent = new SuperEvent(Joystick4DirectionData.UP);

		upEvent.data = new object[1];
		
		upEvent.data[0] = this;
	}

	void Start(){

		if(rect != null){

			clickArea = new Rect(rect.rect.x + rect.anchoredPosition.x,rect.rect.y + rect.anchoredPosition.y,rect.rect.width,rect.rect.height);
		}
	}

	public void Init(Canvas _canvas,RectTransform _rect,float _maxValue){

		canvas = _canvas;

		rect = _rect;

		maxValue = _maxValue;
	}

	private void Down(){

		isDown = true;

		downPos = Input.mousePosition;

		downEvent.data[1] = downPos;

		SuperFunction.Instance.DispatchEvent(gameObject,downEvent);
	}

	private void Up(){

		isDown = false;

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

			Joystick4DirectionData.Direction direction;

			float moveDis;

			if(Mathf.Abs(dx) > Mathf.Abs(dy)){

				dx = Mathf.Clamp(dx,-maxValue,maxValue);

				if(dx > 0){

					direction = Joystick4DirectionData.Direction.RIGHT;

				}else{

					direction = Joystick4DirectionData.Direction.LEFT;
				}

				moveDis = dx;

			}else{

				dy = Mathf.Clamp(dy,-maxValue,maxValue);

				if(dy > 0){
					
					direction = Joystick4DirectionData.Direction.UP;
					
				}else{
					
					direction = Joystick4DirectionData.Direction.DOWN;
				}

				moveDis = dy;
			}

			moveEvent.data[1] = direction;

			moveEvent.data[2] = moveDis;

			moveEvent.data[3] = downPos;

			SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);

		}else if(Input.GetMouseButtonDown(0)){
				
			if(rect != null){
				
				Vector3 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);

				if(clickArea.Contains(v)){
					
					Down();
				}
				
			}else{
				
				Down ();
			}
		}
	}
	
	void OnDisable(){
		
		isDown = false;
	}
}

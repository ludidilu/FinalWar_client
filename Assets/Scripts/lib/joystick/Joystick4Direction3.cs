using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using xy3d.tstd.lib.superFunction;

public class Joystick4Direction3 : MonoBehaviour {
	
	[SerializeField]
	private Canvas canvas;
	
	[SerializeField]
	private RectTransform rect;
	
	[SerializeField]
	private float maxValue;
	
	private Rect clickArea;
	
	private Vector2 downPos;
	
	private bool isDown = false;
	
	void Awake(){

	}
	
	void Start(){
		
		if(rect != null){

			Vector3 v1 = PublicTools.MousePositionToCanvasPosition(canvas,Vector3.zero);

			Vector3 v2 = PublicTools.MousePositionToCanvasPosition(canvas,new Vector3(1,0,0));

			//这个fix是表示鼠标每移动1在canvas中移动fix距离
			float fix = v2.x - v1.x;

			clickArea = new Rect(rect.rect.x + rect.anchoredPosition.x,rect.rect.y + rect.anchoredPosition.y,rect.rect.width,rect.rect.height);

			clickArea.width = clickArea.width - maxValue * 2 * fix;

			clickArea.height = clickArea.height - maxValue * 2 * fix;

			clickArea.center = clickArea.center + new Vector2(maxValue * fix,maxValue * fix);
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

		SuperEvent downEvent = new SuperEvent(Joystick4DirectionData.DOWN);
		
		downEvent.data = new object[2];
		
		downEvent.data[0] = this;
		
		downEvent.data[1] = downPos;
		
		SuperFunction.Instance.DispatchEvent(gameObject,downEvent);
	}
	
	private void Up(){
		
		isDown = false;
		
		SuperEvent upEvent = new SuperEvent(Joystick4DirectionData.UP);
		
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

			float dis = Vector2.Distance(downPos,Input.mousePosition);

			if(dis > maxValue){

				downPos = Vector2.Lerp(downPos,Input.mousePosition,(dis - maxValue) / dis);

				FixDownPos();
			}

			SuperEvent moveEvent = new SuperEvent(Joystick4DirectionData.MOVE);
			
			moveEvent.data = new object[4];
			
			moveEvent.data[0] = this;

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

	private void FixDownPos(){

		Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas,downPos);

		if(!clickArea.Contains(v)){

			if(v.x < clickArea.xMin){

				v = new Vector3(clickArea.xMin,v.y);

			}else if(v.x > clickArea.xMax){

				v = new Vector2(clickArea.xMax,v.y);
			}

			if(v.y < clickArea.yMin){
				
				v = new Vector3(v.x,clickArea.yMin);
				
			}else if(v.y > clickArea.yMax){
				
				v = new Vector2(v.x,clickArea.yMax);
			}

			downPos = PublicTools.CanvasPostionToMousePosition(canvas,v);
		}
	}
	
	void OnDisable(){
		
		isDown = false;
	}
}

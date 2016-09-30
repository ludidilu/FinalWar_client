using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using xy3d.tstd.lib.superFunction;

public class Joystick4Direction2 : MonoBehaviour {
	
	public enum Direction{
		
		UP,
		DOWN,
		RIGHT,
		LEFT
	}
	
	public const string DOWN = "joystickDown";
	
	public const string MOVE = "joystickMove";
	
	public const string UP = "joystickUp";
	
	[SerializeField]
	private Canvas canvas;
	
	[SerializeField]
	private RectTransform rect;
	
	[SerializeField]
	private float maxValue;
	
	private Rect clickArea;
	
	private Vector3 downPos;
	
	private bool isDown = false;
	
	private SuperEvent downEvent;
	
	private SuperEvent moveEvent;
	
	private SuperEvent upEvent;
	
	void Awake(){
		
		downEvent = new SuperEvent(DOWN);
		
		downEvent.data = new object[1];
		
		moveEvent = new SuperEvent(MOVE);
		
		moveEvent.data = new object[2];
		
		upEvent = new SuperEvent(UP);
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
		
		downEvent.data[0] = downPos;
		
		SuperFunction.Instance.DispatchEvent(gameObject,downEvent);
	}
	
	private void Up(){
		
		isDown = false;
		
		SuperFunction.Instance.DispatchEvent(gameObject,upEvent);
	}
	
	void Update(){
		
		if(isDown){
			
			if(Input.GetMouseButtonUp(0)){
				
				Up ();
				
				return;
			}
			
			float dx = Input.mousePosition.x - downPos.x;
			
			float dy = Input.mousePosition.y - downPos.y;
			
			Direction direction;
			
			if(Mathf.Abs(dx) > Mathf.Abs(dy)){

				if(Mathf.Abs(dx) > maxValue){
				
					if(dx > 0){
						
						direction = Direction.RIGHT;
						
					}else{
						
						direction = Direction.LEFT;
					}

					moveEvent.data[0] = direction;
					
					SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);

					downPos = Input.mousePosition;
				}
				
			}else{

				if(Mathf.Abs(dy) > maxValue){

					if(dy > 0){
						
						direction = Direction.UP;
						
					}else{
						
						direction = Direction.DOWN;
					}

					moveEvent.data[0] = direction;
					
					SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);

					downPos = Input.mousePosition;
					
				}
			}

//			downPos = Input.mousePosition;
			
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
}

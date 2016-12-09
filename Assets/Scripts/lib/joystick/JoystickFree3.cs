using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using xy3d.tstd.lib.superFunction;

public class JoystickFree3 : MonoBehaviour {
	
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
		
		Vector3 v1 = PublicTools.MousePositionToCanvasPosition(canvas,Vector3.zero);
		
		Vector3 v2 = PublicTools.MousePositionToCanvasPosition(canvas,new Vector3(1,0,0));
		
		//这个fix是表示鼠标每移动1在canvas中移动fix距离
		float fix = v2.x - v1.x;
		
		clickArea = new Rect(rect.rect.x + rect.anchoredPosition.x,rect.rect.y + rect.anchoredPosition.y,rect.rect.width,rect.rect.height);
		
		clickArea.width = clickArea.width - maxValue * 2 * fix;
		
		clickArea.height = clickArea.height - maxValue * 2 * fix;
		
		clickArea.center = clickArea.center + new Vector2(maxValue * fix,maxValue * fix);
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

			dx = Mathf.Clamp(dx,-maxValue,maxValue);
				
			dy = Mathf.Clamp(dy,-maxValue,maxValue);
			
			float dis = Vector2.Distance(downPos,Input.mousePosition);
			
			if(dis > maxValue){
				
				downPos = Vector2.Lerp(downPos,Input.mousePosition,(dis - maxValue) / dis);
				
				FixDownPos();
			}
			
			SuperEvent moveEvent = new SuperEvent(JoystickData.MOVE);
			
			moveEvent.data = new object[3];
			
			moveEvent.data[0] = this;
			
			moveEvent.data[1] = new Vector2(dx,dy);
			
			moveEvent.data[2] = downPos;
			
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

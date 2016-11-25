using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superFunction;
using xy3d.tstd.lib.publicTools;

public class ClickMove : MonoBehaviour {

	public enum Direction{
		
		UP,
		DOWN,
		RIGHT,
		LEFT
	}

	public const string MOVE = "clickMove";

	public const string UP = "clickUp";

	[SerializeField]
	private Canvas canvas;
	
	[SerializeField]
	private RectTransform rect;

	private Rect clickArea;

	private SuperEvent moveEvent;

	private SuperEvent upEvent;

	private bool isDown = false;

	private float topRight;

	private float topLeft;

	private float bottomRight;

	private float bottomLeft;

	void Awake(){

		moveEvent = new SuperEvent(MOVE);

		moveEvent.data = new object[1];

		upEvent = new SuperEvent(UP);
	}

	void Start(){
		
		if(rect != null){
			
			clickArea = new Rect(rect.rect.x + rect.anchoredPosition.x,rect.rect.y + rect.anchoredPosition.y,rect.rect.width,rect.rect.height);
		}
	}

	void Update(){
		
		if(isDown){
			
			if(!Input.GetMouseButton(0)){

				isDown = false;

				SuperFunction.Instance.DispatchEvent(gameObject,upEvent);
				
				return;
			}

			Vector3 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
			
			Vector3 topRight = Vector3.Cross(new Vector3(clickArea.xMax,clickArea.yMax,0),v);

			Vector3 topLeft = Vector3.Cross(new Vector3(clickArea.xMin,clickArea.yMax,0),v);

			Vector3 bottomRight = Vector3.Cross(new Vector3(clickArea.xMax,clickArea.yMin,0),v);

			Vector3 bottomLeft = Vector3.Cross(new Vector3(clickArea.xMin,clickArea.yMin,0),v);

			Direction direction;

			if(topLeft.z > 0 && topRight.z < 0){

				direction = Direction.DOWN;

			}else if(topRight.z > 0 && bottomRight.z < 0){

				direction = Direction.LEFT;

			}else if(bottomRight.z > 0 && bottomLeft.z < 0){

				direction = Direction.UP;

			}else{

				direction = Direction.RIGHT;
			}

			moveEvent.data[0] = direction;
			
			SuperFunction.Instance.DispatchEvent(gameObject,moveEvent);
			
		}else if(Input.GetMouseButtonDown(0)){
			
			if(rect != null){
				
				Vector3 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
				
				if(clickArea.Contains(v)){
					
					isDown = true;
				}
				
			}else{
				
				isDown = true;
			}
		}
	}

	void OnDisable(){
		
		isDown = false;
	}
}

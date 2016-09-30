using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.publicTools;
using UnityEngine.UI;

public class GG : MonoBehaviour {

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private RectTransform rect;

	// Use this for initialization
	void Start () {
	
	}

	private Vector3 downPos;
	
	// Update is called once per frame
	void Update () {
	
		if(Input.mouseScrollDelta.y < 0){

			Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
			
			Vector2 v2 = (v - rect.anchoredPosition) / rect.localScale.x;

			rect.localScale = rect.localScale * 0.9f;

			rect.anchoredPosition = v - v2 * rect.localScale.x;

			FixRect();

		}else if(Input.mouseScrollDelta.y > 0){

			Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
			
			Vector2 v2 = (v - rect.anchoredPosition) / rect.localScale.x;
			
			rect.localScale = rect.localScale / 0.9f;
			
			rect.anchoredPosition = v - v2 * rect.localScale.x;
		}

		if(Input.GetMouseButtonDown(0)){

			downPos = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);

		}else if(Input.GetMouseButton(0)){

			if(rect.localScale.x > 1){
				
				Vector3 nowPos = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
				
				rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + nowPos.x - downPos.x,rect.anchoredPosition.y + nowPos.y - downPos.y);

				FixRect();

				downPos = nowPos;
			}
		}
	}

	private void FixRect(){

		if(rect.localScale.x < 1){

			rect.anchoredPosition = Vector2.zero;

		}else{

			if(rect.anchoredPosition.x - (canvas.transform as RectTransform).rect.width / 2 * rect.localScale.x > -(canvas.transform as RectTransform).rect.width / 2){
				
				rect.anchoredPosition = new Vector2(-(canvas.transform as RectTransform).rect.width / 2 + (canvas.transform as RectTransform).rect.width / 2 * rect.localScale.x,rect.anchoredPosition.y);
				
			}else if(rect.anchoredPosition.x + (canvas.transform as RectTransform).rect.width / 2 * rect.localScale.x < (canvas.transform as RectTransform).rect.width / 2){
				
				rect.anchoredPosition = new Vector2((canvas.transform as RectTransform).rect.width / 2 - (canvas.transform as RectTransform).rect.width / 2 * rect.localScale.x,rect.anchoredPosition.y);
			}
			
			if(rect.anchoredPosition.y - (canvas.transform as RectTransform).rect.height / 2 * rect.localScale.x > -(canvas.transform as RectTransform).rect.height / 2){
				
				rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,-(canvas.transform as RectTransform).rect.height / 2 + (canvas.transform as RectTransform).rect.height / 2 * rect.localScale.x);
				
			}else if(rect.anchoredPosition.y + (canvas.transform as RectTransform).rect.height / 2 * rect.localScale.x < (canvas.transform as RectTransform).rect.height / 2){
				
				rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,(canvas.transform as RectTransform).rect.height / 2 - (canvas.transform as RectTransform).rect.height / 2 * rect.localScale.x);
			}
		}
	}
}

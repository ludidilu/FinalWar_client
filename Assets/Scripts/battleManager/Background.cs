using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public void OnMouseUpAsButton(){
		
		if (MapUnit.touchable) {
			
			SendMessageUpwards("BackgroundClick");
		}
	}
}

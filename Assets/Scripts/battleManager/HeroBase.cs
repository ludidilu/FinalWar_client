using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour {

	[SerializeField]
	private Image frame;

	[SerializeField]
	private Image body;

	public HeroSDS sds;

	public int cardUid;
	
	public void SetFrameVisible(bool _visible){
		
		frame.gameObject.SetActive (_visible);
	}
	
	public void SetFrameColor(Color _color){
		
		frame.color = _color;
	}

	protected void SetBodyColor(){

		if (sds.threat) {
			
			body.color = BattleManager.threatColor;
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour {

	[SerializeField]
	private Image frame;

	[SerializeField]
	private Image body;

	[SerializeField]
	private Text nameText;

	public HeroSDS sds{ get; private set;}

	public int cardUid;

	protected void InitCard(HeroSDS _heroSDS){

		sds = _heroSDS;

		nameText.text = sds.name;
	}

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

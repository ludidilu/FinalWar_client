using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour {

	[SerializeField]
	public Image body;
	
	[SerializeField]
	protected Text hp;
	
	[SerializeField]
	protected Image frame;

	public HeroSDS sds;

	public int cardUid;
	
	public void SetFrameVisible(bool _visible){
		
		frame.gameObject.SetActive (_visible);
	}
	
	public void SetFrameColor(Color _color){
		
		frame.color = _color;
	}
}

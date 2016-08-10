using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCard : MonoBehaviour,IPointerClickHandler {

	[SerializeField]
	private Image frame;

	[SerializeField]
	public Image body;

	[SerializeField]
	private Text hp;

	public int cardUid;

	public HeroSDS sds;

	public void Init(int _cardUid,int _id){

		cardUid = _cardUid;

		sds = StaticData.GetData<HeroSDS> (_id);

		hp.text = sds.hp.ToString ();
	}

	public void Init(int _id,int _hp,int _power){

		cardUid = -1;

		sds = StaticData.GetData<HeroSDS> (_id);

		hp.text = _hp.ToString ();
	}

	public void SetHp(int _hp){

		hp.text = _hp.ToString ();
	}

	public void SetMouseEnable(bool _b){

		body.raycastTarget = _b;
	}

	public void SetFrameVisible(bool _visible){

		frame.gameObject.SetActive (_visible);
	}

	public void SetFrameColor(Color _color){

		frame.color = _color;
	}

	public void OnPointerClick(PointerEventData _data){

		SendMessageUpwards ("HeroClick", this, SendMessageOptions.DontRequireReceiver);
	}
}

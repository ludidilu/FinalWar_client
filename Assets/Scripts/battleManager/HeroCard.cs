using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCard : HeroBase,IPointerClickHandler {

	[SerializeField]
	protected Text cost;

	public void Init(int _cardUid,int _id){

		cardUid = _cardUid;

		sds = StaticData.GetData<HeroSDS> (_id);

		cost.text = sds.cost.ToString ();

		SetBodyColor ();
	}

	public void OnPointerClick(PointerEventData _data){

		SendMessageUpwards ("HeroClick", this, SendMessageOptions.DontRequireReceiver);
	}
}

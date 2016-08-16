using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCard : HeroBase,IPointerClickHandler {

	public void Init(int _cardUid,int _id){

		cardUid = _cardUid;

		sds = StaticData.GetData<HeroSDS> (_id);

		hp.text = sds.hp.ToString ();
	}

	public void OnPointerClick(PointerEventData _data){

		SendMessageUpwards ("HeroClick", this, SendMessageOptions.DontRequireReceiver);
	}
}

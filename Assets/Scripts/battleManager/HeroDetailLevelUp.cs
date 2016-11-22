using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroDetailLevelUp : MonoBehaviour,IPointerClickHandler {

	public void OnPointerClick(PointerEventData _data)
	{
		SendMessageUpwards("LevelUpClick", SendMessageOptions.DontRequireReceiver);
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroDetail : MonoBehaviour {

	[SerializeField]
	private Text cost;

	[SerializeField]
	private Text hp;

	[SerializeField]
	private Text attack;

	[SerializeField]
	private Text shoot;

	[SerializeField]
	private Text defense;

	[SerializeField]
	private Text support;

	public void Init(HeroCard _hero){

		cost.text = _hero.sds.cost.ToString ();

		hp.text = _hero.sds.hp.ToString ();

		attack.text = _hero.sds.attack.ToString ();

		shoot.text = _hero.sds.shoot.ToString ();

		defense.text = _hero.sds.defense.ToString ();

		support.text = _hero.sds.support.ToString ();

		gameObject.SetActive (true);
	}

	public void Hide(){

		gameObject.SetActive (false);
	}
}

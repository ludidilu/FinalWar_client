using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroDetail : MonoBehaviour {

	[SerializeField]
	private Text cost;

	[SerializeField]
	private Text hp;

	[SerializeField]
	private Text atk;

	[SerializeField]
	private Text shoot;

	[SerializeField]
	private Text def;

	public void Init(HeroCard _hero){

		cost.text = _hero.sds.cost.ToString ();

		hp.text = _hero.sds.hp.ToString ();

		atk.text = _hero.sds.atk.ToString ();

		shoot.text = _hero.sds.shoot.ToString ();

		def.text = _hero.sds.def.ToString ();

		gameObject.SetActive (true);
	}

	public void Hide(){

		gameObject.SetActive (false);
	}
}

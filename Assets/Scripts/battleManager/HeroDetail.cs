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
	private Text counter;

	[SerializeField]
	private Text defense;

	[SerializeField]
	private Text leader;

	private HeroBase hero;

	public void Show(HeroBase _hero){

		hero = _hero;

		cost.text = hero.sds.cost.ToString ();

		hp.text = hero.sds.hp.ToString ();

		attack.text = hero.sds.attack.ToString ();

		shoot.text = hero.sds.shoot.ToString ();

		counter.text = hero.sds.counter.ToString ();
		
		defense.text = hero.sds.defense.ToString ();

		leader.text = hero.sds.leader.ToString ();

		if (!gameObject.activeSelf) {

			gameObject.SetActive (true);
		}
	}

	public void Hide(HeroBase _hero){

		if (hero == _hero) {

			hero = null;

			if (gameObject.activeSelf) {

				gameObject.SetActive (false);
			}
		}
	}

	public void Hide(){
			
		hero = null;
		
		if (gameObject.activeSelf) {
			
			gameObject.SetActive (false);
		}
	}
}

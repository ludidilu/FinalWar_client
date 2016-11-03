using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FinalWar;

public class DamageCalculator : MonoBehaviour {

	[SerializeField]
	private InputField attackerID;

	[SerializeField]
	private InputField attackerPower;

	[SerializeField]
	private InputField attackerHp;

	[SerializeField]
	private InputField defenderID;

	[SerializeField]
	private InputField defenderPower;

	[SerializeField]
	private Text info;

	[SerializeField]
	private Toggle isDefense;

	// Use this for initialization
	void Start () {
	
		ConfigDictionary.Instance.LoadLocalConfig (Application.streamingAssetsPath + "/local.xml");
		
		StaticData.path = ConfigDictionary.Instance.table_path;
		
		StaticData.Dispose ();
		
		StaticData.Load<HeroSDS> ("hero");

		info.text = "";
	}

	public void Attack(){

	}

	public void Shoot(){

	}

	public void Counter(){

	}

	private bool Check(out Hero _attacker,out Hero _defender){

		if(string.IsNullOrEmpty(attackerID.text) || string.IsNullOrEmpty(attackerPower.text) || string.IsNullOrEmpty(attackerHp.text) || string.IsNullOrEmpty(defenderID.text) || string.IsNullOrEmpty(defenderPower.text)){

			_attacker = _defender = null;

			return false;

		}else{

			HeroSDS attackerSDS = StaticData.GetData<HeroSDS>(int.Parse(attackerID.text));
			
			_attacker = new Hero(true,attackerSDS,0,int.Parse(attackerHp.text));
			
			HeroSDS defenderSDS = StaticData.GetData<HeroSDS>(int.Parse(defenderID.text));
			
			_defender = new Hero(false,defenderSDS,0,defenderSDS.hp);

			if(isDefense.isOn){

				_defender.SetAction(Hero.HeroAction.DEFENSE);
			}

			return true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

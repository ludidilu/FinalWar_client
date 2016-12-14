using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateHero : MonoBehaviour {

	[SerializeField]
	private int shield;

	[SerializeField]
	private int hp;

	[SerializeField]
	private int attack;

	[SerializeField]
	private int shooter;

	[SerializeField]
	private int supporter;

	[SerializeField]
	private int defender;

	[SerializeField]
	private int helper;

	[SerializeField]
	private int building;

	[SerializeField]
	private int[] aScore;

	[SerializeField]
	private int[] cScore;

	[SerializeField]
	private int num;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Click(){

		int id = 1000000;

		for (int i = 0; i < num; i++) {

			for(int m = 0 ; m < 5 ; m++){

				int score = aScore[m];


			}
		}
	}
}

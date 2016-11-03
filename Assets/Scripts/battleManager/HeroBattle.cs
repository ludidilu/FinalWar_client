﻿using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superTween;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using FinalWar;

public class HeroBattle : HeroBase {

	private static readonly Color[] POWER_COLORS = new Color[]{

		Color.red,
		Color.yellow,
		Color.green
	};

	[SerializeField]
	public Transform moveTrans;

	[SerializeField]
	public Transform shockTrans;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	protected Text hp;

	[SerializeField]
	protected Text shield;

	private Hero hero;

	public bool isMine{

		get{

			return hero.isMine;
		}
	}

	public int pos{

		get{

			return hero.pos;
		}
	}

	public void Init(int _id){
		
		HeroSDS heroSDS = StaticData.GetData<HeroSDS> (_id);

		InitCard (heroSDS);

		hp.gameObject.SetActive (false);

		shield.gameObject.SetActive (false);

		SetBodyColor ();
	}

	public void Init(Hero _hero){

		hero = _hero;

		InitCard (hero.sds as HeroSDS);

		RefreshHp ();

		RefreshShield();

		SetBodyColor ();
	}

	public void RefreshHp(){

		SetHp (hero.nowHp);
	}

	private void SetHp(int _hp){

		hp.text = _hp.ToString ();
	}

	public void RefreshShield(){

		SetShield(hero.nowShield);
	}

	public void SetShield(int _shield){

		shield.text = _shield.ToString();
	}

	public void Shock(List<Vector3> _targets,AnimationCurve _curve,float _shockDis,int _shieldDamage,int _hpDamage){
		
		Vector3 shockVector = Vector3.zero;
		
		for(int i = 0 ; i < _targets.Count ; i++){
			
			shockVector += (transform.position - _targets[i]).normalized;
		}
		
		if(shockVector == Vector3.zero){
			
			Vector3 v2 = transform.position - _targets[0];
			
			float angle = Mathf.Atan2(v2.y,v2.x);
			
			angle += Mathf.PI * 0.5f;
			
			shockVector = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0) * _shockDis;
			
		}else{
			
			shockVector = shockVector.normalized * _shockDis;
		}
		
		Action<float> shockToDel = delegate(float obj) {
			
			float value = _curve.Evaluate(obj);
			
			shockTrans.position = moveTrans.position - shockVector * value;
		};
		
		SuperTween.Instance.To(0,1,1,shockToDel,null);

		string str = "";

		if(_shieldDamage < 0){

			str += "<color=\"#FFFF00\">" + _shieldDamage + "</color>";

			if(_hpDamage < 0){

				str += "   ";
			}
		}

		if(_hpDamage < 0){

			str += "<color=\"#FF0000\">" + _hpDamage + "</color>";
		}

		ShowHud (str, Color.red, null);

		RefreshShield();

		RefreshHp ();
	}

	public void ShowHud(string _str,Color _color,Action _callBack){

		GameObject go = GameObject.Instantiate<GameObject>(BattleControl.Instance.damageNumResources);
		
		go.transform.SetParent(transform.parent,false);
		
		go.transform.position = transform.position;
		
		DamageNum damageNum = go.GetComponent<DamageNum>();
		
		damageNum.Init(_str,_color,_callBack);
	}

	public void Die(Action _del){

		Action dieOver = delegate() {

			GameObject.Destroy(gameObject);

			if(_del != null){

				_del();
			}
		};

		SuperTween.Instance.To (1, 0, BattleControl.Instance.dieTime, DieTo, dieOver);
	}

	private void DieTo(float _v){

		canvasGroup.alpha = _v;
	}
}

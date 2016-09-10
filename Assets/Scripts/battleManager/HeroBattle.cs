using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superTween;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class HeroBattle : HeroBase {

	[SerializeField]
	public Transform moveTrans;

	[SerializeField]
	public Transform shockTrans;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	public Image body;
	
	[SerializeField]
	protected Text hp;

	[SerializeField]
	protected Text power;

	public int pos = -1;

	private int nowHp;

	private int nowPower;
	
	public bool isMine;
	
	public void Init(int _id,int _hp,int _power,int _pos,bool _isMine){
		
		sds = StaticData.GetData<HeroSDS> (_id);

		SetHp (_hp);

		SetPower (_power);

		pos = _pos;
		
		isMine = _isMine;
	}
	
	public void SetHp(int _hp){

		nowHp = _hp;
		
		hp.text = nowHp.ToString ();
	}

	public void SetPower(int _power){

		nowPower = _power;

		power.text = nowPower.ToString ();
	}

	public void Shock(List<Vector3> _targets,AnimationCurve _curve,float _shockDis,int _damage){
		
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

		GameObject go = GameObject.Instantiate<GameObject>(BattleControl.Instance.damageNumResources);
		
		go.transform.SetParent(transform.parent,false);
		
		go.transform.position = transform.position;
		
		DamageNum damageNum = go.GetComponent<DamageNum>();
		
		damageNum.Init(-_damage,null);
		
		nowHp -= _damage;

		SetHp (nowHp);
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

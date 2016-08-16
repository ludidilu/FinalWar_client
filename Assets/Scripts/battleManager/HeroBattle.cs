using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superTween;
using System;

public class HeroBattle : HeroBase {

	[SerializeField]
	private float hitPercent;

	[SerializeField]
	private float shockDis;

	[SerializeField]
	private AnimationCurve curve;

	[SerializeField]
	private AnimationCurve shockCurve;

	[SerializeField]
	private Transform moveTrans;

	[SerializeField]
	private Transform shockTrans;

	public int pos = -1;
	
	public bool isMine;
	
	public void Init(int _id,int _hp,int _pos,bool _isMine){
		
		sds = StaticData.GetData<HeroSDS> (_id);
		
		hp.text = _hp.ToString ();
		
		pos = _pos;
		
		isMine = _isMine;
	}
	
	public void SetHp(int _hp){
		
		hp.text = _hp.ToString ();
	}

	public void Attack(HeroBattle _target, int _damage, int _damageSelf){

		bool getHit = false;

		Action<float> MoveToDel = delegate(float obj) {

			float value = curve.Evaluate(obj);
			
			moveTrans.position = transform.position + (_target.transform.position - transform.position) * value;

			if(!getHit){

				if(obj > hitPercent){

					getHit = true;

					if(_damage > 0){

						_target.Shock(transform);
					}

					if(_damageSelf > 0){

						Shock(_target.transform);
					}
				}
			}
		};

		SuperTween.Instance.To(0,1,1,MoveToDel,null);
	}

	private void Shock(Transform _target){

		Vector3 shockVector = (_target.transform.position - transform.position).normalized * shockDis;

		Action<float> ShockToDel = delegate(float obj) {

			float value = shockCurve.Evaluate(obj);
			
			shockTrans.position = moveTrans.position - shockVector * value;
		};
		
		SuperTween.Instance.To(0,1,1,ShockToDel,null);
	}
}

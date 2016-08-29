using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superTween;
using System;
using System.Collections.Generic;

public class BattleControl : MonoBehaviour {

	[SerializeField]
	private float hitPercent;

	[SerializeField]
	private float defenderMoveDis;

	[SerializeField]
	private float shockDis;

	[SerializeField]
	public float damageNumGap;

	[SerializeField]
	private AnimationCurve shootCurve;

	[SerializeField]
	private float shootFix;

	[SerializeField]
	private AnimationCurve attackCurve;

	[SerializeField]
	private AnimationCurve shockCurve;

	[SerializeField]
	private GameObject arrowResources;

	[SerializeField]
	public GameObject damageNumResources;

	public static BattleControl Instance{get;private set;}

	void Awake(){

		Instance = this;
	}

	public void Rush(List<HeroBattle> _attackers,HeroBattle _stander,List<int> _damages,Action _callBack){

		bool beShock = false;

		for(int i = 0 ; i < _damages.Count ; i++){

			if(_damages[i] > 0){

				beShock = true;

				break;
			}
		}

		for(int i = 0 ; i < _attackers.Count ; i++){

			int index = i;

			HeroBattle attacker = _attackers[i];

			bool getHit = false;
			
			Action<float> moveToDel = delegate(float obj) {
				
				float value = attackCurve.Evaluate(obj);
				
				attacker.moveTrans.position = Vector3.LerpUnclamped(attacker.transform.position,_stander.transform.position,value);

				if(!getHit && obj > hitPercent){

					getHit = true;
					
					if(beShock && index == 0){

						List<Vector3> vList = new List<Vector3>();

						for(int m = 0 ; m < _attackers.Count ; m++){

							vList.Add(_attackers[m].transform.position);
						}

						_stander.Shock(vList,shockCurve,shockDis,_damages);
					}
				}
			};
			
			SuperTween.Instance.To(0,1,1,moveToDel,null);
		}

		if(beShock){

			SuperTween.Instance.DelayCall(2.5f,_callBack);

		}else{

			SuperTween.Instance.DelayCall(2.0f,_callBack);
		}
	}

	public void Shoot(List<HeroBattle> _shooters,HeroBattle _stander,List<int> _damages,Action _callBack){

		bool beShock = false;
		
		for(int i = 0 ; i < _damages.Count ; i++){
			
			if(_damages[i] > 0){
				
				beShock = true;
				
				break;
			}
		}

		for(int i = 0 ; i < _shooters.Count ; i++){

			int index = i;

			HeroBattle shooter = _shooters[i];

			float angle = Mathf.Atan2(_stander.transform.position.y - shooter.transform.position.y,_stander.transform.position.x - shooter.transform.position.x);
			
			angle += Mathf.PI * 0.5f;
			
			GameObject arrow = GameObject.Instantiate<GameObject>(arrowResources);
			
			arrow.transform.SetParent(shooter.transform.parent,false);
			
			arrow.transform.position = shooter.transform.position;
			
			arrow.SetActive(false);
			
			Action<float> shootToDel = delegate(float obj) {
				
				float v = shootCurve.Evaluate(obj);
				
				if(!arrow.activeSelf){
					
					arrow.SetActive(true);
				}
				
				Vector3 targetPos = Vector3.Lerp(shooter.transform.position,_stander.transform.position,obj);
				
				targetPos += new Vector3(Mathf.Cos(angle) * v * shootFix,Mathf.Sin(angle) * v * shootFix,0);
				
				(arrow.transform as RectTransform).localEulerAngles = new Vector3(0,0,Mathf.Atan2(targetPos.y - arrow.transform.position.y,targetPos.x - arrow.transform.position.x) *180 / Mathf.PI);
				
				arrow.transform.position = targetPos;
			};
			
			Action shootOverDel = delegate() {
				
				GameObject.Destroy(arrow);
				
				if(index == 0 && beShock){

					List<Vector3> vList = new List<Vector3>();

					for(int m = 0 ; m < _shooters.Count ; m++){

						vList.Add(_shooters[m].transform.position);
					}
					
					_stander.Shock(vList,shockCurve,shockDis,_damages);
				}
			};

			SuperTween.Instance.To(0,1,1,shootToDel,shootOverDel);
		}
		
		if(beShock){
			
			SuperTween.Instance.DelayCall(3f,_callBack);
			
		}else{
			
			SuperTween.Instance.DelayCall(2f,_callBack);
		}
	}

	public void Attack(HeroBattle _attacker,Vector3 _targetPos,HeroBattle _defender,HeroBattle _supporter,int _damage,int _damageSelf,Action _callBack){

		bool getHit = false;
		
		Action<float> moveToDel = delegate(float obj) {

			float value = attackCurve.Evaluate(obj);
			
			_attacker.moveTrans.position = Vector3.LerpUnclamped(_attacker.transform.position,_targetPos,value);
			
			if(!getHit && obj > hitPercent){

				getHit = true;
				
				if(_supporter != null){
					
					if(_damage > 0){
						
						_supporter.Shock(new List<Vector3>(){_attacker.transform.position},shockCurve,shockDis,new List<int>(){_damage});
					}

					if(_damageSelf > 0){
						
						_attacker.Shock(new List<Vector3>(){_supporter.transform.position},shockCurve,shockDis,new List<int>(){_damageSelf});
					}
					
				}else{
					
					if(_damage > 0){
						
						_defender.Shock(new List<Vector3>(){_attacker.transform.position},shockCurve,shockDis,new List<int>(){_damage});
					}
					
					if(_damageSelf > 0){
						
						_attacker.Shock(new List<Vector3>(){_defender.transform.position},shockCurve,shockDis,new List<int>(){_damageSelf});
					}
				}
			}
		};
		
		SuperTween.Instance.To(0,1,1,moveToDel,null);
		
		if(_supporter != null){
			
			Vector3 supportPos = _supporter.transform.position;
			
			Action<float> supportToDel = delegate(float obj) {
				
				_supporter.transform.position = Vector3.Lerp(supportPos,_targetPos,obj);
			};
			
			SuperTween.Instance.To(0,1,0.5f,supportToDel,null);
			
			Vector3 defenderMoveTargetPos = Vector3.zero;

			if(_defender != null){
				
				Vector3 attackerV = (_targetPos - _attacker.transform.position).normalized;
				
				Vector3 supportV = (_targetPos - _supporter.transform.position).normalized;
				
				if(attackerV == -supportV){
					
					float angle = Mathf.Atan2(_targetPos.y - _supporter.transform.position.y,_targetPos.x - _supporter.transform.position.x);
					
					angle += Mathf.PI * 0.5f;
					
					defenderMoveTargetPos = new Vector3(_targetPos.x + Mathf.Cos(angle) * defenderMoveDis,_targetPos.y + Mathf.Sin(angle) * defenderMoveDis,_targetPos.z);
					
				}else{
					
					defenderMoveTargetPos = _targetPos + (attackerV + supportV).normalized * defenderMoveDis;
				}
				
				Action<float> defenderToDel = delegate(float obj) {
					
					_defender.transform.position = Vector3.Lerp(_targetPos,defenderMoveTargetPos,obj);
				};
				
				SuperTween.Instance.To(0,1,0.5f,defenderToDel,null);
			}
			
			Action<float> resetToDel = delegate(float obj) {
				
				_supporter.transform.position = Vector3.Lerp(_targetPos,supportPos,obj);
				
				if(_defender != null){
					
					_defender.transform.position = Vector3.Lerp(defenderMoveTargetPos,_targetPos,obj);
				}
			};
			
			Action resetDel = delegate() {
				
				SuperTween.Instance.To(0,1,0.5f,resetToDel,null);
			};
			
			SuperTween.Instance.DelayCall(2,resetDel);
			
			SuperTween.Instance.DelayCall(3,_callBack);
			
		}else{
			
			SuperTween.Instance.DelayCall(2.5f,_callBack);
		}
	}
}

using UnityEngine;
using superTween;
using System;
using System.Collections.Generic;

public class BattleControl : MonoBehaviour
{
    [SerializeField]
    private float hitPercent;

    [SerializeField]
    private float shockDis;

    [SerializeField]
    public float dieTime;

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

    public static BattleControl Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Rush(HeroBattle _attacker, HeroBattle _stander, int _damage, Action _callBack)
    {
        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

			Vector3 vv = Vector3.LerpUnclamped(_attacker.transform.localPosition, _stander.transform.localPosition, value);

			_attacker.moveTrans.localPosition = vv - _attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

				if (_damage < 0)
                {
					_stander.Shock(_attacker.transform.localPosition, shockCurve, shockDis, _damage);
                }
            }
        };

		SuperTween.Instance.To(0, 1, 1, moveToDel, null);

        if (_damage < 0)
        {
            SuperTween.Instance.DelayCall(2.5f, _callBack);
        }
        else
        {
            SuperTween.Instance.DelayCall(2.0f, _callBack);
        }
    }

    public void Shoot(HeroBattle _shooter, HeroBattle _stander, int _damage, Action _callBack)
    {
		float angle = Mathf.Atan2(_stander.transform.localPosition.y - _shooter.transform.localPosition.y, _stander.transform.localPosition.x - _shooter.transform.localPosition.x);

        angle += Mathf.PI * 0.5f;

        GameObject arrow = GameObject.Instantiate<GameObject>(arrowResources);

		arrow.transform.SetParent(_shooter.transform.parent, false);

		arrow.transform.localPosition = _shooter.transform.localPosition;

        arrow.SetActive(false);

        Action<float> shootToDel = delegate (float obj)
        {
            float v = shootCurve.Evaluate(obj);

            if (!arrow.activeSelf)
            {
                arrow.SetActive(true);
            }

            Vector3 targetPos = Vector3.Lerp(_shooter.transform.localPosition, _stander.transform.localPosition, obj);

            targetPos += new Vector3(Mathf.Cos(angle) * v * shootFix, Mathf.Sin(angle) * v * shootFix, 0);

            (arrow.transform as RectTransform).localEulerAngles = new Vector3(0, 0, Mathf.Atan2(targetPos.y - arrow.transform.localPosition.y, targetPos.x - arrow.transform.localPosition.x) * 180 / Mathf.PI);

            arrow.transform.localPosition = targetPos;
        };

        Action shootOverDel = delegate ()
        {
            GameObject.Destroy(arrow);

			if (_damage < 0)
            {
				_stander.Shock(_shooter.transform.localPosition, shockCurve, shockDis, _damage);
            }
        };

        SuperTween.Instance.To(0, 1, 1, shootToDel, shootOverDel);

        if (_damage < 0)
        {
            SuperTween.Instance.DelayCall(3f, _callBack);
        }
        else
        {
            SuperTween.Instance.DelayCall(2f, _callBack);
        }
    }

    public void Attack(List<HeroBattle> _attackers, List<List<HeroBattle>> _helpers, Vector3 _targetPos, HeroBattle _defender, List<HeroBattle> _supporters, int _defenderShieldDamage, int _defenderHpDamage, List<int> _supportersShieldDamage, List<int> _supportersHpDamage, List<int> _attackersShieldDamage, List<int> _attackersHpDamage, Action _callBack)
    {
        Action<float> supportToDel = delegate (float obj)
        {
            for (int i = 0; i < _supporters.Count; i++)
            {
				HeroBattle tmpHero = _supporters[i];

				Vector3 v = Vector3.Lerp(tmpHero.transform.localPosition, _targetPos, obj);

				tmpHero.moveTrans.transform.localPosition = v - tmpHero.transform.localPosition;
            }

			for (int i = 0; i < _helpers.Count; i++)
			{
				List<HeroBattle> tmpList = _helpers[i];
				
				for(int m = 0 ; m < tmpList.Count ; m++){
					
					HeroBattle tmpHero = tmpList[m];
					
					Vector3 v = Vector3.Lerp(tmpHero.transform.localPosition, _attackers[i].transform.localPosition, obj);
					
					tmpHero.moveTrans.transform.localPosition = v - tmpHero.transform.localPosition;
				}
			}
        };

        SuperTween.Instance.To(0, 0.5f, 0.5f, supportToDel, null);

        Action<float> resetToDel = delegate (float obj)
        {
            for (int i = 0; i < _supporters.Count; i++)
            {
				HeroBattle tmpHero = _supporters[i];

				Vector3 v = Vector3.Lerp(_targetPos, tmpHero.transform.localPosition, obj);

				tmpHero.moveTrans.transform.localPosition = v - tmpHero.transform.localPosition;
            }

			for (int i = 0; i < _helpers.Count; i++)
			{
				List<HeroBattle> tmpList = _helpers[i];
				
				for(int m = 0 ; m < tmpList.Count ; m++){
					
					HeroBattle tmpHero = tmpList[m];
					
					Vector3 v = Vector3.Lerp(_attackers[i].transform.localPosition, tmpHero.transform.localPosition, obj);
					
					tmpHero.moveTrans.transform.localPosition = v - tmpHero.transform.localPosition;
				}
			}
        };

        Action resetDel = delegate ()
        {
            SuperTween.Instance.To(0.5f, 1, 0.5f, resetToDel, null);

            SuperTween.Instance.DelayCall(2, _callBack);
        };

        List<Vector3> vList = new List<Vector3>();

        for (int m = 0; m < _attackers.Count; m++)
        {
            vList.Add(_attackers[m].transform.localPosition);
        }

        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            for (int i = 0; i < _attackers.Count; i++)
            {
                HeroBattle attacker = _attackers[i];

				Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, _targetPos, value);

				attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;
            }

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_defender != null && (_defenderShieldDamage < 0 || _defenderHpDamage < 0))
                {
                    _defender.Shock(vList, shockCurve, shockDis, _defenderShieldDamage, _defenderHpDamage);
                }

                for (int i = 0; i < _attackers.Count; i++)
                {
                    if (_attackersShieldDamage[i] < 0 || _attackersHpDamage[i] < 0)
                    {
                        _attackers[i].Shock(new List<Vector3>() { _targetPos }, shockCurve, shockDis, _attackersShieldDamage[i], _attackersHpDamage[i]);
                    }
                }

                for (int i = 0; i < _supporters.Count; i++)
                {
                    if (_supportersShieldDamage[i] < 0 || _supportersHpDamage[i] < 0)
                    {
                        _supporters[i].Shock(vList, shockCurve, shockDis, _supportersShieldDamage[i], _supportersHpDamage[i]);
                    }
                }
            }
        };

        SuperTween.Instance.To(0, 1, 1, moveToDel, resetDel);
    }
}

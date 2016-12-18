using UnityEngine;
using xy3d.tstd.lib.superTween;
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

    public void Rush(List<HeroBattle> _attackers, HeroBattle _stander, int _shieldDamage, int _hpDamage, Action _callBack)
    {
        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            for (int i = 0; i < _attackers.Count; i++)
            {
                HeroBattle attacker = _attackers[i];

				Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, _stander.transform.localPosition, value);

				attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;
            }

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_shieldDamage < 0 || _hpDamage < 0)
                {
                    List<Vector3> vList = new List<Vector3>();

                    for (int m = 0; m < _attackers.Count; m++)
                    {

                        vList.Add(_attackers[m].transform.localPosition);
                    }

                    _stander.Shock(vList, shockCurve, shockDis, _shieldDamage, _hpDamage);
                }
            }
        };

        SuperTween.Instance.To(0, 1, 1, moveToDel, null);

        if (_shieldDamage < 0 || _hpDamage < 0)
        {
            SuperTween.Instance.DelayCall(2.5f, _callBack);
        }
        else
        {
            SuperTween.Instance.DelayCall(2.0f, _callBack);
        }
    }

    public void Shoot(List<HeroBattle> _shooters, HeroBattle _stander, int _shieldDamage, int _hpDamage, Action _callBack)
    {
        GameObject[] arrows = new GameObject[_shooters.Count];

        float[] angles = new float[_shooters.Count];

        for (int i = 0; i < _shooters.Count; i++)
        {
            HeroBattle shooter = _shooters[i];

            float angle = Mathf.Atan2(_stander.transform.localPosition.y - shooter.transform.localPosition.y, _stander.transform.localPosition.x - shooter.transform.localPosition.x);

            angle += Mathf.PI * 0.5f;

            GameObject arrow = GameObject.Instantiate<GameObject>(arrowResources);

            arrow.transform.SetParent(shooter.transform.parent, false);

            arrow.transform.localPosition = shooter.transform.localPosition;

            arrow.SetActive(false);

            arrows[i] = arrow;

            angles[i] = angle;
        }

        Action<float> shootToDel = delegate (float obj)
        {
            float v = shootCurve.Evaluate(obj);

            for (int i = 0; i < arrows.Length; i++)
            {
                GameObject arrow = arrows[i];

                float angle = angles[i];

                if (!arrow.activeSelf)
                {
                    arrow.SetActive(true);
                }

                Vector3 targetPos = Vector3.Lerp(_shooters[i].transform.localPosition, _stander.transform.localPosition, obj);

                targetPos += new Vector3(Mathf.Cos(angle) * v * shootFix, Mathf.Sin(angle) * v * shootFix, 0);

                (arrow.transform as RectTransform).localEulerAngles = new Vector3(0, 0, Mathf.Atan2(targetPos.y - arrow.transform.localPosition.y, targetPos.x - arrow.transform.localPosition.x) * 180 / Mathf.PI);

                arrow.transform.localPosition = targetPos;
            }
        };

        Action shootOverDel = delegate ()
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                GameObject.Destroy(arrows[i]);
            }

            if (_shieldDamage < 0 || _hpDamage < 0)
            {
                List<Vector3> vList = new List<Vector3>();

                for (int m = 0; m < _shooters.Count; m++)
                {

                    vList.Add(_shooters[m].transform.localPosition);
                }

                _stander.Shock(vList, shockCurve, shockDis, _shieldDamage, _hpDamage);
            }
        };

        SuperTween.Instance.To(0, 1, 1, shootToDel, shootOverDel);

        if (_shieldDamage < 0 || _hpDamage < 0)
        {
            SuperTween.Instance.DelayCall(3f, _callBack);
        }
        else
        {
            SuperTween.Instance.DelayCall(2f, _callBack);
        }
    }

    public void Attack(List<HeroBattle> _attackers, Vector3 _targetPos, HeroBattle _defender, List<HeroBattle> _supporters, int _defenderShieldDamage, int _defenderHpDamage, List<int> _supportersShieldDamage, List<int> _supportersHpDamage, List<int> _attackersShieldDamage, List<int> _attackersHpDamage, Action _callBack)
    {
        Action resetDel;

        if (_supporters.Count > 0)
        {
            Vector3[] supportPos = new Vector3[_supporters.Count];

            for (int i = 0; i < _supporters.Count; i++)
            {
                supportPos[i] = _supporters[i].transform.localPosition;
            }

            Action<float> supportToDel = delegate (float obj)
            {
                for (int i = 0; i < _supporters.Count; i++)
                {
                    _supporters[i].transform.localPosition = Vector3.Lerp(supportPos[i], _targetPos, obj);
                }
            };

            SuperTween.Instance.To(0, 0.5f, 0.5f, supportToDel, null);

            Action<float> resetToDel = delegate (float obj)
            {
                for (int i = 0; i < _supporters.Count; i++)
                {
                    _supporters[i].transform.localPosition = Vector3.Lerp(_targetPos, supportPos[i], obj);
                }
            };

            resetDel = delegate ()
            {
                SuperTween.Instance.To(0.5f, 1, 0.5f, resetToDel, null);

                SuperTween.Instance.DelayCall(2, _callBack);
            };
        }
        else
        {
            resetDel = delegate ()
            {
                SuperTween.Instance.DelayCall(1.5f, _callBack);
            };
        }

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

//                attacker.moveTrans.localPosition = Vector3.LerpUnclamped(attacker.transform.localPosition, _targetPos, value);
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

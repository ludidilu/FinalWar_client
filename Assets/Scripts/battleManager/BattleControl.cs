using UnityEngine;
//using superTween;
using System;
using System.Collections.Generic;
using System.Collections;
using superSequenceControl;

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

    public IEnumerator Rush(int _index, HeroBattle _attacker, HeroBattle _stander, int _damage, Action _callBack)
    {
        _attacker.RefreshAttack();

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

        SuperSequenceControl.To(0f, 1f, 1f, moveToDel, _index);

        yield return null;

        if (_damage < 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1.0f, _index);
        }

        yield return null;

        _callBack();
    }

    public IEnumerator Shoot(int _index, HeroBattle _shooter, HeroBattle _stander, int _damage, Action _callBack)
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

        SuperSequenceControl.To(0f, 1f, 1f, shootToDel, _index);

        yield return null;

        Destroy(arrow);

        if (_damage < 0)
        {
            _stander.Shock(_shooter.transform.localPosition, shockCurve, shockDis, _damage);
        }

        if (_damage < 0)
        {
            SuperSequenceControl.DelayCall(2f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        _callBack();
    }

    public IEnumerator PrepareAttack(int _index, Vector3 _pos, HeroBattle _attacker, HeroBattle _defender, HeroBattle _supporter, List<HeroBattle> _attackerSupporters, List<HeroBattle> _defenderSupporters, int _attackerSpeed, int _defenderSpeed, Action _del)
    {
        HeroBattle defenderReal;

        if (_supporter != null)
        {
            defenderReal = _supporter;
        }
        else
        {
            defenderReal = _defender;
        }

        Action<float> supportersMove = null;

        if (_attackerSupporters != null || _defenderSupporters != null)
        {
            supportersMove = delegate (float obj)
            {
                if (_attackerSupporters != null)
                {
                    for (int i = 0; i < _attackerSupporters.Count; i++)
                    {
                        HeroBattle hero = _attackerSupporters[i];

                        Vector3 v = Vector3.Lerp(hero.transform.localPosition, _attacker.transform.localPosition, obj);

                        hero.moveTrans.transform.localPosition = v - hero.transform.localPosition;
                    }
                }

                if (_defenderSupporters != null)
                {
                    for (int i = 0; i < _defenderSupporters.Count; i++)
                    {
                        HeroBattle hero = _defenderSupporters[i];

                        Vector3 v = Vector3.Lerp(hero.transform.localPosition, defenderReal.transform.localPosition, obj);

                        hero.moveTrans.transform.localPosition = v - hero.transform.localPosition;
                    }
                }
            };

            SuperSequenceControl.To(0, 0.5f, 0.5f, supportersMove, _index);

            yield return null;
        }

        Action dele0 = delegate ()
        {
            SuperSequenceControl.MoveNext(_index);
        };

        _attacker.ShowHud("aa", Color.blue, dele0);

        defenderReal.ShowHud("bb", Color.blue, null);

        yield return null;

        if (defenderReal == _supporter)
        {
            if (supportersMove != null)
            {
                SuperSequenceControl.To(0.5f, 0f, 0.5f, supportersMove, 0);
            }

            if (_defender != null)
            {
                Vector3 v1 = (_defender.transform.localPosition - _attacker.transform.localPosition).normalized;

                Vector3 v2 = (_defender.transform.localPosition - _supporter.transform.localPosition).normalized;

                Vector3 v3 = v1 + v2;

                Vector3 targetPos = _defender.transform.localPosition + v3.normalized * Vector3.Distance(_defender.transform.localPosition, _attacker.transform.localPosition) * 0.5f;

                Action<float> defenderToDel = delegate (float _value)
                {
                    Vector3 v = Vector3.Lerp(_defender.transform.localPosition, targetPos, _value);

                    _defender.moveTrans.localPosition = v - _defender.transform.localPosition;
                };

                SuperSequenceControl.To(0f, 1f, 0.5f, defenderToDel, 0);
            }

            Action<float> supporterToDel = delegate (float _value)
            {
                Vector3 v = Vector3.Lerp(_supporter.transform.localPosition, _pos, _value);

                _supporter.moveTrans.localPosition = v - _supporter.transform.localPosition;
            };

            SuperSequenceControl.To(0f, 1f, 0.5f, supporterToDel, _index);

            yield return null;
        }
        else
        {
            if (supportersMove != null)
            {
                SuperSequenceControl.To(0.5f, 0f, 0.5f, supportersMove, _index);

                yield return null;
            }
        }

        _del();
    }

    public IEnumerator Attack(int _index, Vector3 _targetPos, HeroBattle _attacker, HeroBattle _defender, int _damage, Action _callBack)
    {
        _attacker.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(_attacker.transform.localPosition, _targetPos, value);

            _attacker.moveTrans.localPosition = vv - _attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_damage < 0)
                {
                    _defender.Shock(_attacker.transform.localPosition, shockCurve, shockDis, _damage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_damage < 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        _callBack();
    }

    public IEnumerator AttackAndCounter(int _index, Vector3 _targetPos, HeroBattle _attacker, HeroBattle _defender, int _attackDamage, int _defenseDamage, Action _callBack)
    {
        _attacker.RefreshAttack();

        _defender.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(_attacker.transform.localPosition, _targetPos, value);

            _attacker.moveTrans.localPosition = vv - _attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_attackDamage < 0)
                {
                    _defender.Shock(_attacker.transform.localPosition, shockCurve, shockDis, _attackDamage);
                }

                if (_defenseDamage < 0)
                {
                    _attacker.Shock(_targetPos, shockCurve, shockDis, _defenseDamage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_attackDamage < 0 || _defenseDamage < 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        _callBack();
    }

    public IEnumerator Counter(int _index, Vector3 _targetPos, HeroBattle _attacker, HeroBattle _defender, int _damage, Action _callBack)
    {
        _attacker.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(_targetPos, _defender.transform.localPosition, value);

            _attacker.moveTrans.localPosition = vv - _attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_damage < 0)
                {
                    _defender.Shock(_attacker.transform.localPosition, shockCurve, shockDis, _damage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_damage < 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        _callBack();
    }
}

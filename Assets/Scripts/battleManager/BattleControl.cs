using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using superSequenceControl;
using FinalWar;

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

    public IEnumerator Rush(int _index, int _lastIndex, BattleRushVO _vo)
    {
        HeroBattle attacker = BattleManager.Instance.heroDic[_vo.attacker];

        HeroBattle stander = BattleManager.Instance.heroDic[_vo.stander];

        attacker.RefreshAttack();

        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, stander.transform.localPosition, value);

            attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_vo.damage != 0)
                {
                    stander.Shock(attacker.transform.localPosition, shockCurve, shockDis, _vo.damage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, moveToDel, _index);

        yield return null;

        if (_vo.damage != 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1.0f, _index);
        }

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Shoot(int _index, int _lastIndex, BattleShootVO _vo)
    {
        HeroBattle shooter = BattleManager.Instance.heroDic[_vo.shooter];

        HeroBattle stander = BattleManager.Instance.heroDic[_vo.stander];

        float angle = Mathf.Atan2(stander.transform.localPosition.y - shooter.transform.localPosition.y, stander.transform.localPosition.x - shooter.transform.localPosition.x);

        angle += Mathf.PI * 0.5f;

        GameObject arrow = GameObject.Instantiate<GameObject>(arrowResources);

        arrow.transform.SetParent(shooter.transform.parent, false);

        arrow.transform.localPosition = shooter.transform.localPosition;

        arrow.SetActive(false);

        Action<float> shootToDel = delegate (float obj)
        {
            float v = shootCurve.Evaluate(obj);

            if (!arrow.activeSelf)
            {
                arrow.SetActive(true);
            }

            Vector3 targetPos = Vector3.Lerp(shooter.transform.localPosition, stander.transform.localPosition, obj);

            targetPos += new Vector3(Mathf.Cos(angle) * v * shootFix, Mathf.Sin(angle) * v * shootFix, 0);

            (arrow.transform as RectTransform).localEulerAngles = new Vector3(0, 0, Mathf.Atan2(targetPos.y - arrow.transform.localPosition.y, targetPos.x - arrow.transform.localPosition.x) * 180 / Mathf.PI);

            arrow.transform.localPosition = targetPos;
        };

        SuperSequenceControl.To(0f, 1f, 1f, shootToDel, _index);

        yield return null;

        Destroy(arrow);

		if (_vo.damage != 0) {
			
			stander.Shock (shooter.transform.localPosition, shockCurve, shockDis, _vo.damage);

			SuperSequenceControl.DelayCall (1.5f, _index);

		} else {

			SuperSequenceControl.DelayCall(1f, _index);
		}

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator PrepareAttack(int _index, int _lastIndex, BattlePrepareAttackVO _vo)
    {
        Vector3 targetPos = BattleManager.Instance.mapUnitDic[_vo.pos].transform.localPosition;

        HeroBattle attacker = BattleManager.Instance.heroDic[_vo.attacker];

        HeroBattle defenderReal;

        HeroBattle defender = null;

        HeroBattle supporter = null;

        if (_vo.pos == _vo.defender)
        {
            defenderReal = defender = BattleManager.Instance.heroDic[_vo.defender];
        }
        else
        {
            defenderReal = supporter = BattleManager.Instance.heroDic[_vo.defender];

            BattleManager.Instance.heroDic.TryGetValue(_vo.pos, out defender);
        }

        List<HeroBattle> attackerSupporters = null;

        if (_vo.attackerSupperters != null)
        {
            attackerSupporters = new List<HeroBattle>();

            for (int i = 0; i < _vo.attackerSupperters.Count; i++)
            {
                attackerSupporters.Add(BattleManager.Instance.heroDic[_vo.attackerSupperters[i]]);
            }
        }

        List<HeroBattle> defenderSupporters = null;

        if (_vo.defenderSupporters != null)
        {
            defenderSupporters = new List<HeroBattle>();

            for (int i = 0; i < _vo.defenderSupporters.Count; i++)
            {
                defenderSupporters.Add(BattleManager.Instance.heroDic[_vo.defenderSupporters[i]]);
            }
        }

        Action<float> supportersMove = null;

        if (attackerSupporters != null || defenderSupporters != null)
        {
            supportersMove = delegate (float obj)
            {
                if (attackerSupporters != null)
                {
                    for (int i = 0; i < attackerSupporters.Count; i++)
                    {
                        HeroBattle hero = attackerSupporters[i];

                        Vector3 v = Vector3.Lerp(hero.transform.localPosition, attacker.transform.localPosition, obj);

                        hero.moveTrans.transform.localPosition = v - hero.transform.localPosition;
                    }
                }

                if (defenderSupporters != null)
                {
                    for (int i = 0; i < defenderSupporters.Count; i++)
                    {
                        HeroBattle hero = defenderSupporters[i];

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

        attacker.ShowHud("aa", Color.blue, dele0);

        defenderReal.ShowHud("bb", Color.blue, null);

        yield return null;

        if (defenderReal == supporter)
        {
            if (supportersMove != null)
            {
                SuperSequenceControl.To(0.5f, 0f, 0.5f, supportersMove, 0);
            }

            if (defender != null)
            {
                Vector3 v1 = (defender.transform.localPosition - attacker.transform.localPosition).normalized;

                Vector3 v2 = (defender.transform.localPosition - supporter.transform.localPosition).normalized;

                Vector3 v3 = v1 + v2;

                Vector3 tmpPos = defender.transform.localPosition + v3.normalized * Vector3.Distance(defender.transform.localPosition, attacker.transform.localPosition) * 0.5f;

                Action<float> defenderToDel = delegate (float _value)
                {
                    Vector3 v = Vector3.Lerp(defender.transform.localPosition, tmpPos, _value);

                    defender.moveTrans.localPosition = v - defender.transform.localPosition;
                };

                SuperSequenceControl.To(0f, 1f, 0.5f, defenderToDel, 0);
            }

            Action<float> supporterToDel = delegate (float _value)
            {
                Vector3 v = Vector3.Lerp(supporter.transform.localPosition, targetPos, _value);

                supporter.moveTrans.localPosition = v - supporter.transform.localPosition;
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

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Attack(int _index, int _lastIndex, BattleAttackVO _vo)
    {
        HeroBattle attacker = BattleManager.Instance.heroDic[_vo.attacker];

        HeroBattle defender = BattleManager.Instance.heroDic[_vo.defender];

        Vector3 targetPos = BattleManager.Instance.mapUnitDic[_vo.pos].transform.localPosition;

        attacker.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, targetPos, value);

            attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_vo.damage != 0)
                {
                    defender.Shock(attacker.transform.localPosition, shockCurve, shockDis, _vo.damage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_vo.damage != 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        attacker.RefreshAttackWithoutShield();

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator AttackAndCounter(int _index, int _lastIndex, BattleAttackAndCounterVO _vo)
    {
        HeroBattle attacker = BattleManager.Instance.heroDic[_vo.attacker];

        HeroBattle defender = BattleManager.Instance.heroDic[_vo.defender];

        Vector3 targetPos = BattleManager.Instance.mapUnitDic[_vo.pos].transform.localPosition;

        attacker.RefreshAttack();

        defender.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, targetPos, value);

            attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_vo.attackDamage != 0)
                {
                    defender.Shock(attacker.transform.localPosition, shockCurve, shockDis, _vo.attackDamage);
                }

                if (_vo.defenseDamage != 0)
                {
                    attacker.Shock(targetPos, shockCurve, shockDis, _vo.defenseDamage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_vo.attackDamage != 0 || _vo.defenseDamage != 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        attacker.RefreshAttackWithoutShield();

        defender.RefreshAttackWithoutShield();

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Counter(int _index, int _lastIndex, BattleCounterVO _vo)
    {
        Vector3 targetPos = BattleManager.Instance.mapUnitDic[_vo.pos].transform.localPosition;

        HeroBattle attacker = BattleManager.Instance.heroDic[_vo.attacker];

        HeroBattle defender = BattleManager.Instance.heroDic[_vo.defender];

        attacker.RefreshAttack();

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(targetPos, defender.transform.localPosition, value);

            attacker.moveTrans.localPosition = vv - attacker.transform.localPosition;

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                if (_vo.damage != 0)
                {
                    defender.Shock(attacker.transform.localPosition, shockCurve, shockDis, _vo.damage);
                }
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        if (_vo.damage != 0)
        {
            SuperSequenceControl.DelayCall(1.5f, _index);
        }
        else
        {
            SuperSequenceControl.DelayCall(1f, _index);
        }

        yield return null;

        attacker.RefreshAttackWithoutShield();

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Move(int _index, int _lastIndex, BattleMoveVO _vo)
    {
        List<KeyValuePair<int, int>> tmpList = new List<KeyValuePair<int, int>>();

        List<KeyValuePair<int, int>> tmpList2 = new List<KeyValuePair<int, int>>();

        Dictionary<int, HeroBattle> tmpDic = new Dictionary<int, HeroBattle>();

        Dictionary<int, int> moves = _vo.moves;

        while (moves.Count > 0)
        {
            Dictionary<int, int>.Enumerator enumerator = moves.GetEnumerator();

            enumerator.MoveNext();

            tmpList.Add(enumerator.Current);

            while (tmpList.Count > 0)
            {
                KeyValuePair<int, int> pair = tmpList[0];

                moves.Remove(pair.Key);

                tmpList.RemoveAt(0);

                tmpList2.Add(pair);

                if (moves.ContainsKey(pair.Value))
                {
                    tmpList.Add(new KeyValuePair<int, int>(pair.Value, moves[pair.Value]));
                }

                if (moves.ContainsValue(pair.Key))
                {
                    enumerator = moves.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Value == pair.Key)
                        {
                            tmpList.Add(enumerator.Current);

                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < tmpList2.Count; i++)
            {
                KeyValuePair<int, int> pair = tmpList2[i];

                HeroBattle hero = BattleManager.Instance.heroDic[pair.Key];

                tmpDic.Add(pair.Key, hero);

                BattleManager.Instance.heroDic.Remove(pair.Key);

                Vector3 startPos = BattleManager.Instance.mapUnitDic[pair.Key].transform.localPosition;

                Vector3 endPos = BattleManager.Instance.mapUnitDic[pair.Value].transform.localPosition;

                Action<float> toDel = delegate (float obj)
                {
                    hero.transform.localPosition = Vector3.Lerp(startPos, endPos, obj);
                };

                if (i == 0)
                {
                    SuperSequenceControl.To(0f, 1f, 1f, toDel, _index);
                }
                else
                {
                    SuperSequenceControl.To(0f, 1f, 1f, toDel, 0);
                }
            }

            yield return null;

            for (int l = 0; l < tmpList2.Count; l++)
            {
                KeyValuePair<int, int> pair = tmpList2[l];

                BattleManager.Instance.heroDic.Add(pair.Value, tmpDic[pair.Key]);

                int index = pair.Value;

                MapUnit unit = BattleManager.Instance.mapUnitDic[index];

                BattleManager.Instance.SetMapUnitColor(unit);
            }

            tmpList.Clear();

            tmpList2.Clear();

            tmpDic.Clear();
        }

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Die(int _index, int _lastIndex, BattleDeathVO _vo)
    {
        Action dele = delegate ()
        {
            SuperSequenceControl.MoveNext(_index);
        };

        for (int i = 0; i < _vo.deads.Count; i++)
        {
            int pos = _vo.deads[i];

            HeroBattle hero = BattleManager.Instance.heroDic[pos];

            BattleManager.Instance.heroDic.Remove(pos);

            if (i == 0)
            {
                hero.Die(dele);
            }
            else
            {
                hero.Die(null);
            }
        }

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }
    
}

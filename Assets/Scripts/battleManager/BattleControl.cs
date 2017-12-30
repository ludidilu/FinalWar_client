using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using superSequenceControl;
using FinalWar;
using System.Linq;
using gameObjectFactory;

public class BattleControl : MonoBehaviour
{
    [SerializeField]
    private BattleManager battleManager;

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
    public float hudHeight;

    [SerializeField]
    public Material mat;

    [SerializeField]
    public Material matGray;

    [SerializeField]
    public Sprite[] typeSprite;

    [SerializeField]
    public Sprite frame;

    [SerializeField]
    public Sprite frameChoose;

    [SerializeField]
    public float zFixStep;

    [SerializeField]
    public float moveAwayDistance;

    public IEnumerator Rush(int _index, int _lastIndex, BattleRushVO _vo)
    {
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, stander.transform.localPosition, value);

            attacker.moveTrans.localPosition = attacker.hudTrans.InverseTransformPoint(attacker.transform.parent.TransformPoint(vv));

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, moveToDel, _index);

        yield return null;

        bool shock = stander.TakeEffect(new List<BattleHeroEffectVO>() { _vo.vo });

        if (shock)
        {
            stander.Shock(attacker, shockCurve, shockDis);
        }

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _lastIndex);
    }

    public IEnumerator Shoot(int _index, int _lastIndex, BattleShootVO _vo)
    {
        HeroBattle shooter = battleManager.heroDic[_vo.shooter];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        float angle = Mathf.Atan2(stander.transform.localPosition.y - shooter.transform.localPosition.y, stander.transform.localPosition.x - shooter.transform.localPosition.x);

        angle += Mathf.PI * 0.5f;

        GameObject arrow = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/DamageArrow.prefab", null);

        arrow.transform.SetParent(battleManager.arrowContainer, false);

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

            arrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetPos.y - arrow.transform.localPosition.y, targetPos.x - arrow.transform.localPosition.x) * Mathf.Rad2Deg);

            arrow.transform.localPosition = targetPos;
        };

        SuperSequenceControl.To(0f, 1f, 1f, shootToDel, _index);

        yield return null;

        Destroy(arrow);

        bool shock = stander.TakeEffect(_vo.effectList);

        if (shock)
        {
            stander.Shock(shooter, shockCurve, shockDis);
        }

        SuperSequenceControl.DelayCall(2.0f, _lastIndex);
    }

    public IEnumerator Support(int _index, int _lastIndex, BattleSupportVO _vo)
    {
        Vector3 targetPos = battleManager.mapUnitDic[_vo.stander].transform.localPosition;

        HeroBattle supporter = battleManager.heroDic[_vo.supporter];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        Action<float> supporterToDel = delegate (float _value)
        {
            Vector3 v = Vector3.Lerp(supporter.transform.localPosition, targetPos, _value);

            supporter.hudTrans.localPosition = supporter.transform.InverseTransformPoint(supporter.transform.parent.TransformPoint(v));
        };

        SuperSequenceControl.To(0f, 0.5f, 0.5f, supporterToDel, _index);

        yield return null;

        bool shock = stander.TakeEffect(_vo.effectList);

        if (shock)
        {
            stander.Shock(supporter, shockCurve, shockDis);
        }

        SuperSequenceControl.DelayCall(0.5f, _index);

        yield return null;

        SuperSequenceControl.To(0.5f, 0f, 0.5f, supporterToDel, _lastIndex);
    }

    public IEnumerator PrepareAttack(int _index, int _lastIndex, BattlePrepareAttackVO _vo)
    {
        Vector3 targetPos = battleManager.mapUnitDic[_vo.pos].transform.localPosition;

        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle defenderReal;

        HeroBattle defender = null;

        HeroBattle supporter = null;

        if (_vo.pos == _vo.defender)
        {
            defenderReal = defender = battleManager.heroDic[_vo.defender];
        }
        else
        {
            defenderReal = supporter = battleManager.heroDic[_vo.defender];

            battleManager.heroDic.TryGetValue(_vo.pos, out defender);
        }

        if (defenderReal == supporter)
        {
            if (defender != null)
            {
                Vector3 v1 = (defender.transform.localPosition - attacker.transform.localPosition).normalized;

                Vector3 v2 = (defender.transform.localPosition - supporter.transform.localPosition).normalized;

                Vector3 v3 = v1 + v2;

                if (v3 == Vector3.zero)
                {
                    v3 = (Quaternion.AngleAxis(90, Vector3.forward) * v1);
                }

                Vector3 tmpPos = defender.transform.localPosition + v3.normalized * Vector3.Distance(defender.transform.localPosition, attacker.transform.localPosition) * moveAwayDistance;

                Action<float> defenderToDel = delegate (float _value)
                {
                    Vector3 v = Vector3.Lerp(defender.transform.localPosition, tmpPos, _value);

                    defender.hudTrans.localPosition = defender.transform.InverseTransformPoint(defender.transform.parent.TransformPoint(v));
                };

                SuperSequenceControl.To(0f, 1f, 0.5f, defenderToDel, 0);
            }

            Action<float> supporterToDel = delegate (float _value)
            {
                Vector3 v = Vector3.Lerp(supporter.transform.localPosition, targetPos, _value);

                supporter.hudTrans.localPosition = supporter.transform.InverseTransformPoint(supporter.transform.parent.TransformPoint(v));
            };

            SuperSequenceControl.To(0f, 1f, 0.5f, supporterToDel, _index);

            yield return null;
        }

        Action dele0 = delegate ()
        {
            SuperSequenceControl.MoveNext(_index);
        };

        attacker.ShowHud(_vo.attackerSpeed.ToString(), Color.grey, Color.red, 0, dele0);

        defenderReal.ShowHud(_vo.defenderSpeed.ToString(), Color.grey, Color.red, 0, null);

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator AttackAndCounter(int _index, int _lastIndex, BattleAttackAndCounterVO _vo)
    {
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle defender = battleManager.heroDic[_vo.defender];

        Vector3 targetPos = battleManager.mapUnitDic[_vo.pos].transform.localPosition;

        if (_vo.attackerShield)
        {
            attacker.RefreshAttack();
        }
        else
        {
            attacker.RefreshAttackWithoutShield();
        }

        if (_vo.defenderShield)
        {
            defender.RefreshAttack();
        }
        else
        {
            defender.RefreshAttackWithoutShield();
        }

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, targetPos, value);

            attacker.moveTrans.localPosition = attacker.hudTrans.InverseTransformPoint(attacker.transform.parent.TransformPoint(vv));

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        bool defenderShock = defender.TakeEffect(new List<BattleHeroEffectVO>() { _vo.attackVO });

        if (defenderShock)
        {
            defender.Shock(attacker, shockCurve, shockDis);
        }

        bool attackerShock = attacker.TakeEffect(new List<BattleHeroEffectVO>() { _vo.defenseVO });

        if (attackerShock)
        {
            attacker.Shock(defender, shockCurve, shockDis);
        }

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _index);

        yield return null;

        attacker.RefreshAttackWithoutShield();

        defender.RefreshAttackWithoutShield();

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator AttackBoth(int _index, int _lastIndex, BattleAttackBothVO _vo)
    {
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle defender = battleManager.heroDic[_vo.defender];

        Vector3 attackPos = battleManager.mapUnitDic[_vo.defender].transform.localPosition;

        Vector3 defensePos = battleManager.mapUnitDic[_vo.attacker].transform.localPosition;

        Vector3 targetPos = Vector3.Lerp(attackPos, defensePos, 0.5f);

        if (_vo.attackerShield)
        {
            attacker.RefreshAttack();
        }
        else
        {
            attacker.RefreshAttackWithoutShield();
        }

        if (_vo.defenderShield)
        {
            defender.RefreshAttack();
        }
        else
        {
            defender.RefreshAttackWithoutShield();
        }

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector3 vv = Vector3.LerpUnclamped(attacker.transform.localPosition, targetPos, value);

            attacker.moveTrans.localPosition = attacker.hudTrans.InverseTransformPoint(attacker.transform.parent.TransformPoint(vv));

            vv = Vector3.LerpUnclamped(defender.transform.localPosition, targetPos, value);

            defender.moveTrans.localPosition = defender.hudTrans.InverseTransformPoint(defender.transform.parent.TransformPoint(vv));

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        bool defenderShock = defender.TakeEffect(new List<BattleHeroEffectVO>() { _vo.attackVO });

        if (defenderShock)
        {
            defender.Shock(attacker, shockCurve, shockDis);
        }

        bool attackerShock = attacker.TakeEffect(new List<BattleHeroEffectVO>() { _vo.defenseVO });

        if (attackerShock)
        {
            attacker.Shock(defender, shockCurve, shockDis);
        }

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _index);

        yield return null;

        attacker.RefreshAttackWithoutShield();

        defender.RefreshAttackWithoutShield();

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator AttackOver(int _index, int _lastIndex, BattleAttackOverVO _vo)
    {
        if (_vo.defender != _vo.pos)
        {
            HeroBattle supporter = battleManager.heroDic[_vo.defender];

            Vector3 startPos = supporter.hudTrans.localPosition;

            Action<float> dele = delegate (float _value)
            {
                Vector3 v = Vector3.Lerp(startPos, Vector3.zero, _value);

                supporter.hudTrans.localPosition = v;
            };

            SuperSequenceControl.To(0f, 1f, 0.5f, dele, _index);

            HeroBattle defender;

            if (battleManager.heroDic.TryGetValue(_vo.pos, out defender))
            {
                Vector3 startPos2 = defender.hudTrans.localPosition;

                Action<float> dele2 = delegate (float _value)
                {
                    Vector3 v = Vector3.Lerp(startPos2, Vector3.zero, _value);

                    defender.hudTrans.localPosition = v;
                };

                SuperSequenceControl.To(0f, 1f, 0.5f, dele2, 0);
            }

            yield return null;

            SuperSequenceControl.DelayCall(0.5f, _index);

            yield return null;
        }

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
            tmpList.Add(moves.ElementAt(0));

            while (tmpList.Count > 0)
            {
                KeyValuePair<int, int> pair = tmpList[0];

                moves.Remove(pair.Key);

                tmpList.RemoveAt(0);

                tmpList2.Add(pair);

                int tmpIndex;

                if (moves.TryGetValue(pair.Value, out tmpIndex))
                {
                    tmpList.Add(new KeyValuePair<int, int>(pair.Value, tmpIndex));

                    moves.Remove(pair.Value);
                }

                Dictionary<int, int>.Enumerator enumerator = moves.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value == pair.Key)
                    {
                        tmpList.Add(enumerator.Current);

                        moves.Remove(enumerator.Current.Key);

                        break;
                    }
                }
            }

            for (int i = 0; i < tmpList2.Count; i++)
            {
                KeyValuePair<int, int> pair = tmpList2[i];

                HeroBattle hero = battleManager.heroDic[pair.Key];

                tmpDic.Add(pair.Key, hero);

                battleManager.heroDic.Remove(pair.Key);

                Vector3 startPos = battleManager.mapUnitDic[pair.Key].transform.localPosition;

                Vector3 endPos = battleManager.mapUnitDic[pair.Value].transform.localPosition;

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

                HeroBattle hero = tmpDic[pair.Key];

                battleManager.heroDic.Add(pair.Value, hero);

                hero.RefreshAll();

                int index = pair.Value;

                MapUnit unit = battleManager.mapUnitDic[index];

                battleManager.SetMapUnitColor(unit);
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

            HeroBattle hero = battleManager.heroDic[pos];

            battleManager.heroDic.Remove(pos);

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

    public IEnumerator TriggerAura(int _index, int _lastIndex, BattleTriggerAuraVO _vo)
    {
        HeroBattle hero = battleManager.heroDic[_vo.pos];

        Action<float> dele = delegate (float _value)
        {
            hero.moveTrans.localRotation = Quaternion.Euler(0, 0, _value);
        };

        SuperSequenceControl.To(0, 720, 0.5f, dele, _index);

        yield return null;

        if (_vo.data != null)
        {
            Dictionary<int, BattleHeroEffectVO>.Enumerator enumerator = _vo.data.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HeroBattle targetHero = battleManager.heroDic[enumerator.Current.Key];

                targetHero.RefreshAttackWithoutShield();

                bool shock = targetHero.TakeEffect(new List<BattleHeroEffectVO>() { enumerator.Current.Value });

                if (shock)
                {
                    targetHero.Shock(hero, shockCurve, shockDis);
                }
            }

            SuperSequenceControl.DelayCall(2.0f, _lastIndex);
        }
        else
        {
            SuperSequenceControl.MoveNext(_lastIndex);
        }
    }
}

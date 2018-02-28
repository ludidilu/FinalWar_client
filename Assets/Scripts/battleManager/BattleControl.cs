using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using superSequenceControl;
using FinalWar;
using System.Linq;
using gameObjectFactory;
using publicTools;

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
    private AnimationCurve cameraMoveCurve;

    [SerializeField]
    private float cameraMoveSpeedFix;

    [SerializeField]
    public float hudHeight;

    [SerializeField]
    public Material mat;

    [SerializeField]
    public Material matGray;

    [SerializeField]
    public Sprite frame;

    [SerializeField]
    public Sprite frameChoose;

    [SerializeField]
    public float zFixStep;

    [SerializeField]
    public float moveAwayDistance;

    [SerializeField]
    public float attackMoveDistance;

    [SerializeField]
    private float mapTargetScale = 1.3f;

    [SerializeField]
    private float unitTargetScale = 0.5f;

    [SerializeField]
    private float bgAlpha = 0.3f;

    public IEnumerator Rush(int _index, int _lastIndex, BattleRushVO _vo)
    {
        MoveCamera(_index, _vo.attacker, _vo.stander);

        yield return null;

        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        bool getHit = false;

        Action<float> moveToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector2 v = Vector2.LerpUnclamped(attacker.transform.position, stander.transform.position, value);

            attacker.moveTrans.position = new Vector3(v.x, v.y, attacker.moveTrans.position.z);

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, moveToDel, _index);

        yield return null;

        HeroBeDamaged(attacker, stander);

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _lastIndex);
    }

    public IEnumerator Shoot(int _index, int _lastIndex, BattleShootVO _vo)
    {
        MoveCamera(_index, _vo.shooter, _vo.stander);

        yield return null;

        HeroBattle shooter = battleManager.heroDic[_vo.shooter];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        float angle = Mathf.Atan2(stander.transform.position.y - shooter.transform.position.y, stander.transform.position.x - shooter.transform.position.x);

        angle += Mathf.PI * 0.5f;

        GameObject arrow = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/DamageArrow.prefab", null);

        arrow.transform.SetParent(battleManager.arrowContainer, false);

        arrow.transform.position = shooter.transform.position;

        arrow.SetActive(false);

        Action<float> shootToDel = delegate (float obj)
        {
            float v = shootCurve.Evaluate(obj);

            if (!arrow.activeSelf)
            {
                arrow.SetActive(true);
            }

            Vector2 targetPos = Vector2.Lerp(shooter.transform.position, stander.transform.position, obj);

            targetPos += new Vector2(Mathf.Cos(angle) * v * shootFix, Mathf.Sin(angle) * v * shootFix);

            arrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetPos.y - arrow.transform.position.y, targetPos.x - arrow.transform.position.x) * Mathf.Rad2Deg);

            arrow.transform.position = targetPos;
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
        MoveCamera(_index, _vo.supporter, _vo.stander);

        yield return null;

        HeroBattle supporter = battleManager.heroDic[_vo.supporter];

        HeroBattle stander = battleManager.heroDic[_vo.stander];

        Action<float> supporterToDel = delegate (float _value)
        {
            Vector2 v = Vector2.Lerp(supporter.transform.position, stander.transform.position, _value);

            supporter.hudTrans.position = new Vector3(v.x, v.y, supporter.hudTrans.position.z);
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
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle defenderReal;

        HeroBattle defender = null;

        HeroBattle supporter = null;

        if (_vo.attackType != AttackType.A_S)
        {
            MoveCamera(_index, _vo.attacker, _vo.defender);

            yield return null;

            PrepareAttack(_index, _vo.attacker, _vo.defender);

            defenderReal = defender = battleManager.heroDic[_vo.defender];
        }
        else
        {
            MoveCamera(_index, _vo.attacker, _vo.defender, _vo.pos);

            yield return null;

            PrepareAttack(_index, _vo.attacker, _vo.defender, _vo.pos);

            defenderReal = supporter = battleManager.heroDic[_vo.defender];

            battleManager.heroDic.TryGetValue(_vo.pos, out defender);
        }

        yield return null;

        Vector3 targetPos = battleManager.mapUnitDic[_vo.pos].transform.position;

        Vector2 attackPos0 = Vector2.Lerp(attacker.transform.position, targetPos, attackMoveDistance);

        Action<float> dele = delegate (float _value)
        {
            Vector2 v = Vector2.Lerp(attacker.transform.position, attackPos0, _value);

            attacker.hudTrans.position = new Vector3(v.x, v.y, attacker.hudTrans.position.z);
        };

        if (_vo.attackType == AttackType.A_S)
        {
            if (defender != null)
            {
                Vector3 v1 = (defender.transform.position - attacker.transform.position).normalized;

                Vector3 v2 = (defender.transform.position - supporter.transform.position).normalized;

                Vector3 v3 = v1 + v2;

                if (v3 == Vector3.zero)
                {
                    v3 = (Quaternion.AngleAxis(90, Vector3.forward) * v1);
                }

                Vector3 tmpPos = defender.transform.position + v3.normalized * Vector3.Distance(defender.transform.position, attacker.transform.position) * moveAwayDistance;

                dele += delegate (float _value)
                {
                    Vector2 v = Vector2.Lerp(defender.transform.position, tmpPos, _value);

                    defender.hudTrans.position = new Vector3(v.x, v.y, defender.hudTrans.position.z);
                };
            }

            dele += delegate (float _value)
            {
                Vector2 v = Vector2.Lerp(supporter.transform.position, targetPos, _value);

                supporter.hudTrans.position = new Vector3(v.x, v.y, supporter.hudTrans.position.z);
            };
        }
        else if (_vo.attackType == AttackType.A_A)
        {
            Vector2 attackPos1 = Vector2.Lerp(defenderReal.transform.position, attacker.transform.position, attackMoveDistance);

            dele += delegate (float _value)
            {
                Vector2 v = Vector2.Lerp(defenderReal.transform.position, attackPos1, _value);

                defenderReal.hudTrans.position = new Vector3(v.x, v.y, defenderReal.hudTrans.position.z);
            };
        }

        SuperSequenceControl.To(0f, 1f, 0.5f, dele, _index);

        yield return null;

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

        Vector2 startPos = attacker.hudTrans.position;

        Vector2 targetPos = defender.hudTrans.position;

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector2 v = Vector2.LerpUnclamped(startPos, targetPos, value);

            attacker.moveTrans.position = new Vector3(v.x, v.y, attacker.moveTrans.position.z);

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        HeroBeDamaged(attacker, defender);

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _index);

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator AttackBoth(int _index, int _lastIndex, BattleAttackBothVO _vo)
    {
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        HeroBattle defender = battleManager.heroDic[_vo.defender];

        Vector2 targetPos = Vector2.Lerp(attacker.hudTrans.position, defender.hudTrans.position, 0.5f);

        bool getHit = false;

        Action<float> attackerToDel = delegate (float obj)
        {
            float value = attackCurve.Evaluate(obj);

            Vector2 v = Vector2.LerpUnclamped(attacker.hudTrans.transform.position, targetPos, value);

            attacker.moveTrans.position = new Vector3(v.x, v.y, attacker.moveTrans.position.z);

            v = Vector2.LerpUnclamped(defender.hudTrans.transform.position, targetPos, value);

            defender.moveTrans.position = new Vector3(v.x, v.y, defender.moveTrans.position.z);

            if (!getHit && obj > hitPercent)
            {
                getHit = true;

                SuperSequenceControl.MoveNext(_index);
            }
        };

        SuperSequenceControl.To(0f, 1f, 1f, attackerToDel, _index);

        yield return null;

        HeroBeDamaged(attacker, defender);

        HeroBeDamaged(defender, attacker);

        yield return null;

        SuperSequenceControl.DelayCall(2.0f, _index);

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    private void HeroBeDamaged(HeroBattle _attacker, HeroBattle _hero)
    {
        int nowShield = _hero.hero.nowShield;

        int nowHp = _hero.hero.nowHp;

        int targetShield;

        int targetHp;

        _hero.hero.ProcessDamage(out targetShield, out targetHp);

        List<BattleHeroEffectVO> list = null;

        if (targetShield < nowShield)
        {
            if (list == null)
            {
                list = new List<BattleHeroEffectVO>();
            }

            list.Add(new BattleHeroEffectVO(Effect.SHIELD_CHANGE, targetShield - nowShield));
        }

        if (targetHp < nowHp)
        {
            if (list == null)
            {
                list = new List<BattleHeroEffectVO>();
            }

            list.Add(new BattleHeroEffectVO(Effect.HP_CHANGE, targetHp - nowHp));
        }

        if (list != null)
        {
            bool shock = _hero.TakeEffect(list);

            if (shock)
            {
                _hero.Shock(_attacker, shockCurve, shockDis);
            }
        }
    }

    public IEnumerator AttackOver(int _index, int _lastIndex, BattleAttackOverVO _vo)
    {
        HeroBattle attacker = battleManager.heroDic[_vo.attacker];

        Vector3 attackerStartPos = attacker.hudTrans.localPosition;

        Action<float> dele = delegate (float _value)
        {
            Vector2 v = Vector2.Lerp(attackerStartPos, Vector3.zero, _value);

            attacker.hudTrans.localPosition = new Vector3(v.x, v.y, attacker.hudTrans.localPosition.z);
        };

        if (_vo.attackType == AttackType.A_S)
        {
            HeroBattle supporter = battleManager.heroDic[_vo.defender];

            Vector3 supporterStartPos = supporter.hudTrans.localPosition;

            dele += delegate (float _value)
            {
                Vector2 v = Vector2.Lerp(supporterStartPos, Vector3.zero, _value);

                supporter.hudTrans.localPosition = new Vector3(v.x, v.y, supporter.hudTrans.localPosition.z);
            };

            HeroBattle defender;

            if (battleManager.heroDic.TryGetValue(_vo.pos, out defender))
            {
                Vector3 defenderStartPos = defender.hudTrans.localPosition;

                dele += delegate (float _value)
                {
                    Vector2 v = Vector2.Lerp(defenderStartPos, Vector3.zero, _value);

                    defender.hudTrans.localPosition = new Vector3(v.x, v.y, defender.hudTrans.localPosition.z);
                };
            }
        }
        else if (_vo.attackType == AttackType.A_A)
        {
            HeroBattle defender = battleManager.heroDic[_vo.defender];

            Vector3 defenderStartPos = defender.hudTrans.localPosition;

            dele += delegate (float _value)
            {
                Vector2 v = Vector2.Lerp(defenderStartPos, Vector3.zero, _value);

                defender.hudTrans.localPosition = new Vector3(v.x, v.y, defender.hudTrans.localPosition.z);
            };
        }

        SuperSequenceControl.To(0f, 1f, 0.5f, dele, _index);

        yield return null;

        if (_vo.attackType == AttackType.A_S)
        {
            AttackOver(_index, _vo.attacker, _vo.defender, _vo.pos);
        }
        else
        {
            AttackOver(_index, _vo.attacker, _vo.defender);
        }

        yield return null;

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

                IEnumerator<KeyValuePair<int, int>> enumerator = moves.GetEnumerator();

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

            List<int> ll = new List<int>();

            for (int i = 0; i < tmpList2.Count; i++)
            {
                KeyValuePair<int, int> pair = tmpList2[i];

                if (!ll.Contains(pair.Key))
                {
                    ll.Add(pair.Key);
                }

                if (!ll.Contains(pair.Value))
                {
                    ll.Add(pair.Value);
                }
            }

            MoveCamera(_index, ll.ToArray());

            yield return null;

            for (int i = 0; i < tmpList2.Count; i++)
            {
                KeyValuePair<int, int> pair = tmpList2[i];

                HeroBattle hero = battleManager.heroDic[pair.Key];

                tmpDic.Add(pair.Key, hero);

                battleManager.heroDic.Remove(pair.Key);

                Vector2 startPos = battleManager.mapUnitDic[pair.Key].transform.position;

                Vector2 endPos = battleManager.mapUnitDic[pair.Value].transform.position;

                Action<float> toDel = delegate (float obj)
                {
                    Vector2 v = Vector2.Lerp(startPos, endPos, obj);

                    hero.transform.position = new Vector3(v.x, v.y, hero.transform.position.z);
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

                hero.RefreshAttack();

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

            MoveCamera(_index, pos);

            yield return null;

            HeroBattle hero = battleManager.heroDic[pos];

            battleManager.heroDic.Remove(pos);

            hero.Die(dele);

            yield return null;
        }

        SuperSequenceControl.MoveNext(_lastIndex);
    }

    public IEnumerator Summon(int _index, int _lastIndex, BattleSummonVO _vo)
    {
        MoveCamera(_index, _vo.pos);

        yield return null;

        Hero hero = battleManager.battle.GetHero(_vo.pos);

        HeroBattle heroBattle = battleManager.AddHeroToMap(hero);

        heroBattle.transform.localScale = Vector3.zero;

        Action<float> toDel = delegate (float obj)
        {
            float scale = obj;

            heroBattle.transform.localScale = new Vector3(scale, scale, scale);

            heroBattle.transform.localPosition = new Vector3(heroBattle.transform.localPosition.x, heroBattle.transform.localPosition.y, 100 * (1 - obj));
        };

        SuperSequenceControl.To(10f, 1f, 0.3f, toDel, _index);

        yield return null;

        SuperSequenceControl.DelayCall(0.8f, _lastIndex);
    }

    public IEnumerator TriggerAura(int _index, int _lastIndex, BattleTriggerAuraVO _vo)
    {
        List<int> tmpList = new List<int>();

        tmpList.Add(_vo.pos);

        if (_vo.data != null)
        {
            IEnumerator<int> enumerator2 = _vo.data.Keys.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                if (!tmpList.Contains(enumerator2.Current))
                {
                    tmpList.Add(enumerator2.Current);
                }
            }
        }

        MoveCamera(_index, tmpList.ToArray());

        yield return null;

        HeroBattle hero = battleManager.heroDic[_vo.pos];

        Action<float> dele = delegate (float _value)
        {
            hero.moveTrans.localRotation = Quaternion.Euler(0, 0, _value);
        };

        SuperSequenceControl.To(0, 720, 0.5f, dele, _index);

        yield return null;

        if (_vo.data != null)
        {
            IEnumerator<KeyValuePair<int, List<BattleHeroEffectVO>>> enumerator = _vo.data.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HeroBattle targetHero = battleManager.heroDic[enumerator.Current.Key];

                targetHero.RefreshAttack();

                bool shock = targetHero.TakeEffect(enumerator.Current.Value);

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

    public void PrepareAttack(int _index, params int[] _posArr)
    {
        Action<float> dele = delegate (float _v)
        {
            _v = cameraMoveCurve.Evaluate(_v);

            float mapScale = PublicTools.FloatLerp(battleManager.defaultScale, mapTargetScale, _v);

            battleManager.SetBattleContainerScale(mapScale, Vector2.zero);

            float unitScale = PublicTools.FloatLerp(1, unitTargetScale, _v);

            IEnumerator<MapUnit> enumerator = battleManager.mapUnitDic.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                MapUnit unit = enumerator.Current;

                if (Array.IndexOf(_posArr, unit.index) == -1)
                {
                    Color color = unit.GetMainColor();

                    unit.SetMainColor(new Color(color.r, color.g, color.b, (1 - _v) * battleManager.myMapUnitColor.a));
                }

                unit.transform.localScale = new Vector3(unitScale, unitScale, 1);
            }

            IEnumerator<HeroBattle> enumerator2 = battleManager.heroDic.Values.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                HeroBattle hero = enumerator2.Current;

                if (Array.IndexOf(_posArr, hero.pos) == -1)
                {
                    hero.SetAlpha(1 - _v);
                }

                hero.transform.localScale = new Vector3(unitScale, unitScale, 1);
            }

            float c = PublicTools.FloatLerp(1, bgAlpha, _v);

            battleManager.bg.color = new Color(c, c, c, battleManager.bg.color.a);
        };

        SuperSequenceControl.To(0, 1, 1f, dele, _index);
    }

    public void AttackOver(int _index, params int[] _posArr)
    {
        Action<float> dele = delegate (float _v)
        {
            _v = 1 - cameraMoveCurve.Evaluate(_v);

            float mapScale = PublicTools.FloatLerp(battleManager.defaultScale, mapTargetScale, _v);

            battleManager.SetBattleContainerScale(mapScale, Vector2.zero);

            float unitScale = PublicTools.FloatLerp(1, unitTargetScale, _v);

            IEnumerator<MapUnit> enumerator = battleManager.mapUnitDic.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                MapUnit unit = enumerator.Current;

                if (Array.IndexOf(_posArr, unit.index) == -1)
                {
                    Color color = unit.GetMainColor();

                    unit.SetMainColor(new Color(color.r, color.g, color.b, (1 - _v) * battleManager.myMapUnitColor.a));
                }

                unit.transform.localScale = new Vector3(unitScale, unitScale, 1);
            }

            IEnumerator<HeroBattle> enumerator2 = battleManager.heroDic.Values.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                HeroBattle hero = enumerator2.Current;

                if (Array.IndexOf(_posArr, hero.pos) == -1)
                {
                    hero.SetAlpha(1 - _v);
                }

                hero.transform.localScale = new Vector3(unitScale, unitScale, 1);
            }

            float c = PublicTools.FloatLerp(1, bgAlpha, _v);

            battleManager.bg.color = new Color(c, c, c, battleManager.bg.color.a);
        };

        SuperSequenceControl.To(0, 1, 1f, dele, _index);
    }

    private Vector2 GetCenterPos(params int[] _posArr)
    {
        Vector3 v = Vector3.zero;

        for (int i = 0; i < _posArr.Length; i++)
        {
            MapUnit unit = battleManager.mapUnitDic[_posArr[i]];

            v += unit.transform.position;
        }

        v = v / _posArr.Length;

        return v;
    }

    private void MoveCamera(int _index, params int[] _posArr)
    {
        Vector2 nowPos = battleManager.battleContainer.position;

        Vector2 v = GetCenterPos(_posArr);

        Vector2 targetPos = nowPos - v;

        float dis = v.magnitude;

        Action<float> dele = delegate (float _v)
        {
            _v = cameraMoveCurve.Evaluate(_v);

            Vector2 pos = Vector2.Lerp(nowPos, targetPos, _v);

            battleManager.battleContainer.position = new Vector3(pos.x, pos.y, battleManager.battleContainer.position.z);
        };

        SuperSequenceControl.To(0, 1, dis * cameraMoveSpeedFix, dele, _index);
    }

    public IEnumerator ResetCamera(int _index, int _lastIndex)
    {
        Vector2 nowPos = battleManager.battleContainer.position;

        Vector2 targetPos = battleManager.viewport.center;

        float dis = Vector2.Distance(nowPos, targetPos);

        Action<float> dele = delegate (float _v)
        {
            _v = cameraMoveCurve.Evaluate(_v);

            Vector2 pos = Vector2.Lerp(nowPos, targetPos, _v);

            battleManager.battleContainer.position = new Vector3(pos.x, pos.y, battleManager.battleContainer.position.z);
        };

        SuperSequenceControl.To(0, 1, dis * cameraMoveSpeedFix, dele, _index);

        yield return null;

        SuperSequenceControl.MoveNext(_lastIndex);
    }
}

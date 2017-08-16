using System;
using System.Collections.Generic;
using UnityEngine;
using FinalWar;
using superTween;

public class NewHeroBattle : MonoBehaviour
{
    [SerializeField]
    public Transform hudTrans;

    [SerializeField]
    public Transform moveTrans;

    [SerializeField]
    public Transform shockTrans;

    [SerializeField]
    public Transform zTrans;

    [SerializeField]
    private MeshColorControl frame;

    [SerializeField]
    private MeshColorControl body;

    [SerializeField]
    private TextMesh shield;

    [SerializeField]
    private TextMesh hp;

    [SerializeField]
    private TextMesh attack;

    [SerializeField]
    private TextMesh heroType;

    [SerializeField]
    private TextMesh heroName;

    private Hero hero;

    public bool isMine
    {
        get
        {
            return hero.isMine;
        }
    }

    public int pos
    {
        get
        {
            return hero.pos;
        }
    }

    public bool canAction
    {
        get
        {
            return hero.GetCanAction();
        }
    }

    public HeroSDS sds { get; private set; }

    public int cardUid;

    protected void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        heroName.text = sds.name;

        heroType.text = _heroSDS.heroTypeFix.name;
    }

    public void SetFrameVisible(bool _visible)
    {
        frame.gameObject.SetActive(_visible);
    }

    public void SetFrameColor(Color _color)
    {
        frame.SetColor(_color);
    }

    public void Init(int _id)
    {
        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        hp.gameObject.SetActive(false);

        shield.gameObject.SetActive(false);

        attack.gameObject.SetActive(false);
    }

    public void Init(Hero _hero)
    {
        hero = _hero;

        RefreshAll();
    }

    public void RefreshAll()
    {
        InitCard(hero.sds as HeroSDS);

        RefreshHpAndShield();

        RefreshAttackWithoutShield();
    }

    public void RefreshHpAndShield()
    {
        int nowShield;

        int nowHp;

        hero.ProcessDamage(out nowShield, out nowHp);

        hp.text = nowHp.ToString();

        shield.text = nowShield.ToString();

        body.SetColor(hero.GetCanAction() ? Color.white : Color.grey);
    }

    public void RefreshAttackWithoutShield()
    {
        attack.text = hero.GetDamageWithoutShield().ToString();
    }

    public void RefreshAttack()
    {
        attack.text = hero.GetDamage().ToString();
    }

    public void RefreshAttack(int _attack)
    {
        attack.text = _attack.ToString();
    }

    public void Shock(HeroBattle _hero, AnimationCurve _curve, float _shockDis)
    {
        Vector3 shockVector = (hudTrans.position - _hero.hudTrans.position).normalized * _shockDis;

        Action<float> shockToDel = delegate (float obj)
        {
            float value = _curve.Evaluate(obj);

            shockTrans.localPosition = shockVector * value;
        };

        SuperTween.Instance.To(0, 1, 1, shockToDel, null);
    }

    public void ShowHud(string _str, Color _color, float _yFix, Action _callBack)
    {
        GameObject go = Instantiate(BattleControl.Instance.damageNumResources);

        go.transform.SetParent(transform.parent, false);

        Vector3 pos = transform.parent.InverseTransformPoint(transform.TransformPoint(hudTrans.localPosition));

        go.transform.localPosition = new Vector3(pos.x, pos.y + _yFix, pos.z);

        DamageNum damageNum = go.GetComponent<DamageNum>();

        damageNum.Init(_str, _color, _callBack);
    }

    public bool TakeEffect(List<BattleHeroEffectVO> _list)
    {
        bool shock = false;

        for (int i = 0; i < _list.Count; i++)
        {
            BattleHeroEffectVO effectVO = _list[i];

            switch (effectVO.effect)
            {
                case Effect.DAMAGE:

                    shock = true;

                    ShowHud((-effectVO.data).ToString(), Color.red, i * BattleControl.Instance.hudHeight, null);

                    break;

                case Effect.SHIELD_CHANGE:

                    if (effectVO.data > 0)
                    {
                        ShowHud("+" + effectVO.data.ToString(), Color.yellow, i * BattleControl.Instance.hudHeight, null);
                    }
                    else
                    {
                        shock = true;

                        ShowHud(effectVO.data.ToString(), Color.yellow, i * BattleControl.Instance.hudHeight, null);
                    }

                    break;

                case Effect.HP_CHANGE:

                    if (effectVO.data > 0)
                    {
                        ShowHud("+" + effectVO.data.ToString(), Color.blue, i * BattleControl.Instance.hudHeight, null);
                    }
                    else
                    {
                        shock = true;

                        ShowHud(effectVO.data.ToString(), Color.blue, i * BattleControl.Instance.hudHeight, null);
                    }

                    break;

                case Effect.FIX_ATTACK:
                case Effect.FIX_SPEED:

                    if (effectVO.data > 0)
                    {
                        ShowHud(effectVO.effect.ToString() + " +" + effectVO.data.ToString(), Color.black, i * BattleControl.Instance.hudHeight, null);
                    }
                    else
                    {
                        ShowHud(effectVO.effect.ToString() + " " + effectVO.data.ToString(), Color.black, i * BattleControl.Instance.hudHeight, null);
                    }

                    break;

                case Effect.DISABLE_ACTION:
                case Effect.DISABLE_MOVE:
                case Effect.DISABLE_RECOVER_SHIELD:
                case Effect.SILENCE:

                    ShowHud(effectVO.effect.ToString(), Color.black, i * BattleControl.Instance.hudHeight, null);

                    break;
            }
        }

        RefreshHpAndShield();

        return shock;
    }

    public void Die(Action _del)
    {
        Action dieOver = delegate ()
        {
            Destroy(gameObject);

            if (_del != null)
            {
                _del();
            }
        };

        SuperTween.Instance.To(1, 0, BattleControl.Instance.dieTime, DieTo, dieOver);
    }

    private void DieTo(float _v)
    {
        Color color = frame.GetColor();

        frame.SetColor(new Color(color.r, color.g, color.b, _v));

        color = body.GetColor();

        body.SetColor(new Color(color.r, color.g, color.b, _v));

        shield.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        attack.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        hp.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        heroName.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        heroType.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);
    }
}

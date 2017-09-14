using System;
using System.Collections.Generic;
using UnityEngine;
using FinalWar;
using superTween;
using gameObjectFactory;
using textureFactory;

public class HeroBattle : HeroBase
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
    private SpriteRenderer frame;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    private SpriteRenderer bg;

    [SerializeField]
    private TextMesh shield;

    [SerializeField]
    private TextMeshOutline shieldOutline;

    [SerializeField]
    private TextMesh hp;

    [SerializeField]
    private TextMeshOutline hpOutline;

    [SerializeField]
    private TextMesh attack;

    [SerializeField]
    private TextMeshOutline attackOutline;

    [SerializeField]
    private SpriteRenderer heroType;

    [SerializeField]
    private GameObject chooseFrame;

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

    protected void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        heroType.sprite = BattleControl.Instance.typeSprite[sds.heroTypeFix.ID];

        TextureFactory.Instance.GetTexture<Sprite>("Assets/Resource/texture/" + sds.icon + ".png", GetBodySprite, true);
    }

    private void GetBodySprite(Sprite _sp)
    {
        body.sprite = _sp;
    }

    public void SetFrameVisible(bool _visible)
    {
        chooseFrame.SetActive(_visible);

        //frame.sprite = _visible ? BattleControl.Instance.frameChoose : BattleControl.Instance.frame;
    }

    public void Init(int _cardUid, int _id)
    {
        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        hp.gameObject.SetActive(false);

        attack.gameObject.SetActive(false);

        shield.color = Color.red;

        string text = heroSDS.cost.ToString();

        shield.text = text;

        shieldOutline.SetText(text);
    }

    public void Init(Hero _hero, int _heroUid)
    {
        zTrans.localPosition = new Vector3(0, 0, _heroUid * BattleControl.Instance.zFixStep);

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

        string text = nowHp.ToString();

        hp.text = text;

        hpOutline.SetText(text);

        text = nowShield.ToString();

        shield.text = text;

        shieldOutline.SetText(text);

        if (hero.GetCanAction())
        {
            body.material = BattleControl.Instance.mat;

            bg.material = BattleControl.Instance.mat;

            frame.material = BattleControl.Instance.mat;

            heroType.material = BattleControl.Instance.mat;
        }
        else
        {
            body.material = BattleControl.Instance.matGray;

            bg.material = BattleControl.Instance.matGray;

            frame.material = BattleControl.Instance.matGray;

            heroType.material = BattleControl.Instance.matGray;
        }
    }

    public void RefreshAttackWithoutShield()
    {
        string text = hero.GetDamageWithoutShield().ToString();

        attack.text = text;

        attackOutline.SetText(text);
    }

    public void RefreshAttack()
    {
        string text = hero.GetDamage().ToString();

        attack.text = text;

        attackOutline.SetText(text);
    }

    public void RefreshAttack(int _attack)
    {
        string text = _attack.ToString();

        attack.text = text;

        attackOutline.SetText(text);
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

    public void ShowHud(string _str, Color _color, Color _outlineColor, float _yFix, Action _callBack)
    {
        //GameObject go = Instantiate(Resources.Load<GameObject>("DamageNum"));

        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/DamageNum.prefab", null);

        go.transform.SetParent(BattleManager.Instance.arrowContainer, false);

        Vector3 pos = transform.parent.InverseTransformPoint(transform.TransformPoint(hudTrans.localPosition));

        go.transform.localPosition = new Vector3(pos.x, pos.y + _yFix, 0);

        DamageNum damageNum = go.GetComponent<DamageNum>();

        damageNum.Init(_str, _color, _outlineColor, _callBack);
    }

    public bool TakeEffect(List<BattleHeroEffectVO> _list)
    {
        bool shock = false;

        if (_list != null)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                BattleHeroEffectVO effectVO = _list[i];

                switch (effectVO.effect)
                {
                    case Effect.DAMAGE:

                        shock = true;

                        ShowHud((-effectVO.data).ToString(), Color.red, Color.black, i * BattleControl.Instance.hudHeight, null);

                        break;

                    case Effect.SHIELD_CHANGE:

                        if (effectVO.data > 0)
                        {
                            ShowHud("+" + effectVO.data.ToString(), Color.yellow, Color.blue, i * BattleControl.Instance.hudHeight, null);
                        }
                        else
                        {
                            shock = true;

                            ShowHud(effectVO.data.ToString(), Color.yellow, Color.blue, i * BattleControl.Instance.hudHeight, null);
                        }

                        break;

                    case Effect.HP_CHANGE:

                        if (effectVO.data > 0)
                        {
                            ShowHud("+" + effectVO.data.ToString(), Color.blue, Color.red, i * BattleControl.Instance.hudHeight, null);
                        }
                        else
                        {
                            shock = true;

                            ShowHud(effectVO.data.ToString(), Color.blue, Color.red, i * BattleControl.Instance.hudHeight, null);
                        }

                        break;

                    case Effect.FIX_ATTACK:
                    case Effect.FIX_SPEED:

                        if (effectVO.data > 0)
                        {
                            ShowHud(effectVO.effect.ToString() + " +" + effectVO.data.ToString(), Color.black, Color.red, i * BattleControl.Instance.hudHeight, null);
                        }
                        else
                        {
                            ShowHud(effectVO.effect.ToString() + " " + effectVO.data.ToString(), Color.black, Color.red, i * BattleControl.Instance.hudHeight, null);
                        }

                        break;

                    case Effect.DISABLE_MOVE:
                    case Effect.DISABLE_RECOVER_SHIELD:
                    case Effect.SILENCE:

                        ShowHud(effectVO.effect.ToString(), Color.black, Color.red, i * BattleControl.Instance.hudHeight, null);

                        break;
                }
            }

            RefreshHpAndShield();
        }

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
        frame.color = new Color(1, 1, 1, _v);

        body.color = new Color(1, 1, 1, _v);

        bg.color = new Color(1, 1, 1, _v);

        shield.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        attack.color = new Color(attack.color.r, attack.color.g, attack.color.b, _v);

        hp.color = new Color(hp.color.r, hp.color.g, hp.color.b, _v);

        heroType.color = new Color(1, 1, 1, _v);
    }
}

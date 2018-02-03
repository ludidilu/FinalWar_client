using System;
using System.Collections.Generic;
using UnityEngine;
using FinalWar;
using superTween;
using gameObjectFactory;

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
    private TextMesh speed;

    [SerializeField]
    private TextMeshOutline speedOutline;

    [SerializeField]
    private SpriteRenderer heroType;

    [SerializeField]
    private GameObject chooseFrame;

    private BattleManager battleManager;

    private BattleControl battleControl;

    public Hero hero;

    public bool isHero
    {
        get
        {
            return hero != null;
        }
    }

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

    protected override void GetHeroTypeSprite(Sprite _sp)
    {
        heroType.sprite = _sp;
    }

    protected override void GetBodySprite(Sprite _sp)
    {
        body.sprite = _sp;
    }

    public void SetFrameVisible(bool _visible)
    {
        chooseFrame.SetActive(_visible);

        //frame.sprite = _visible ? battleControl.frameChoose : battleControl.frame;
    }

    public void Init(BattleManager _battleManager, BattleControl _battleControl, int _cardUid, int _id)
    {
        battleManager = _battleManager;

        battleControl = _battleControl;

        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        hp.gameObject.SetActive(false);

        attack.gameObject.SetActive(false);

        shield.color = Color.red;

        string text = heroSDS.cost.ToString();

        shield.text = text;

        shieldOutline.SetText(text);

        speed.gameObject.SetActive(false);
    }

    public void Init(BattleManager _battleManager, BattleControl _battleControl, Hero _hero, int _heroUid)
    {
        battleManager = _battleManager;

        battleControl = _battleControl;

        hero = _hero;

        zTrans.localPosition = new Vector3(0, 0, _heroUid * battleControl.zFixStep);

        RefreshAll();
    }

    public void RefreshAll()
    {
        InitCard(hero.sds as HeroSDS);

        RefreshHpAndShield();

        RefreshAttack();
    }

    public void RefreshHpAndShield()
    {
        int nowShield;

        int nowHp;

        hero.ProcessDamage(out nowShield, out nowHp);

        if (nowShield < 0)
        {
            nowShield = 0;
        }

        if (nowHp < 0)
        {
            nowHp = 0;
        }
        else if (nowHp > sds.GetHp())
        {
            nowHp = sds.GetHp();
        }

        string text = nowHp.ToString();

        hp.text = text;

        hpOutline.SetText(text);

        text = nowShield.ToString();

        shield.text = text;

        shieldOutline.SetText(text);

        int speedFix = hero.GetSpeedFix();

        if (speedFix > 0)
        {
            text = "+" + speedFix;

            speed.gameObject.SetActive(true);

            speed.text = text;

            speed.color = Color.green;

            speedOutline.SetText(text);
        }
        else if (speedFix < 0)
        {
            text = speedFix.ToString();

            speed.gameObject.SetActive(true);

            speed.text = text;

            speed.color = Color.red;

            speedOutline.SetText(text);
        }
        else
        {
            speed.gameObject.SetActive(false);
        }

        if (hero.GetCanAction())
        {
            body.material = battleControl.mat;

            bg.material = battleControl.mat;

            frame.material = battleControl.mat;

            heroType.material = battleControl.mat;
        }
        else
        {
            body.material = battleControl.matGray;

            bg.material = battleControl.matGray;

            frame.material = battleControl.matGray;

            heroType.material = battleControl.matGray;
        }
    }

    public void RefreshAttack()
    {
        //string text = hero.GetAttack().ToString();

        string text = hero.sds.GetAttack().ToString();

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
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/DamageNum.prefab", null);

        go.transform.SetParent(battleManager.arrowContainer, false);

        float scale = go.transform.localScale.x / battleManager.arrowContainer.lossyScale.x;

        go.transform.localScale = new Vector3(scale, scale, 1);

        go.transform.position = new Vector3(hudTrans.position.x, hudTrans.position.y + _yFix, 0);

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
                    case Effect.SHIELD_CHANGE:

                        int data = effectVO.data;

                        if (data > 0)
                        {
                            ShowHud("+" + data.ToString(), Color.yellow, Color.blue, i * battleControl.hudHeight, null);
                        }
                        else
                        {
                            shock = true;

                            ShowHud(data.ToString(), Color.yellow, Color.blue, i * battleControl.hudHeight, null);
                        }

                        break;

                    case Effect.HP_CHANGE:

                        data = effectVO.data;

                        if (data > 0)
                        {
                            ShowHud("+" + data.ToString(), Color.blue, Color.red, i * battleControl.hudHeight, null);
                        }
                        else
                        {
                            shock = true;

                            ShowHud(data.ToString(), Color.blue, Color.red, i * battleControl.hudHeight, null);
                        }

                        break;

                    case Effect.BE_CLEANED:

                        ShowHud("Clean", Color.black, Color.red, i * battleControl.hudHeight, null);

                        break;

                    case Effect.ADD_AURA:

                        AuraSDS sds = StaticData.GetData<AuraSDS>(effectVO.data);

                        ShowHud(sds.hud, Color.black, Color.red, i * battleControl.hudHeight, null);

                        break;

                    case Effect.BE_KILLED:

                        shock = true;

                        ShowHud("Die", Color.black, Color.red, i * battleControl.hudHeight, null);

                        break;

                    case Effect.ADD_MONEY:

                        battleManager.CreateMoneyTf();

                        break;

                    case Effect.CHANGE_HERO:

                        battleManager.ClearHeros();

                        battleManager.CreateHeros();

                        break;
                }
            }

            battleManager.DoRecover();
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

        SuperTween.Instance.To(1, 0, battleControl.dieTime, SetAlpha, dieOver);
    }

    public void SetAlpha(float _v)
    {
        frame.color = new Color(1, 1, 1, _v);

        body.color = new Color(1, 1, 1, _v);

        bg.color = new Color(1, 1, 1, _v);

        shield.color = new Color(shield.color.r, shield.color.g, shield.color.b, _v);

        attack.color = new Color(attack.color.r, attack.color.g, attack.color.b, _v);

        hp.color = new Color(hp.color.r, hp.color.g, hp.color.b, _v);

        heroType.color = new Color(1, 1, 1, _v);

        if (speed.gameObject.activeSelf)
        {
            speed.color = new Color(speed.color.a, speed.color.g, speed.color.b, _v);
        }
    }

    public void GetDesc(ref List<int> _list)
    {
        hero.GetDesc(ref _list);
    }

    public List<int> GetCanAttackPos()
    {
        return hero.GetCanAttackPos();
    }
}

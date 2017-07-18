using UnityEngine;
using superTween;
using System;
using UnityEngine.UI;
using FinalWar;

public class HeroBattle : HeroBase
{
    [SerializeField]
    private Image body;

    [SerializeField]
    public Transform moveTrans;

    [SerializeField]
    public Transform shockTrans;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text hp;

    [SerializeField]
    private Text shield;

    [SerializeField]
    private Text attack;

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
            return hero.canAction == 0;
        }
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

        RefreshHpAndShield();

        RefreshAttackWithoutShield();
    }

    public void RefreshHpAndShield()
    {
        InitCard(hero.sds as HeroSDS);

        int nowShield;

        int nowHp;

        hero.ProcessDamage(out nowShield, out nowHp);

        hp.text = nowHp.ToString();

        shield.text = nowShield.ToString();

        body.color = hero.canAction == 0 ? Color.white : Color.grey;
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

    public void Shock(Vector3 _target, AnimationCurve _curve, float _shockDis, int _damage)
    {
        Vector3 pos = transform.parent.InverseTransformPoint(transform.TransformPoint(moveTrans.localPosition));

        Vector3 shockVector = (pos - _target).normalized * _shockDis;

        Action<float> shockToDel = delegate (float obj)
        {
            float value = _curve.Evaluate(obj);

            shockTrans.localPosition = shockVector * value;
        };

        SuperTween.Instance.To(0, 1, 1, shockToDel, null);

		ShowHud((-_damage).ToString(), Color.red, null);

        RefreshHpAndShield();
    }

    public void ShowHud(string _str, Color _color, Action _callBack)
    {
        GameObject go = Instantiate(BattleControl.Instance.damageNumResources);

        go.transform.SetParent(transform.parent, false);

        Vector3 pos = transform.parent.InverseTransformPoint(transform.TransformPoint(moveTrans.localPosition));

        go.transform.localPosition = pos;

        DamageNum damageNum = go.GetComponent<DamageNum>();

        damageNum.Init(_str, _color, _callBack);
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
        canvasGroup.alpha = _v;
    }
}

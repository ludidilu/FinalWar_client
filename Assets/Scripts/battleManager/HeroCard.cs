using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using superTween;
using System;

public class HeroCard : HeroBase, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField]
    protected Text cost;

    [SerializeField]
    private Image frame;

    [SerializeField]
    private Image body;

    [SerializeField]
    private Image heroType;

    [SerializeField]
    private float showHeroDetailHoldTime;

    private BattleManager battleManager;

    private BattleControl battleControl;

    private HeroDetail heroDetail;

    private bool hasDown = false;

    private static int showHeroDetailTweenID = -1;

    public void Init(BattleManager _battleManager, BattleControl _battleControl, HeroDetail _heroDetail, int _cardUid, int _id)
    {
        battleManager = _battleManager;

        battleControl = _battleControl;

        heroDetail = _heroDetail;

        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        cost.text = sds.cost.ToString();

        if (sds.cost > battleManager.battle.GetNowMoney(battleManager.battle.clientIsMine))
        {
            cost.color = Color.red;
        }
        else
        {
            cost.color = Color.white;
        }
    }

    //public void OnPointerClick(PointerEventData _data)
    //{
    //    battleManager.HeroClick(this);
    //}

    public void OnPointerDown(PointerEventData _data)
    {
        hasDown = true;

        Action dele = delegate ()
        {
            showHeroDetailTweenID = -1;

            heroDetail.Show(this);

            battleManager.HeroClick(this);
        };

        showHeroDetailTweenID = SuperTween.Instance.DelayCall(showHeroDetailHoldTime, dele);
    }

    public void OnPointerExit(PointerEventData _data)
    {
        RemoveShowHeroDetailTween();

        hasDown = false;
    }

    public void OnPointerUp(PointerEventData _data)
    {
        RemoveShowHeroDetailTween();

        if (hasDown)
        {
            hasDown = false;

            battleManager.HeroClick(this);
        }
    }

    public static void RemoveShowHeroDetailTween()
    {
        if (showHeroDetailTweenID != -1)
        {
            SuperTween.Instance.Remove(showHeroDetailTweenID);

            showHeroDetailTweenID = -1;
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
        frame.sprite = _visible ? battleControl.frameChoose : battleControl.frame;
    }
}

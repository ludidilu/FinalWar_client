using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCard : HeroBase, IPointerClickHandler
{
    [SerializeField]
    protected Text cost;

    [SerializeField]
    private Image frame;

    [SerializeField]
    private Image body;

    [SerializeField]
    private Image heroType;

    private BattleManager battleManager;

    private BattleControl battleControl;

    public void Init(BattleManager _battleManager, BattleControl _battleControl, int _cardUid, int _id)
    {
        battleManager = _battleManager;

        battleControl = _battleControl;

        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        cost.text = sds.cost.ToString();
    }

    public void OnPointerClick(PointerEventData _data)
    {
        battleManager.HeroClick(this);
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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using textureFactory;

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

    public void Init(int _cardUid, int _id)
    {
        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        cost.text = sds.cost.ToString();
    }

    public void OnPointerClick(PointerEventData _data)
    {
        BattleManager.Instance.HeroClick(this);
    }

    private void InitCard(HeroSDS _heroSDS)
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
        frame.sprite = _visible ? BattleControl.Instance.frameChoose : BattleControl.Instance.frame;
    }
}

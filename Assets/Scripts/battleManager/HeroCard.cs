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
    private Text nameText;

    [SerializeField]
    private Text ability;

    public void Init(int _cardUid, int _id)
    {
        cardUid = _cardUid;

        HeroSDS heroSDS = StaticData.GetData<HeroSDS>(_id);

        InitCard(heroSDS);

        cost.text = sds.cost.ToString();
    }

    public void OnPointerClick(PointerEventData _data)
    {
        SendMessageUpwards("HeroClick", this, SendMessageOptions.DontRequireReceiver);
    }

    private void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        nameText.text = sds.name;

        ability.text = _heroSDS.heroTypeFix.name;
    }

    public void SetFrameVisible(bool _visible)
    {
        frame.gameObject.SetActive(_visible);
    }

    public void SetFrameColor(Color _color)
    {
        frame.color = _color;
    }
}

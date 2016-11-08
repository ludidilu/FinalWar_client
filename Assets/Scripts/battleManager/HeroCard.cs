using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCard : HeroBase, IPointerClickHandler
{
    [SerializeField]
    protected Text cost;

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
}

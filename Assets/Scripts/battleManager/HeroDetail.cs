using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroDetail : MonoBehaviour
{
    [SerializeField]
    private BattleManager battleManager;

    [SerializeField]
    private Text heroName;

    [SerializeField]
    private ClickText cost;

    [SerializeField]
    private ClickText hp;

    [SerializeField]
    private ClickText shield;

    [SerializeField]
    private ClickText attack;

    [SerializeField]
    private ClickText abilityType;

    [SerializeField]
    private RectTransform container;

    [SerializeField]
    private RectTransform poolContainer;

    [SerializeField]
    private GameObject cellResource;

    [SerializeField]
    private CanvasGroup cg;

    [SerializeField]
    private float defaultHeight;

    private HeroBase hero;

    private Queue<Text> cellList = new Queue<Text>();

    private Queue<Text> cellPool = new Queue<Text>();

    public void Show(HeroBase _hero)
    {
        hero = _hero;

        heroName.text = hero.sds.name;

        cost.text = hero.sds.cost.ToString();

        hp.text = hero.sds.hp.ToString();

        shield.text = hero.sds.shield.ToString();

        attack.text = hero.sds.attack.ToString();

        abilityType.text = hero.sds.heroTypeFix.name;

        abilityType.clickKey = hero.sds.heroTypeFix.ID;

        ClearCell();

        float height = defaultHeight;

        if (!string.IsNullOrEmpty(hero.sds.comment))
        {
            AddCell(hero.sds.comment, ref height);
        }

        if (_hero is HeroBattle)
        {
            HeroBattle hero = _hero as HeroBattle;

            if (hero.isHero)
            {
                List<string> list = null;

                hero.GetDesc(ref list);

                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        AddCell(list[i], ref height);
                    }
                }
            }

            battleManager.ClickHeroBattleShowMapUnitIcon(hero);
        }
        else
        {
            HeroCard hero = _hero as HeroCard;

            battleManager.ClickHeroCardShowMapUnitIcon(hero);
        }

        container.sizeDelta = new Vector2(container.sizeDelta.x, height);

        if (!cg.blocksRaycasts)
        {
            cg.alpha = 1;

            cg.blocksRaycasts = true;
        }
    }

    private void ClearCell()
    {
        while (cellList.Count > 0)
        {
            Text cell = cellList.Dequeue();

            cell.transform.SetParent(poolContainer, false);

            cellPool.Enqueue(cell);
        }
    }

    public void AddCell(string _str, ref float _height)
    {
        Text cell;

        if (cellPool.Count > 0)
        {
            cell = cellPool.Dequeue();
        }
        else
        {
            cell = Instantiate(cellResource).GetComponent<Text>();
        }

        cell.transform.SetParent(container, false);

        cell.text = _str;

        cellList.Enqueue(cell);

        float height = cell.preferredHeight + 1;

        cell.rectTransform.sizeDelta = new Vector2(cell.rectTransform.sizeDelta.x, height);

        cell.rectTransform.anchoredPosition = new Vector2(0, -_height);

        _height += height;
    }

    public void Hide(HeroBase _hero)
    {
        if (hero == _hero)
        {
            hero = null;

            if (cg.blocksRaycasts)
            {
                cg.alpha = 0;

                cg.blocksRaycasts = false;
            }

            battleManager.ClearMapUnitIcon();
        }
    }

    public void Hide()
    {
        hero = null;

        if (cg.blocksRaycasts)
        {
            cg.alpha = 0;

            cg.blocksRaycasts = false;
        }

        battleManager.ClearMapUnitIcon();
    }
}

﻿using UnityEngine;
using UnityEngine.UI;
using superFunction;

public class HeroDetail : MonoBehaviour
{
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
    private Text comment;

    [SerializeField]
    private GameObject commentContainer;

    private HeroBase hero;

    void Awake()
    {
        SuperFunction.Instance.AddEventListener<int>(ClickText.eventGo, ClickText.EVENT_NAME, GetClick);
    }

    private void GetClick(int _index, int _id)
    {
        if (_id > 0)
        {
            DescSDS sds = StaticData.GetData<DescSDS>(_id);

            BattleManager.Instance.Alert(sds.desc, null);
        }
    }

    public void Show(HeroBase _hero)
    {
        hero = _hero;

        heroName.text = hero.sds.name;

        cost.text = hero.sds.cost.ToString();

        hp.text = hero.sds.hp.ToString();

        shield.text = hero.sds.shield.ToString();

        attack.text = hero.sds.attack.ToString();

        abilityType.text = hero.sds.heroTypeFix.name;

        abilityType.clickKey = hero.sds.ID;

        if (!string.IsNullOrEmpty(hero.sds.comment))
        {
            comment.text = hero.sds.comment;

            if (!commentContainer.activeSelf)
            {
                commentContainer.SetActive(true);
            }
        }
        else
        {
            if (commentContainer.activeSelf)
            {
                commentContainer.SetActive(false);
            }
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide(HeroBase _hero)
    {
        if (hero == _hero)
        {
            hero = null;

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Hide()
    {
        hero = null;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

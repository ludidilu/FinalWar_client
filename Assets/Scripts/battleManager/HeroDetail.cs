using UnityEngine;
using UnityEngine.UI;

public class HeroDetail : MonoBehaviour
{
    [SerializeField]
    private Text heroName;

    [SerializeField]
    private Text cost;

    [SerializeField]
    private Text hp;

    [SerializeField]
    private Text shield;

    [SerializeField]
    private Text attack;

    [SerializeField]
    private Text abilityType;

    [SerializeField]
    private Text comment;

    [SerializeField]
    private GameObject commentContainer;

    private HeroBase hero;

    public void Show(HeroBase _hero)
    {
        hero = _hero;

        heroName.text = hero.sds.name;

        cost.text = hero.sds.cost.ToString();

        hp.text = hero.sds.hp.ToString();

        shield.text = hero.sds.shield.ToString();

        attack.text = hero.sds.attack.ToString();

        abilityType.text = hero.sds.heroTypeFix.name;

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

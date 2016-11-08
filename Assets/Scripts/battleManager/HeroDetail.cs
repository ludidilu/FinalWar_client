using UnityEngine;
using System.Collections;
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
    private Text shoot;

    [SerializeField]
    private Text comment;

    private HeroBase hero;

    public void Show(HeroBase _hero)
    {
        hero = _hero;

        heroName.text = hero.sds.name;

        cost.text = hero.sds.cost.ToString();

        hp.text = hero.sds.hp.ToString();

        shield.text = hero.sds.shield.ToString();

        attack.text = hero.sds.attack.ToString();

        shoot.text = hero.sds.shoot.ToString();

        comment.text = hero.sds.comment;

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

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

        shoot.text = hero.sds.shoot.ToString();

        comment.text = hero.sds.comment;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

		if (!string.IsNullOrEmpty (hero.sds.comment)) {

			comment.text = hero.sds.comment;

			if(!commentContainer.activeSelf){

				commentContainer.SetActive(true);
			}

		}else{

			if(commentContainer.activeSelf){

				commentContainer.SetActive(false);
			}
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

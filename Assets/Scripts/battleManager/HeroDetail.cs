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

		ShowReal (hero.sds);
    }

	private void ShowReal(HeroSDS _heroSDS){

		heroName.color = _heroSDS == hero.sds ? Color.black : Color.red;

		heroName.text = _heroSDS.name;
		
		cost.text = _heroSDS.cost.ToString();
		
		hp.text = _heroSDS.hp.ToString();
		
		shield.text = _heroSDS.shield.ToString();
		
		attack.text = _heroSDS.attack.ToString();

		abilityType.text = _heroSDS.heroTypeFix.name;

		if (!string.IsNullOrEmpty (_heroSDS.comment)) {
			
			comment.text = _heroSDS.comment;
			
			if(!commentContainer.activeSelf){
				
				commentContainer.SetActive(true);
			}
			
		}else{
			
			if(commentContainer.activeSelf){
				
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

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
	private Text levelUp;

	[SerializeField]
	private GameObject commentContainer;

    private HeroBase hero;

	private int levelUpID;

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
		
		shoot.text = _heroSDS.shoot.ToString();
		
		if (_heroSDS.levelUp != 0) {
			
			if(!levelUp.gameObject.activeSelf){
				
				levelUp.gameObject.SetActive (true);
			}

			levelUpID = _heroSDS.levelUp;
			
			HeroSDS sds = StaticData.GetData<HeroSDS> (levelUpID);
			
			levelUp.text = sds.name;
			
		} else {
			
			if(levelUp.gameObject.activeSelf){
				
				levelUp.gameObject.SetActive(false);
			}
		}

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

	public void LevelUpClick(){

		HeroSDS sds = StaticData.GetData<HeroSDS> (levelUpID);

		ShowReal (sds);
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FinalWar;

public class NewBehaviourScript : MonoBehaviour
{
    [Serializable]
    public class AuraData
    {
        public string eventName;

        public AuraType auraType;
    }

    public GameObject container;

    public Dropdown eventNameDropdown;

    public InputField priorityInputField;

    public Dropdown triggerTargetDropdown;

    public List<AuraData> auraDataList;

    public AuraConditionComponent condition;

    public Dropdown auraTarget;

    public AuraConditionComponent targetCondition;

    public InputField targetNum;

    public InputField castSkillEffect;

    public Toggle auraBool;

    public Dropdown auraIntDropdown;

    public InputField auraIntInputField;

    public Toggle removeWhenRoundOver;

    public Toggle removeWhenDoDamage;

    public Toggle removeWhenBeDamaged;

    private readonly AuraTarget[] triggerTargetArr = new AuraTarget[]
    {
        AuraTarget.OWNER,
        AuraTarget.OWNER_ALLY,
        AuraTarget.OWNER_ENEMY,
        AuraTarget.OWNER_NEIGHBOUR,
        AuraTarget.OWNER_NEIGHBOUR_ALLY,
        AuraTarget.OWNER_NEIGHBOUR_ENEMY,
    };



    // Use this for initialization
    void Start()
    {
        container.SetActive(false);





        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

        list.Add(new Dropdown.OptionData("EventName"));

        for (int i = 0; i < auraDataList.Count; i++)
        {
            list.Add(new Dropdown.OptionData(auraDataList[i].eventName));
        }

        eventNameDropdown.AddOptions(list);




        list = new List<Dropdown.OptionData>();

        for (int i = 0; i < triggerTargetArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(triggerTargetArr[i].ToString()));
        }

        triggerTargetDropdown.AddOptions(list);






        list = new List<Dropdown.OptionData>();

        string[] strArr = Enum.GetNames(typeof(AuraTarget));

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        auraTarget.AddOptions(list);




        list = new List<Dropdown.OptionData>();

        strArr = Enum.GetNames(typeof(Hero.HeroData));

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        auraIntDropdown.AddOptions(list);



    }

    public void EventNameDropdownValueChagne(int _index)
    {
        if (_index != 0)
        {
            container.SetActive(true);

            AuraData auraData = auraDataList[_index - 1];

            priorityInputField.gameObject.SetActive(auraData.auraType != AuraType.CAST_SKILL);

            auraTarget.gameObject.SetActive(auraData.auraType == AuraType.CAST_SKILL);

            condition.RefreshAuraCompare();

            RefreshTargetDropdown();

            if (auraData.auraType == AuraType.CAST_SKILL)
            {
                castSkillEffect.gameObject.SetActive(true);

                auraBool.gameObject.SetActive(false);

                auraIntDropdown.gameObject.SetActive(false);

                auraIntInputField.gameObject.SetActive(false);
            }
            else if (auraData.auraType == AuraType.FIX_BOOL)
            {
                castSkillEffect.gameObject.SetActive(false);

                auraBool.gameObject.SetActive(true);

                auraIntDropdown.gameObject.SetActive(false);

                auraIntInputField.gameObject.SetActive(false);
            }
            else
            {
                castSkillEffect.gameObject.SetActive(false);

                auraBool.gameObject.SetActive(false);

                auraIntDropdown.gameObject.SetActive(true);

                auraIntInputField.gameObject.SetActive(true);
            }
        }
        else
        {
            container.SetActive(false);
        }
    }

    public void TargetDropdownValueChange(int _index)
    {
        RefreshTargetDropdown();
    }

    private void RefreshTargetDropdown()
    {
        AuraTarget target = (AuraTarget)auraTarget.value;

        switch (target)
        {
            case AuraTarget.OWNER_ALLY:
            case AuraTarget.OWNER_ENEMY:
            case AuraTarget.OWNER_NEIGHBOUR:
            case AuraTarget.OWNER_NEIGHBOUR_ALLY:
            case AuraTarget.OWNER_NEIGHBOUR_ENEMY:

                targetCondition.gameObject.SetActive(true);

                targetCondition.RefreshAuraCompare();

                targetNum.gameObject.SetActive(true);

                break;

            default:

                targetCondition.gameObject.SetActive(false);

                targetNum.gameObject.SetActive(false);

                break;
        }
    }
}

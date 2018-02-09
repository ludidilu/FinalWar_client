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
        }
        else
        {
            container.SetActive(false);
        }
    }

    public void TargetDropdownValueChange(int _index)
    {

    }
}

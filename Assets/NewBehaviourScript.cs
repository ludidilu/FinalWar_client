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

    public InputField targetNumInputField;

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

                targetNumInputField.gameObject.SetActive(true);

                break;

            default:

                targetCondition.gameObject.SetActive(false);

                targetNumInputField.gameObject.SetActive(false);

                break;
        }
    }

    public void Save()
    {
        AuraData data = auraDataList[eventNameDropdown.value - 1];

        string eventName = data.eventName;

        AuraType auraType = data.auraType;

        int priority;

        AuraTarget effectTarget;

        AuraTarget triggerTarget;

        AuraConditionCompare compare;

        Hero.HeroData compareType0 = default(Hero.HeroData);

        Hero.HeroData compareType1 = default(Hero.HeroData);

        AuraTarget compareTarget0 = default(AuraTarget);

        AuraTarget compareTarget1 = default(AuraTarget);

        int compareData0 = 0;

        int compareData1 = 0;

        AuraConditionCompare targetCompare;

        Hero.HeroData targetCompareType0 = default(Hero.HeroData);

        Hero.HeroData targetCompareType1 = default(Hero.HeroData);

        AuraTarget targetCompareTarget0 = default(AuraTarget);

        AuraTarget targetCompareTarget1 = default(AuraTarget);

        int targetCompareData0 = 0;

        int targetCompareData1 = 0;

        int targetNum = 0;

        int[] effectData;

        string[] removeEventNames;

        if (auraType == AuraType.CAST_SKILL)
        {
            priority = 0;

            effectTarget = (AuraTarget)auraTarget.value;

            if (effectTarget == AuraTarget.OWNER_ALLY || effectTarget == AuraTarget.OWNER_ENEMY || effectTarget == AuraTarget.OWNER_NEIGHBOUR || effectTarget == AuraTarget.OWNER_NEIGHBOUR_ALLY || effectTarget == AuraTarget.OWNER_NEIGHBOUR_ENEMY)
            {
                targetCompare = (AuraConditionCompare)targetCondition.compareDropdown.value;

                if (targetCompare != AuraConditionCompare.NULL)
                {
                    targetCompareType0 = (Hero.HeroData)targetCondition.compareType0.value;

                    targetCompareType1 = (Hero.HeroData)targetCondition.compareType1.value;

                    if (targetCompareType0 == Hero.HeroData.DATA)
                    {
                        if (string.IsNullOrEmpty(targetCondition.compareData0.text))
                        {
                            targetCompareData0 = 0;
                        }
                        else
                        {
                            targetCompareData0 = int.Parse(targetCondition.compareData0.text);
                        }
                    }
                    else
                    {
                        targetCompareTarget0 = AuraConditionComponent.auraConditionTarget[targetCondition.compareTarget0.value];
                    }

                    if (targetCompareType1 == Hero.HeroData.DATA)
                    {
                        if (string.IsNullOrEmpty(targetCondition.compareData1.text))
                        {
                            targetCompareData1 = 0;
                        }
                        else
                        {
                            targetCompareData1 = int.Parse(targetCondition.compareData1.text);
                        }
                    }
                    else
                    {
                        targetCompareTarget1 = AuraConditionComponent.auraConditionTarget[targetCondition.compareTarget1.value];
                    }
                }

                targetNum = int.Parse(targetNumInputField.text);
            }

            effectData = new int[] { int.Parse(castSkillEffect.text) };
        }
        else
        {
            if (string.IsNullOrEmpty(priorityInputField.text))
            {
                priority = 0;
            }
            else
            {
                priority = int.Parse(priorityInputField.text);
            }

            if (auraType == AuraType.FIX_BOOL)
            {
                effectData = new int[] { auraBool.isOn ? 1 : 0 };
            }
            else
            {
                effectData = new int[] { auraIntDropdown.value, int.Parse(auraIntInputField.text) };
            }
        }

        triggerTarget = (AuraTarget)triggerTargetDropdown.value;

        compare = (AuraConditionCompare)condition.compareDropdown.value;

        if (compare != AuraConditionCompare.NULL)
        {
            compareType0 = (Hero.HeroData)condition.compareType0.value;

            compareType1 = (Hero.HeroData)condition.compareType1.value;

            if (compareType0 == Hero.HeroData.DATA)
            {
                if (string.IsNullOrEmpty(condition.compareData0.text))
                {
                    compareData0 = 0;
                }
                else
                {
                    compareData0 = int.Parse(condition.compareData0.text);
                }
            }
            else
            {
                compareTarget0 = AuraConditionComponent.auraConditionTarget[condition.compareTarget0.value];
            }

            if (compareType1 == Hero.HeroData.DATA)
            {
                if (string.IsNullOrEmpty(condition.compareData1.text))
                {
                    compareData1 = 0;
                }
                else
                {
                    compareData1 = int.Parse(condition.compareData1.text);
                }
            }
            else
            {
                compareTarget1 = AuraConditionComponent.auraConditionTarget[condition.compareTarget1.value];
            }
        }

        List<string> list = new List<string>();

        if (removeWhenRoundOver.isOn)
        {
            list.Add(BattleConst.ROUND_OVER);
        }

        if (removeWhenDoDamage)
        {
            list.Add(BattleConst.DO_DAMAGE);
        }

        if (removeWhenBeDamaged)
        {
            list.Add(BattleConst.BE_DAMAGED);
        }

        removeEventNames = list.ToArray();

        List<string> result = new List<string>();

        result.Add(eventName);

        result.Add(priority.ToString());

        result.Add(((int)triggerTarget).ToString());

        result.Add(((int)compare).ToString());

        if (compare == AuraConditionCompare.NULL)
        {
            result.Add(string.Empty);

            result.Add(string.Empty);

            result.Add(string.Empty);
        }
        else
        {
            result.Add(((int)compareType0).ToString() + "$" + ((int)compareType1).ToString());

            string data0 = "0";

            string data1 = "0";

            string target0 = "0";

            string target1 = "0";

            if (compareType0 == Hero.HeroData.DATA)
            {
                data0 = compareData0.ToString();
            }
            else
            {
                target0 = ((int)compareType0).ToString();
            }

            if (compareType1 == Hero.HeroData.DATA)
            {
                data1 = compareData1.ToString();
            }
            else
            {
                target1 = ((int)compareType1).ToString();
            }

            result.Add(target0 + "$" + target1);

            result.Add(data0 + "$" + data1);
        }
    }
}

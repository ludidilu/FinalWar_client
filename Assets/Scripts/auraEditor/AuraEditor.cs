using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FinalWar;

public class AuraEditor : MonoBehaviour
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

    public Toggle isSetIntToggle;

    public Dropdown triggerTargetDropdown;

    public List<AuraData> auraDataList;

    public AuraConditionComponent condition;

    public Dropdown auraTarget;

    public InputField targetNumInputField;

    public InputField castSkillEffect;

    public Dropdown auraIntDropdown;

    public InputField auraIntInputField;

    public Toggle removeWhenRoundOver;

    public Toggle removeWhenDoDamage;

    public Toggle removeWhenBeDamaged;

    public Toggle isShow;

    public InputField descInputField;

    public InputField hudInputField;

    public GameObject resultGo;

    public InputField resultInputField;

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

        resultGo.SetActive(false);



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

            auraTarget.gameObject.SetActive(auraData.auraType == AuraType.CAST_SKILL);

            condition.RefreshAuraCompare();

            RefreshTargetDropdown();

            if (auraData.auraType == AuraType.CAST_SKILL)
            {
                priorityInputField.gameObject.SetActive(false);

                isSetIntToggle.gameObject.SetActive(false);

                castSkillEffect.gameObject.SetActive(true);

                auraIntDropdown.gameObject.SetActive(false);

                auraIntInputField.gameObject.SetActive(false);
            }
            else
            {
                priorityInputField.gameObject.SetActive(true);

                isSetIntToggle.gameObject.SetActive(true);

                castSkillEffect.gameObject.SetActive(false);

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
        if (!auraTarget.gameObject.activeSelf)
        {
            targetNumInputField.gameObject.SetActive(false);

            return;
        }

        AuraTarget target = (AuraTarget)auraTarget.value;

        switch (target)
        {
            case AuraTarget.OWNER_ALLY:
            case AuraTarget.OWNER_ENEMY:
            case AuraTarget.OWNER_NEIGHBOUR:
            case AuraTarget.OWNER_NEIGHBOUR_ALLY:
            case AuraTarget.OWNER_NEIGHBOUR_ENEMY:

                targetNumInputField.gameObject.SetActive(true);

                break;

            default:

                targetNumInputField.gameObject.SetActive(false);

                break;
        }
    }

    public void Save()
    {
        AuraData data = auraDataList[eventNameDropdown.value - 1];

        string eventName = data.eventName;

        AuraType auraType = data.auraType;

        int priority = 0;

        AuraTarget effectTarget = default(AuraTarget);

        AuraTarget triggerTarget;

        AuraConditionCompare compare;

        Hero.HeroData compareType0 = default(Hero.HeroData);

        Hero.HeroData compareType1 = default(Hero.HeroData);

        AuraTarget compareTarget0 = default(AuraTarget);

        AuraTarget compareTarget1 = default(AuraTarget);

        int compareData0 = 0;

        int compareData1 = 0;

        int targetNum = 0;

        int[] effectData;

        string[] removeEventNames;

        if (auraType == AuraType.CAST_SKILL)
        {
            effectTarget = (AuraTarget)auraTarget.value;

            if (effectTarget == AuraTarget.OWNER_ALLY || effectTarget == AuraTarget.OWNER_ENEMY || effectTarget == AuraTarget.OWNER_NEIGHBOUR || effectTarget == AuraTarget.OWNER_NEIGHBOUR_ALLY || effectTarget == AuraTarget.OWNER_NEIGHBOUR_ENEMY)
            {
                if (string.IsNullOrEmpty(targetNumInputField.text))
                {
                    targetNum = 0;
                }
                else
                {
                    targetNum = int.Parse(targetNumInputField.text);
                }
            }

            if (string.IsNullOrEmpty(castSkillEffect.text))
            {
                Debug.Log("error:castSkillEffect is empty!");

                return;
            }

            effectData = new int[] { int.Parse(castSkillEffect.text) };
        }
        else
        {
            if (string.IsNullOrEmpty(auraIntInputField.text))
            {
                Debug.Log("error:AuraInt is zero!");

                return;
            }

            effectData = new int[] { auraIntDropdown.value, int.Parse(auraIntInputField.text) };

            if (!string.IsNullOrEmpty(priorityInputField.text))
            {
                priority = int.Parse(priorityInputField.text);
            }
        }

        triggerTarget = triggerTargetArr[triggerTargetDropdown.value];

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
            list.Add(BattleConst.RECOVER);
        }

        if (removeWhenDoDamage.isOn)
        {
            list.Add(BattleConst.DO_DAMAGE);
        }

        if (removeWhenBeDamaged.isOn)
        {
            list.Add(BattleConst.BE_DAMAGED);
        }

        removeEventNames = list.ToArray();

        List<string> result = new List<string>();

        result.Add(eventName);

        if (auraType == AuraType.CAST_SKILL)
        {
            result.Add(((int)auraType).ToString());
        }
        else
        {
            if (isSetIntToggle.isOn)
            {
                result.Add(((int)AuraType.SET_INT).ToString());
            }
            else
            {
                result.Add(((int)AuraType.FIX_INT).ToString());
            }
        }

        result.Add(priority.ToString());

        result.Add(((int)triggerTarget).ToString());

        result.Add(((int)compare).ToString());

        if (compare == AuraConditionCompare.NULL)
        {
            result.Add(string.Empty);

            result.Add(string.Empty);
        }
        else
        {
            result.Add(((int)compareType0).ToString() + "$" + ((int)compareType1).ToString());

            string data0 = "0";

            string data1 = "0";

            if (compareType0 == Hero.HeroData.DATA)
            {
                data0 = compareData0.ToString();
            }
            else
            {
                data0 = ((int)compareTarget0).ToString();
            }

            if (compareType1 == Hero.HeroData.DATA)
            {
                data1 = compareData1.ToString();
            }
            else
            {
                data1 = ((int)compareTarget1).ToString();
            }

            result.Add(data0 + "$" + data1);
        }

        result.Add(((int)effectTarget).ToString());

        result.Add(targetNum.ToString());

        string[] strArr = new string[effectData.Length];

        for (int i = 0; i < effectData.Length; i++)
        {
            strArr[i] = effectData[i].ToString();
        }

        result.Add(string.Join("$", strArr));

        result.Add(string.Join("$", removeEventNames));

        result.Add(isShow.isOn ? "1" : "0");

        result.Add(descInputField.text);

        result.Add(hudInputField.text);

        result.Add(descInputField.text);

        string resultStr = string.Join(",", result.ToArray());

        resultGo.SetActive(true);

        resultInputField.text = resultStr;

        Debug.Log(resultStr);
    }

    public void ClickBg()
    {
        resultGo.SetActive(false);
    }
}

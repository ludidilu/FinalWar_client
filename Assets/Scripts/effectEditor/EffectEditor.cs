using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FinalWar;

public class EffectEditor : MonoBehaviour
{
    public Dropdown effectDropdown;

    public AuraConditionComponent condition;

    public Dropdown effectDataDropdown;

    public InputField effectDataInputField;

    public InputField descInputField;

    public GameObject resultGo;

    public InputField resultInputField;

    // Use this for initialization
    void Start()
    {
        resultGo.SetActive(false);



        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

        string[] strArr = Enum.GetNames(typeof(Effect));

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        effectDropdown.AddOptions(list);




        list = new List<Dropdown.OptionData>();

        strArr = Enum.GetNames(typeof(Hero.HeroData));

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        effectDataDropdown.AddOptions(list);




        EffectDropdownValueChagne(0);

        condition.RefreshAuraCompare();
    }

    public void EffectDropdownValueChagne(int _index)
    {
        Effect effect = (Effect)_index;

        if (effect == Effect.ADD_AURA || effect == Effect.CHANGE_HERO)
        {
            effectDataDropdown.gameObject.SetActive(false);

            effectDataInputField.gameObject.SetActive(true);
        }
        else if (effect == Effect.BE_KILLED || effect == Effect.BE_CLEANED)
        {
            effectDataDropdown.gameObject.SetActive(false);

            effectDataInputField.gameObject.SetActive(false);
        }
        else
        {
            effectDataDropdown.gameObject.SetActive(true);

            effectDataInputField.gameObject.SetActive(true);
        }
    }

    public void Save()
    {
        Effect effect = (Effect)effectDropdown.value;

        string priority;

        if ((Effect)effectDropdown.value == Effect.BE_CLEANED)
        {
            priority = "0";
        }
        else if ((Effect)effectDataDropdown.value == Effect.CHANGE_HERO)
        {
            priority = "2";
        }
        else
        {
            priority = "1";
        }

        AuraConditionCompare compare;

        Hero.HeroData compareType0 = default(Hero.HeroData);

        Hero.HeroData compareType1 = default(Hero.HeroData);

        AuraTarget compareTarget0 = default(AuraTarget);

        AuraTarget compareTarget1 = default(AuraTarget);

        int compareData0 = 0;

        int compareData1 = 0;

        string[] effectData;

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

        if (effect == Effect.ADD_AURA || effect == Effect.CHANGE_HERO)
        {
            if (string.IsNullOrEmpty(effectDataInputField.text))
            {
                Debug.Log("effectDataInputField is empty!");

                return;
            }

            effectData = new string[] { effectDataInputField.text };
        }
        else if (effect == Effect.BE_KILLED || effect == Effect.BE_CLEANED)
        {
            effectData = new string[0];
        }
        else
        {
            if (string.IsNullOrEmpty(effectDataInputField.text))
            {
                Debug.Log("effectDataInputField is empty!");

                return;
            }

            effectData = new string[] { effectDataDropdown.value.ToString(), effectDataInputField.text };
        }

        List<string> result = new List<string>();

        result.Add(((int)effect).ToString());

        result.Add(priority);

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

        result.Add(string.Join("$", effectData));

        result.Add(descInputField.text);

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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FinalWar;

public class AuraConditionComponent : MonoBehaviour
{
    public Dropdown compareDropdown;

    public GameObject container;

    public Dropdown compareType0;

    public Dropdown compareType1;

    public Dropdown compareTarget0;

    public Dropdown compareTarget1;

    public InputField compareData0;

    public InputField compareData1;

    public static readonly AuraTarget[] auraConditionTarget = new AuraTarget[]
    {
        AuraTarget.OWNER,
        AuraTarget.TRIGGER,
        AuraTarget.TRIGGER_TARGET,
    };

    // Use this for initialization
    void Awake()
    {
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

        string[] strArr = Enum.GetNames(typeof(AuraConditionCompare));

        list = new List<Dropdown.OptionData>();

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        compareDropdown.AddOptions(list);




        list = new List<Dropdown.OptionData>();

        strArr = Enum.GetNames(typeof(Hero.HeroData));

        for (int i = 0; i < strArr.Length; i++)
        {
            list.Add(new Dropdown.OptionData(strArr[i]));
        }

        compareType0.AddOptions(list);

        compareType1.AddOptions(list);




        list = new List<Dropdown.OptionData>();

        for (int i = 0; i < auraConditionTarget.Length; i++)
        {
            list.Add(new Dropdown.OptionData(auraConditionTarget[i].ToString()));
        }

        compareTarget0.AddOptions(list);

        compareTarget1.AddOptions(list);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AuraConditionDropdownChange(int _index)
    {
        RefreshAuraCompare();
    }

    public void RefreshAuraCompare()
    {
        if (compareDropdown.value != 0)
        {
            container.SetActive(true);

            RefreshAuraCondition0();

            RefreshAuraCondition1();
        }
        else
        {
            container.SetActive(false);
        }
    }

    private void RefreshAuraCondition0()
    {
        if (compareType0.value != 0)
        {
            compareTarget0.gameObject.SetActive(true);

            compareData0.gameObject.SetActive(false);
        }
        else
        {
            compareTarget0.gameObject.SetActive(false);

            compareData0.gameObject.SetActive(true);
        }
    }

    private void RefreshAuraCondition1()
    {
        if (compareType1.value != 0)
        {
            compareTarget1.gameObject.SetActive(true);

            compareData1.gameObject.SetActive(false);
        }
        else
        {
            compareTarget1.gameObject.SetActive(false);

            compareData1.gameObject.SetActive(true);
        }
    }

    public void AuraConditionTypeDropdownChange0(int _index)
    {
        RefreshAuraCondition0();
    }

    public void AuraConditionTypeDropdownChange1(int _index)
    {
        RefreshAuraCondition1();
    }
}

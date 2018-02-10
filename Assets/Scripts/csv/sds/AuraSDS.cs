using FinalWar;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int triggerTarget;
    public int conditionCompare;
    public int[] conditionType;
    public int[] conditionData;
    public int effectType;
    public int effectTarget;
    public int targetConditionCompare;
    public int[] targetConditionType;
    public int[] targetConditionData;
    public int effectTargetNum;
    public int[] effectData;
    public string[] removeEventNames;

    private Hero.HeroData[] conditionTypeFix;

    private Hero.HeroData[] targetConditionTypeFix;

    public override void Fix()
    {
        conditionTypeFix = new Hero.HeroData[conditionType.Length];

        for (int i = 0; i < conditionType.Length; i++)
        {
            conditionTypeFix[i] = (Hero.HeroData)conditionType[i];
        }

        targetConditionTypeFix = new Hero.HeroData[targetConditionType.Length];

        for (int i = 0; i < targetConditionType.Length; i++)
        {
            targetConditionTypeFix[i] = (Hero.HeroData)targetConditionType[i];
        }
    }

    public int GetID()
    {
        return ID;
    }

    public string GetEventName()
    {
        return eventName;
    }

    public AuraTarget GetTriggerTarget()
    {
        return (AuraTarget)triggerTarget;
    }

    public AuraConditionCompare GetConditionCompare()
    {
        return (AuraConditionCompare)conditionCompare;
    }

    public Hero.HeroData[] GetConditionType()
    {
        return conditionTypeFix;
    }

    public int[] GetConditionData()
    {
        return conditionData;
    }

    public AuraType GetEffectType()
    {
        return (AuraType)effectType;
    }

    public AuraTarget GetEffectTarget()
    {
        return (AuraTarget)effectTarget;
    }

    public AuraConditionCompare GetTargetConditionCompare()
    {
        return (AuraConditionCompare)targetConditionCompare;
    }

    public Hero.HeroData[] GetTargetConditionType()
    {
        return targetConditionTypeFix;
    }

    public int[] GetTargetConditionData()
    {
        return targetConditionData;
    }

    public int GetEffectTargetNum()
    {
        return effectTargetNum;
    }

    public int[] GetEffectData()
    {
        return effectData;
    }

    public string[] GetRemoveEventNames()
    {
        return removeEventNames;
    }
}

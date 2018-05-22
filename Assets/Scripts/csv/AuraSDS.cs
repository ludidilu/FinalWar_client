using FinalWar;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int effectType;
    public int priority;
    public int triggerTarget;
    public int conditionCompare;
    public int[] conditionType;
    public int[] conditionData;
    public int effectTarget;
    public int effectTargetNum;
    public int[] effectData;
    public string[] removeEventNames;

    private Hero.HeroData[] conditionTypeFix;

    public override void Fix()
    {
        conditionTypeFix = new Hero.HeroData[conditionType.Length];

        for (int i = 0; i < conditionType.Length; i++)
        {
            conditionTypeFix[i] = (Hero.HeroData)conditionType[i];
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

    public AuraType GetEffectType()
    {
        return (AuraType)effectType;
    }

    public int GetPriority()
    {
        return priority;
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

    public AuraTarget GetEffectTarget()
    {
        return (AuraTarget)effectTarget;
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

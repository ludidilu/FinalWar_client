using FinalWar;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int priority;
    public int triggerTarget;
    public int conditionCompare;
    public int[] conditionType;
    public int[] conditionTarget;
    public int[] conditionData;
    public int effectType;
    public int effectTarget;
    public int effectTargetNum;
    public int[] effectData;
    public string[] removeEventNames;
    public string desc;

    private AuraTarget[] conditionTargetFix;

    private Hero.HeroData[] conditionTypeFix;

    public override void Fix()
    {
        conditionTypeFix = new Hero.HeroData[conditionType.Length];

        conditionTargetFix = new AuraTarget[conditionType.Length];

        for (int i = 0; i < conditionType.Length; i++)
        {
            conditionTypeFix[i] = (Hero.HeroData)conditionType[i];

            conditionTargetFix[i] = (AuraTarget)conditionTarget[i];
        }
    }

    public string GetEventName()
    {
        return eventName;
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

    public AuraTarget[] GetConditionTarget()
    {
        return conditionTargetFix;
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

    public string GetDesc()
    {
        return desc;
    }
}

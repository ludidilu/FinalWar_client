using FinalWar;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int triggerTarget;
    public int conditionCompare;
    public int[] conditionType;
    public int[] conditionTarget;
    public int[] conditionData;
    public int effectType;
    public int effectTarget;
    public int targetConditionCompare;
    public int[] targetConditionType;
    public int[] targetConditionTarget;
    public int[] targetConditionData;
    public int effectTargetNum;
    public int[] effectData;
    public string[] removeEventNames;

    private AuraTarget[] conditionTargetFix;
    private Hero.HeroData[] conditionTypeFix;

    private AuraTarget[] targetConditionTargetFix;
    private Hero.HeroData[] targetConditionTypeFix;

    public override void Fix()
    {
        conditionTypeFix = new Hero.HeroData[conditionType.Length];

        conditionTargetFix = new AuraTarget[conditionTarget.Length];

        for (int i = 0; i < conditionType.Length; i++)
        {
            conditionTypeFix[i] = (Hero.HeroData)conditionType[i];

            conditionTargetFix[i] = (AuraTarget)conditionTarget[i];
        }

        targetConditionTypeFix = new Hero.HeroData[targetConditionType.Length];

        targetConditionTargetFix = new AuraTarget[targetConditionTarget.Length];

        for (int i = 0; i < targetConditionType.Length; i++)
        {
            targetConditionTypeFix[i] = (Hero.HeroData)targetConditionType[i];

            targetConditionTargetFix[i] = (AuraTarget)targetConditionTarget[i];
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

    public AuraConditionCompare GetTargetConditionCompare()
    {
        return (AuraConditionCompare)targetConditionCompare;
    }

    public Hero.HeroData[] GetTargetConditionType()
    {
        return targetConditionTypeFix;
    }

    public AuraTarget[] GetTargetConditionTarget()
    {
        return targetConditionTargetFix;
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

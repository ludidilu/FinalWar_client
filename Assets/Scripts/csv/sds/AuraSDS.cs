public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int auraTrigger;
    public int auraConditionCompare;
    public int[] auraConditionType;
    public int[] auraConditionTarget;
    public int[] auraConditionData;
    public int auraType;
    public int auraTarget;
    public int auraTargetNum;
    public int[] auraData;

    private AuraTarget[] auraConditionTargetFix;

    private AuraConditionType[] auraConditionTypeFix;

    public override void Fix()
    {
        auraConditionTypeFix = new AuraConditionType[auraConditionType.Length];

        auraConditionTargetFix = new AuraTarget[auraConditionType.Length];

        for (int i = 0; i < auraConditionType.Length; i++)
        {
            auraConditionTypeFix[i] = (AuraConditionType)auraConditionType[i];

            auraConditionTargetFix[i] = (AuraTarget)auraConditionTarget[i];
        }
    }

    public string GetEventName()
    {
        return eventName;
    }

    public AuraTarget GetAuraTrigger()
    {
        return (AuraTarget)auraTrigger;
    }

    public AuraConditionCompare GetAuraConditionCompare()
    {
        return (AuraConditionCompare)auraConditionCompare;
    }

    public AuraConditionType[] GetAuraConditionType()
    {
        return auraConditionTypeFix;
    }

    public AuraTarget[] GetAuraConditionTarget()
    {
        return auraConditionTargetFix;
    }

    public int[] GetAuraConditionData()
    {
        return auraConditionData;
    }

    public AuraType GetAuraType()
    {
        return (AuraType)auraType;
    }

    public AuraTarget GetAuraTarget()
    {
        return (AuraTarget)auraTarget;
    }

    public int GetAuraTargetNum()
    {
        return auraTargetNum;
    }

    public int[] GetAuraData()
    {
        return auraData;
    }
}

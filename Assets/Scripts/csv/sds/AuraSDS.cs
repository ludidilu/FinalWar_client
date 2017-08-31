public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int auraTrigger;
    public int auraCondition;
    public int[] auraConditionTarget;
    public int auraConditionData;
    public int auraType;
    public int auraTarget;
    public int[] auraData;

    private AuraTarget[] auraConditionTargetFix;

    public override void Fix()
    {
        auraConditionTargetFix = new AuraTarget[auraConditionTarget.Length];

        for (int i = 0; i < auraConditionTarget.Length; i++)
        {
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

    public AuraCondition GetAuraCondition()
    {
        return (AuraCondition)auraCondition;
    }

    public AuraTarget[] GetAuraConditionTarget()
    {
        return auraConditionTargetFix;
    }

    public int GetAuraConditionData()
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

    public int[] GetAuraData()
    {
        return auraData;
    }
}

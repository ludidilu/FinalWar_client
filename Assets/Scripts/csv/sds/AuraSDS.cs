public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int[] auraCondition;
    public int[] auraConditionData;
    public int auraType;
    public int auraTarget;
    public int[] auraData;

    private AuraCondition[] auraConditionFix;

    public override void Fix()
    {
        auraConditionFix = new AuraCondition[auraCondition.Length];

        for (int i = 0; i < auraCondition.Length; i++)
        {
            auraConditionFix[i] = (AuraCondition)auraCondition[i];
        }
    }

    public string GetEventName()
    {
        return eventName;
    }

    public AuraCondition[] GetAuraCondition()
    {
        return auraConditionFix;
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

    public int[] GetAuraData()
    {
        return auraData;
    }
}

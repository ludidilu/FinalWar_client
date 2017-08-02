public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int auraType;
    public int auraTarget;
    public int[] auraData;

    public string GetEventName()
    {
        return eventName;
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

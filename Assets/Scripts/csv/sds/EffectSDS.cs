using FinalWar;

public partial class EffectSDS : CsvBase, IEffectSDS
{
    public int effect;
    public int priority;
    public int conditionCompare;
    public int[] conditionType;
    public int[] conditionData;
    public int[] data;

    private Hero.HeroData[] conditionTypeFix;

    public override void Fix()
    {
        conditionTypeFix = new Hero.HeroData[conditionType.Length];

        for (int i = 0; i < conditionType.Length; i++)
        {
            conditionTypeFix[i] = (Hero.HeroData)conditionType[i];
        }
    }

    public Effect GetEffect()
    {
        return (Effect)effect;
    }

    public int GetPriority()
    {
        return priority;
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

    public int[] GetData()
    {
        return data;
    }
}

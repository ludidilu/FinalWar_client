public partial class EffectSDS : CsvBase, IEffectSDS
{
    public int effect;
    public int priority;
    public int[] data;

    public Effect GetEffect()
    {
        return (Effect)effect;
    }

    public int GetPriority()
    {
        return priority;
    }

    public int[] GetData()
    {
        return data;
    }
}

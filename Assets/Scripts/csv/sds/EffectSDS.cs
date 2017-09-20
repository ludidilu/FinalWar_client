public class EffectSDS : CsvBase, IEffectSDS
{
    public int effect;
    public int effectPriority;
    public int[] data;

    public Effect GetEffect()
    {
        return (Effect)effect;
    }

    public int GetEffectPriority()
    {
        return effectPriority;
    }

    public int[] GetData()
    {
        return data;
    }
}

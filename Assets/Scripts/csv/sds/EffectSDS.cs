public class EffectSDS : CsvBase, IEffectSDS
{
    public int effect;
    public int[] data;

    public Effect GetEffect()
    {
        return (Effect)effect;
    }

    public int[] GetData()
    {
        return data;
    }
}

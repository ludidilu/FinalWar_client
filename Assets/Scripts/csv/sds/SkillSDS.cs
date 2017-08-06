public partial class SkillSDS : CsvBase, ISkillSDS
{
    public bool isStop;
    public int[] effects;

    public bool GetIsStop()
    {
        return isStop;
    }

    public int[] GetEffects()
    {
        return effects;
    }
}

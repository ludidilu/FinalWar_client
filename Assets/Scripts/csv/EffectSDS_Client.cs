using FinalWar;

public partial class EffectSDS : CsvBase, IEffectSDS
{
    public string desc;

    private string descFix;

    public string GetDesc()
    {
        if (string.IsNullOrEmpty(descFix))
        {
            descFix = BattleManager.FixDesc(desc, AuraSDS.GetDescFix);
        }

        return descFix;
    }
}

using FinalWar;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string desc;
    public string hud;

    private string descFix;

    public string GetDesc()
    {
        if (string.IsNullOrEmpty(descFix))
        {
            descFix = BattleManager.FixStr(desc, GetDescFix);
        }

        return descFix;
    }

    public static string GetDescFix(string _str)
    {
        int id = int.Parse(_str);

        AuraSDS sds = StaticData.GetData<AuraSDS>(id);

        return sds.GetDesc();
    }
}

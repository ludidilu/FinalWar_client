using FinalWar;

public partial class BattleInitDataSDS : CsvBase, IBattleInitDataSDS
{
    public int mapID;
    public int maxRoundNum;
    public int mPlayerInitDataID;
    public int oPlayerInitDataID;

    private PlayerInitDataSDS mPlayerInitData;
    private PlayerInitDataSDS oPlayerInitData;

    public int GetMapID()
    {
        return mapID;
    }

    public int GetMaxRoundNum()
    {
        return maxRoundNum;
    }

    public IPlayerInitDataSDS GetMPlayerInitData()
    {
        if (mPlayerInitData == null)
        {
            mPlayerInitData = StaticData.GetData<PlayerInitDataSDS>(mPlayerInitDataID);
        }

        return mPlayerInitData;
    }

    public IPlayerInitDataSDS GetOPlayerInitData()
    {
        if (oPlayerInitData == null)
        {
            oPlayerInitData = StaticData.GetData<PlayerInitDataSDS>(oPlayerInitDataID);
        }

        return oPlayerInitData;
    }
}

public class MapSDS : CsvBase, IMapSDS
{
    public string name;
    public int[] heroPos;
    public int[] heroID;

    private MapData mapData;

    public MapData GetMapData()
    {
        return mapData;
    }

    public void SetMapData(MapData _mapData)
    {
        mapData = _mapData;
    }

    public int[] GetHeroPos()
    {
        return heroPos;
    }

    public int[] GetHeroID()
    {
        return heroID;
    }
}


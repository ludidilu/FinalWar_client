using System.IO;

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

    public int[] GetHeroPos()
    {
        return heroPos;
    }

    public int[] GetHeroID()
    {
        return heroID;
    }

    public override void Fix()
    {
        mapData = new MapData();

        using (FileStream fs = new FileStream(ConfigDictionary.Instance.map_path + name, FileMode.Open))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                mapData.GetData(br);
            }
        }
    }
}


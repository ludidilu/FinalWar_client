using System.IO;
using System.Collections.Generic;

public class Map
{
	public static Dictionary<int, MapData> mapDataDic;
	
	public static void Init()
	{
		mapDataDic = new Dictionary<int, MapData>();
		
		Dictionary<int, MapSDS> mapDic = StaticData.GetDic<MapSDS>();
		
		Dictionary<int, MapSDS>.Enumerator enumerator = mapDic.GetEnumerator();
		
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, MapSDS> pair = enumerator.Current;
			
			MapData mapData = new MapData();
			
			mapDataDic.Add(pair.Key, mapData);
			
			using (FileStream fs = new FileStream(ConfigDictionary.Instance.map_path + pair.Value.name, FileMode.Open))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					mapData.GetData(br);
				}
			}
		}
	}
}


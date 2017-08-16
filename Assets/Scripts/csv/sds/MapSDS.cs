using System.Collections.Generic;
using System;
using System.IO;
#if USE_ASSETBUNDLE
using System.Threading;
using wwwManager;
using UnityEngine;
using thread;
#endif

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

    public static void Load(Action _callBack)
    {
        Dictionary<string, MapData> mapDic = new Dictionary<string, MapData>();

        Dictionary<int, MapSDS> dic = StaticData.GetDic<MapSDS>();

        Dictionary<int, MapSDS>.ValueCollection.Enumerator enumerator = dic.Values.GetEnumerator();

#if USE_ASSETBUNDLE

        int loadNum = dic.Count;

        Action oneLoadOver = delegate ()
        {
            loadNum--;

            if (loadNum == 0)
            {
                if (_callBack != null)
                {
                    _callBack();
                }
            }
        };
#endif

        while (enumerator.MoveNext())
        {
            string mapName = enumerator.Current.name;

            MapData mapData;

            if (mapDic.TryGetValue(mapName, out mapData))
            {
                enumerator.Current.mapData = mapData;

#if USE_ASSETBUNDLE

                oneLoadOver();
#endif
                continue;
            }

            mapData = new MapData();

            enumerator.Current.mapData = mapData;

            mapDic.Add(enumerator.Current.name, mapData);

#if !USE_ASSETBUNDLE

            using (FileStream fs = new FileStream(ConfigDictionary.Instance.map_path + mapName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    mapData.GetData(br);
                }
            }
#else
            ParameterizedThreadStart getData = delegate (object _obj)
            {
                BinaryReader br = _obj as BinaryReader;

                mapData.GetData(br);

                br.Close();

                br.BaseStream.Dispose();
            };

            Action<WWW> dele = delegate (WWW _www)
            {
                MemoryStream ms = new MemoryStream(_www.bytes);

                BinaryReader br = new BinaryReader(ms);

                ThreadScript.Instance.Add(getData, br, oneLoadOver);
            };

            WWWManager.Instance.Load("/map/" + mapName, dele);
#endif
        }

#if !USE_ASSETBUNDLE

        if (_callBack != null)
        {
            _callBack();
        }
#endif
    }
}


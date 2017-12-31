using System.Collections.Generic;
using System;
using System.IO;
using FinalWar;
#if USE_ASSETBUNDLE
using System.Threading;
using wwwManager;
using UnityEngine;
using thread;
#endif

public class MapSDS : CsvBase, IMapSDS
{
    public string name;
    public string[] hero;
    public bool isFearAction;
    public string[] fearAction;

    private MapData mapData;

    private KeyValuePair<int, int>[] heroReal;

    private KeyValuePair<int, int>[] fearActionReal;

    public MapData GetMapData()
    {
        return mapData;
    }

    public KeyValuePair<int, int>[] GetHero()
    {
        return heroReal;
    }

    public bool GetIsFearAction()
    {
        return isFearAction;
    }

    public KeyValuePair<int, int>[] GetFearAction()
    {
        return fearActionReal;
    }

    public override void Fix()
    {
        heroReal = new KeyValuePair<int, int>[hero.Length];

        for (int i = 0; i < hero.Length; i++)
        {
            string[] strArr = hero[i].Split('&');

            heroReal[i] = new KeyValuePair<int, int>(int.Parse(strArr[0]), int.Parse(strArr[1]));
        }

        fearActionReal = new KeyValuePair<int, int>[fearAction.Length];

        for (int i = 0; i < fearAction.Length; i++)
        {
            string[] strArr = fearAction[i].Split('&');

            fearActionReal[i] = new KeyValuePair<int, int>(int.Parse(strArr[0]), int.Parse(strArr[1]));
        }
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


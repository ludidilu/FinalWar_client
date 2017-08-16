using System;
using System.Collections.Generic;
using System.IO;
#if USE_ASSETBUNDLE
using wwwManager;
using UnityEngine;
using System.Threading;
using thread;
#endif

public static class MapManager
{
    private static Dictionary<string, MapData> mapDic = new Dictionary<string, MapData>();

    public static void Load(Action _callBack)
    {
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
                enumerator.Current.SetMapData(mapData);

#if USE_ASSETBUNDLE

                oneLoadOver();
#endif
                continue;
            }

            mapData = new MapData();

            enumerator.Current.SetMapData(mapData);

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

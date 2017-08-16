using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if USE_ASSETBUNDLE
using wwwManager;
#endif
using FinalWar;

public static class ResourceLoader
{
    private static Action callBack;

    private static int num;

    public static void Load(Action _callBack)
    {
        callBack = _callBack;

        LoadConfig(ConfigLoadOver);
    }

    private static void ConfigLoadOver()
    {
        num = 3;

        LoadRandomData(OneLoadOver);

        LoadTables(LoadMap);

        OneLoadOver();
    }

    public static void LoadConfig(Action _callBack)
    {
#if !USE_ASSETBUNDLE

        LoadConfigLocal();

        if (_callBack != null)
        {
            _callBack();
        }
#else
        Action<WWW> dele = delegate (WWW _www)
        {
            ConfigDictionary.Instance.SetData(_www.text);

            if (_callBack != null)
            {
                _callBack();
            }
        };

        WWWManager.Instance.Load("local.xml", dele);
#endif
    }

    public static void LoadConfigLocal()
    {
        ConfigDictionary.Instance.LoadLocalConfig(Path.Combine(Application.streamingAssetsPath, "local.xml"));
    }

    public static void LoadRandomData(Action _callBack)
    {
#if !USE_ASSETBUNDLE

        using (FileStream fs = new FileStream(Path.Combine(ConfigDictionary.Instance.random_path, "random.dat"), FileMode.Open))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                BattleRandomPool.Load(br);

                if (_callBack != null)
                {
                    _callBack();
                }
            }
        }
#else
        Action<WWW> dele = delegate (WWW _www)
        {
            using (MemoryStream ms = new MemoryStream(_www.bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    BattleRandomPool.Load(br);

                    if (_callBack != null)
                    {
                        _callBack();
                    }
                }
            }
        };

        WWWManager.Instance.Load("random.dat", dele);
#endif
    }

    public static void LoadTables(Action _callBack)
    {
#if !USE_ASSETBUNDLE

        LoadTablesLocal();

        if (_callBack != null)
        {
            _callBack();
        }
#else
        StaticData.LoadCsvDataFromFile(_callBack, LoadCsv.Init);
#endif
    }

    public static void LoadTablesLocal()
    {
        StaticData.path = ConfigDictionary.Instance.table_path;

        StaticData.Dispose();

        StaticData.Load<MapSDS>("map");

        StaticData.Load<HeroTypeSDS>("heroType");

        StaticData.Load<HeroSDS>("hero");

        StaticData.Load<EffectSDS>("effect");

        StaticData.Load<SkillSDS>("skill");

        StaticData.Load<AuraSDS>("aura");
    }

    private static void LoadMap()
    {
        MapManager.Load(OneLoadOver);
    }

    private static void OneLoadOver()
    {
        num--;

        if (num == 0)
        {
            if (callBack != null)
            {
                Action tmpCb = callBack;

                callBack = null;

                tmpCb();
            }
        }
    }
}

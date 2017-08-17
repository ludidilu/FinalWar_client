using System;
using UnityEngine;
using System.IO;
#if USE_ASSETBUNDLE
using wwwManager;
using thread;
using System.Threading;
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
        num = 4;

        LoadRandomData(OneLoadOver);

        LoadTables(LoadMap);

        LoadAiData(OneLoadOver);

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
        MapSDS.Load(OneLoadOver);
    }

    private static void LoadAiData(Action _callBack)
    {
#if !USE_ASSETBUNDLE

        string actionStr = File.ReadAllText(ConfigDictionary.Instance.ai_path + "ai_action.xml");
        string summonStr = File.ReadAllText(ConfigDictionary.Instance.ai_path + "ai_summon.xml");

        BattleAi.Init(actionStr, summonStr);

        if (_callBack != null)
        {
            _callBack();
        }
#else
        string actionStr = string.Empty;
        string summonStr = string.Empty;

        ThreadStart threadDele = delegate ()
        {
            BattleAi.Init(actionStr, summonStr);
        };

        Action dele = delegate ()
        {
            ThreadScript.Instance.Add(threadDele, _callBack);
        };

        Action<WWW> getActionStr = delegate (WWW _www)
        {
            actionStr = _www.text;

            if (!string.IsNullOrEmpty(summonStr))
            {
                dele();
            }
        };

        Action<WWW> getSummonStr = delegate (WWW _www)
        {
            summonStr = _www.text;

            if (!string.IsNullOrEmpty(actionStr))
            {
                dele();
            }
        };

        WWWManager.Instance.Load("/map/ai_action.xml", getActionStr);

        WWWManager.Instance.Load("/map/ai_summon.xml", getSummonStr);
#endif
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

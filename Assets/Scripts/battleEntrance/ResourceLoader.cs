using System;
using UnityEngine;
using System.IO;
#if USE_ASSETBUNDLE
using wwwManager;
using thread;
using System.Threading;
using assetManager;
using assetBundleManager;
#endif
using FinalWar;
using gameObjectFactory;

public static class ResourceLoader
{
    private static readonly string[] preloadPrefabs = new string[] {
        "Assets/Resource/prefab/Arrow.prefab",
        "Assets/Resource/prefab/DamageArrow.prefab",
        "Assets/Resource/prefab/DamageNum.prefab",
        "Assets/Resource/prefab/HeroBattle.prefab",
        "Assets/Resource/prefab/HeroCard.prefab",
        "Assets/Resource/prefab/MapUnit.prefab",
        "Assets/Resource/prefab/ShootArrow.prefab"
    };

    private static Action callBack;

    private static int num;

    public static void Load(Action _callBack)
    {
        callBack = _callBack;

        LoadConfig(ConfigLoadOver);
    }

    private static void ConfigLoadOver()
    {
        num = 7;

        LoadRandomData();

        LoadTables();

        LoadAiData();

        LoadPrefabs();

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

    private static void LoadRandomData()
    {
#if !USE_ASSETBUNDLE

        using (FileStream fs = new FileStream(Path.Combine(ConfigDictionary.Instance.random_path, "random.dat"), FileMode.Open))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                BattleRandomPool.Load(br);

                OneLoadOver();
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

                    OneLoadOver();
                }
            }
        };

        WWWManager.Instance.Load("random.dat", dele);
#endif
    }

    private static void LoadTables()
    {
#if !USE_ASSETBUNDLE

        LoadTablesLocal();

        LoadMap();
#else
        StaticData.LoadCsvDataFromFile(LoadMap, LoadCsv.Init);
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

        StaticData.Load<AuraSDS>("aura");

        StaticData.Load<TestCardsSDS>("testCards");
    }

    private static void LoadMap()
    {
        MapSDS.Load(OneLoadOver);
    }

    private static void LoadAiData()
    {
#if !USE_ASSETBUNDLE

        string actionStr = File.ReadAllText(ConfigDictionary.Instance.ai_path + "ai_action.xml");
        string summonStr = File.ReadAllText(ConfigDictionary.Instance.ai_path + "ai_summon.xml");

        BattleAi.Init(actionStr, summonStr);

        OneLoadOver();
#else
        string actionStr = string.Empty;
        string summonStr = string.Empty;

        ThreadStart threadDele = delegate ()
        {
            BattleAi.Init(actionStr, summonStr);
        };

        Action dele = delegate ()
        {
            ThreadScript.Instance.Add(threadDele, OneLoadOver);
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

        WWWManager.Instance.Load("/ai/ai_action.xml", getActionStr);

        WWWManager.Instance.Load("/ai/ai_summon.xml", getSummonStr);
#endif
    }

    private static void LoadPrefabs()
    {
        Action<GameObject> goDele = delegate (GameObject _go)
        {
            OneLoadOver();
        };

#if !USE_ASSETBUNDLE

        GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);

        GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleManager.prefab", goDele);

        GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleControl.prefab", goDele);
#else
        Action dele = delegate ()
        {
            AssetBundleManager.Instance.Load("texture", null);

            GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);

            GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleManager.prefab", goDele);

            GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleControl.prefab", goDele);
        };

        AssetManager.Instance.Init(dele);
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

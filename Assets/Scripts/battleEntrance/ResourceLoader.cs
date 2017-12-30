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
        "Assets/Resource/prefab/BattleManager.prefab",
        "Assets/Resource/prefab/Arrow.prefab",
        "Assets/Resource/prefab/DamageArrow.prefab",
        "Assets/Resource/prefab/DamageNum.prefab",
        "Assets/Resource/prefab/HeroBattle.prefab",
        "Assets/Resource/prefab/HeroCard.prefab",
        "Assets/Resource/prefab/MapUnit.prefab",
        "Assets/Resource/prefab/ShootArrow.prefab",
        "Assets/Resource/prefab/BattleEntrance.prefab",
        "Assets/Resource/prefab/BattleOnline.prefab",
        "Assets/Resource/prefab/BattleView.prefab",
        "Assets/Resource/prefab/BattleChoose.prefab",
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
        num = 4;

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

        StaticData.Load<DescSDS>("desc");

        StaticData.Load<BattleSDS>("battle");
    }

    private static void LoadMap()
    {
        MapSDS.Load(OneLoadOver);
    }

    private static void LoadAiData()
    {
#if !USE_ASSETBUNDLE

        string actionStr = File.ReadAllText(Path.Combine(ConfigDictionary.Instance.ai_path, "ai_action.xml"));
        string summonStr = File.ReadAllText(Path.Combine(ConfigDictionary.Instance.ai_path, "ai_summon.xml"));

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
#if !USE_ASSETBUNDLE

        GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);
#else
        Action dele = delegate ()
        {
            AssetBundleManager.Instance.Load("texture", null);

            AssetBundleManager.Instance.Load("font", null);

            GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);
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

using System;
using System.Collections.Generic;
using UnityEngine;
using superGraphicRaycast;
using superRaycast;
using publicTools;
using superFunction;
using superTween;

public static class BattleGuide
{
    private const string TAG_NAME = "Guide";

    private static BattleManager battleManager;

    private static int guideIndex;

    public static void Start(BattleManager _battleManager, int _guideIndex)
    {
        battleManager = _battleManager;

        SuperGraphicRaycast.SetFilter(true);

        SuperGraphicRaycast.AddFilterTag(TAG_NAME);

        SuperRaycast.SetFilter(true);

        SuperRaycast.AddTag(TAG_NAME);

        guideIndex = _guideIndex;

        StartGuide();
    }

    private static void Over()
    {
        SuperGraphicRaycast.SetFilter(false);

        SuperGraphicRaycast.RemoveFilterTag(TAG_NAME);

        SuperRaycast.SetFilter(false);

        SuperRaycast.RemoveTag(TAG_NAME);
    }

    private static void Success(int _index)
    {
        ClearData();

        GuideSDS sds = StaticData.GetData<GuideSDS>(guideIndex);

        if (sds.over)
        {
            Over();
        }
        else
        {
            guideIndex++;

            SuperTween.Instance.NextFrameCall(StartGuide);
        }
    }

    private static void Fail(int _index)
    {
        ClearData();

        guideIndex--;

        SuperTween.Instance.NextFrameCall(StartGuide);
    }

    private static void StartGuide()
    {
        SetData();


    }

    private static void SetData()
    {
        GuideSDS sds = StaticData.GetData<GuideSDS>(guideIndex);

        for (int i = 0; i < sds.eventNameArr.Length; i++)
        {
            if (sds.eventResultArr[i])
            {
                SuperFunction.Instance.AddEventListener(battleManager.eventGo, sds.eventNameArr[i], Success);
            }
            else
            {
                SuperFunction.Instance.AddEventListener(battleManager.eventGo, sds.eventNameArr[i], Fail);
            }
        }

        for (int i = 0; i < sds.gameObjectNameArr.Length; i++)
        {
            GameObject go = GameObject.Find(sds.gameObjectNameArr[i]);

            PublicTools.SetTag(go, TAG_NAME);
        }
    }

    private static void ClearData()
    {
        GuideSDS sds = StaticData.GetData<GuideSDS>(guideIndex);

        for (int i = 0; i < sds.eventNameArr.Length; i++)
        {
            if (sds.eventResultArr[i])
            {
                SuperFunction.Instance.RemoveEventListener(battleManager.eventGo, sds.eventNameArr[i], Success);
            }
            else
            {
                SuperFunction.Instance.RemoveEventListener(battleManager.eventGo, sds.eventNameArr[i], Fail);
            }
        }

        for (int i = 0; i < sds.gameObjectNameArr.Length; i++)
        {
            GameObject go = GameObject.Find(sds.gameObjectNameArr[i]);

            if (go != null)
            {
                PublicTools.ClearTag(go);
            }
        }
    }
}

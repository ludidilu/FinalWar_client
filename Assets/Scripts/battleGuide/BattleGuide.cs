using System;
using System.Collections.Generic;
using UnityEngine;
using superGraphicRaycast;
using superRaycast;
using publicTools;
using superFunction;
using superTween;
using textureFactory;

public static class BattleGuide
{
    private const string TAG_NAME = "Guide";

    private static BattleManager battleManager;

    private static int guideIndex;

    private static GameObject finger;

    private static int fingerTweenID = -1;

    public static void Start(BattleManager _battleManager, int _guideIndex)
    {
        if (finger == null)
        {
            finger = new GameObject();

            finger.layer = LayerMask.NameToLayer("UI");

            SpriteRenderer sr = finger.AddComponent<SpriteRenderer>();

            sr.sortingOrder = 1;

            Action<Sprite> dele = delegate (Sprite _sp)
            {
                sr.sprite = _sp;

                finger.SetActive(false);
            };

            TextureFactory.Instance.GetTexture("Assets/Resource/texture/ui_shouzhi.png", dele, true);
        }

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

        if (sds.gameObjectNameArr.Length > 1)
        {
            battleManager.GetMouseUp();
        }

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

        GuideSDS sds = StaticData.GetData<GuideSDS>(guideIndex);

        if (sds.gameObjectNameArr.Length == 1)
        {
            finger.SetActive(true);

            finger.transform.position = GameObject.Find(sds.gameObjectNameArr[0]).transform.position;
        }
        else if (sds.gameObjectNameArr.Length == 2)
        {
            finger.SetActive(true);

            FingerMove(GameObject.Find(sds.gameObjectNameArr[0]).transform.position, GameObject.Find(sds.gameObjectNameArr[1]).transform.position);
        }
    }

    private static void FingerMove(Vector3 _start, Vector3 _end)
    {
        Action start = null;

        Action<float> to = null;

        Action end = null;

        to = delegate (float _value)
        {
            finger.transform.position = Vector3.Lerp(_start, _end, _value);
        };

        end = delegate ()
        {
            fingerTweenID = -1;

            finger.transform.position = _start;

            start();
        };

        start = delegate ()
        {
            fingerTweenID = SuperTween.Instance.To(0, 1, 1, to, end);
        };

        end();
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
        finger.SetActive(false);

        if (fingerTweenID != -1)
        {
            SuperTween.Instance.Remove(fingerTweenID);

            fingerTweenID = -1;
        }

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

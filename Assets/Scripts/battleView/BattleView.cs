using UnityEngine;
using gameObjectFactory;
using superFunction;
using System.Collections;

public class BattleView : UIPanel
{
    private static GameObject m_battleManagerEventGo;

    public static GameObject battleManagerEventGo
    {
        get
        {
            if (m_battleManagerEventGo == null)
            {
                m_battleManagerEventGo = new GameObject("BattleManagerEventGo");
            }

            return m_battleManagerEventGo;
        }
    }

    private BattleManager battleManager;

    public override void OnEnter()
    {
        if (battleManager == null)
        {
            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleManager.prefab", null);

            battleManager = go.GetComponent<BattleManager>();

            battleManager.Init(battleManagerEventGo);

            SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleQuit);
        }

        battleManager.RequestRefreshData();
    }

    private void BattleQuit(int _index)
    {
        UIManager.Instance.Hide(uid);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            StartGuide();
        }
    }

    private void StartGuide()
    {
        superGraphicRaycast.SuperGraphicRaycast.SetFilter(true);

        superGraphicRaycast.SuperGraphicRaycast.AddFilterTag("Guide");

        superRaycast.SuperRaycast.SetFilter(true);

        superRaycast.SuperRaycast.AddTag("Guide");





        superSequenceControl.SuperSequenceControl.Start(Sequence);
    }

    private void ProcessGuideUnit(int _index)
    {

    }

    private IEnumerator Sequence(int _index)
    {
        SuperFunction.SuperFunctionCallBack0 dele = delegate (int _indexx)
        {
            superSequenceControl.SuperSequenceControl.MoveNext(_index);
        };

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_ACTION, dele);

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_SUMMON, dele);

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_UNACTION, dele);

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_UNSUMMON, dele);

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_ROUND_OVER, dele);


        GameObject go1 = GameObject.Find("HeroCard");

        publicTools.PublicTools.SetTag(go1, "Guide");

        GameObject go2 = battleManager.mapUnitDic[2].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        superSequenceControl.SuperSequenceControl.DelayCall(0, _index);

        yield return null;

        go1 = battleManager.cardList[1].gameObject;

        publicTools.PublicTools.SetTag(go1, "Guide");

        go2 = battleManager.mapUnitDic[6].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        superSequenceControl.SuperSequenceControl.DelayCall(0, _index);

        yield return null;

        go1 = battleManager.cardList[2].gameObject;

        publicTools.PublicTools.SetTag(go1, "Guide");

        go2 = battleManager.mapUnitDic[9].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        go1 = GameObject.Find("btConfirm");

        publicTools.PublicTools.SetTag(go1, "Guide");

        yield return null;

        publicTools.PublicTools.ClearTag(go1);

        go1 = battleManager.mapUnitDic[2].gameObject;

        publicTools.PublicTools.SetTag(go1, "Guide");

        go2 = battleManager.mapUnitDic[1].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        battleManager.GetMouseUp();

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        go1 = battleManager.mapUnitDic[6].gameObject;

        publicTools.PublicTools.SetTag(go1, "Guide");

        go2 = battleManager.mapUnitDic[5].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        battleManager.GetMouseUp();

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        go1 = battleManager.mapUnitDic[9].gameObject;

        publicTools.PublicTools.SetTag(go1, "Guide");

        go2 = battleManager.mapUnitDic[8].gameObject;

        publicTools.PublicTools.SetTag(go2, "Guide");

        yield return null;

        battleManager.GetMouseUp();

        publicTools.PublicTools.ClearTag(go1);

        publicTools.PublicTools.ClearTag(go2);

        go1 = GameObject.Find("btConfirm");

        publicTools.PublicTools.SetTag(go1, "Guide");

        yield return null;

        publicTools.PublicTools.ClearTag(go1);


        superGraphicRaycast.SuperGraphicRaycast.SetFilter(false);

        superGraphicRaycast.SuperGraphicRaycast.RemoveFilterTag("Guide");

        superRaycast.SuperRaycast.SetFilter(false);

        superRaycast.SuperRaycast.RemoveTag("Guide");
    }

}

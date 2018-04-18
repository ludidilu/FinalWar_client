using UnityEngine;
using gameObjectFactory;
using superFunction;
using publicTools;
using tuple;
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
            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/BattleManager.prefab", null);

            battleManager = go.GetComponent<BattleManager>();

            PublicTools.SetTag(battleManager.quitBt, BattleGuide.TAG_NAME);

            battleManager.Init(battleManagerEventGo);

            SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleQuit);
        }

        Tuple<bool, int, IEnumerator> tuple = (Tuple<bool, int, IEnumerator>)data;

        if (!tuple.first)
        {
            int guideID = tuple.second;

            if (guideID != 0)
            {
                SuperFunction.Instance.AddOnceEventListener(battleManagerEventGo, BattleManager.BATTLE_START, StartGuide);
            }
        }
        else
        {
            battleManager.EnterReplay();

            IEnumerator enumerator = ((Tuple<bool, int, IEnumerator>)data).third;

            int id = -1;

            SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
            {
                if (!enumerator.MoveNext())
                {
                    SuperFunction.Instance.RemoveEventListener(id);

                    battleManager.ExitReplay();
                }
            };

            SuperFunction.Instance.AddOnceEventListener(battleManagerEventGo, BattleManager.BATTLE_START, dele);

            id = SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_ROUND_OVER, dele);
        }

        battleManager.RequestRefreshData();
    }

    private void StartGuide(int _index)
    {
        int guideID = (int)data;

        BattleGuide.Start(battleManager, guideID);
    }

    private void BattleQuit(int _index)
    {
        Tuple<bool, int, IEnumerator> tuple = (Tuple<bool, int, IEnumerator>)data;

        int guideID = tuple.second;

        if (guideID != 0)
        {
            BattleGuide.Over();
        }

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
        BattleGuide.Start(battleManager, 1);
    }
}
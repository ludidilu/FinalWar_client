using UnityEngine;
using gameObjectFactory;
using superFunction;

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
        int guideID = (int)data;

        if (guideID != 0)
        {


            SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
            {
                BattleGuide.Start(battleManager, guideID);
            };

            SuperFunction.Instance.AddOnceEventListener(battleManagerEventGo, BattleManager.BATTLE_START, dele);
        }

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
        BattleGuide.Start(battleManager, 1);
    }


}

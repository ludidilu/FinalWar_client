using UnityEngine;
using gameObjectFactory;
using superFunction;

public class BattleView : UIBase
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

    private GameObject mainCamera;

    private BattleManager battleManager;

    public override bool IsFullScreen()
    {
        return true;
    }

    public override void OnEnter()
    {
        mainCamera = Camera.main.gameObject;

        mainCamera.SetActive(false);

        if (battleManager != null)
        {
            battleManager.RequestRefreshData();
        }
        else
        {
            GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/BattleManager.prefab", GetBattleManager);
        }
    }

    public override void OnExit()
    {
        mainCamera.SetActive(true);
    }

    private void GetBattleManager(GameObject _go)
    {
        battleManager = _go.GetComponent<BattleManager>();

        battleManager.Init(battleManagerEventGo);

        SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleQuit);

        battleManager.RequestRefreshData();
    }

    private void BattleQuit(int _index)
    {
        UIManager.Instance.Hide(this);
    }
}

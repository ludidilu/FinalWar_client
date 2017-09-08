using FinalWar;
using System.IO;
using System;
using System.Collections.Generic;
using superFunction;

public class BattleLocal
{
    private static BattleLocal _Instance;

    public static BattleLocal Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new BattleLocal();
            }

            return _Instance;
        }
    }

    private const int mapID = 2;

    private Battle_server battleServer;

    private Action<BinaryReader> clientReceiveDataCallBack;

    public BattleLocal()
    {
        battleServer = new Battle_server(false);

        battleServer.ServerSetCallBack(ServerSendDataCallBack, ServerBattleOverCallBack);
    }

    private void ServerSendDataCallBack(bool _isMine, bool _isPush, MemoryStream _ms)
    {
        if (_isMine)
        {
            _ms.Position = 0;

            if (_isPush)
            {
                using (BinaryReader br = new BinaryReader(_ms))
                {
                    BattleManager.Instance.ReceiveData(br);
                }
            }
            else
            {
                using (BinaryReader br = new BinaryReader(_ms))
                {
                    Action<BinaryReader> tmpCb = clientReceiveDataCallBack;

                    clientReceiveDataCallBack = null;

                    tmpCb(br);
                }
            }
        }
    }

    private void ServerBattleOverCallBack(Battle.BattleResult _result)
    {

    }

    public void Start()
    {
        BattleManager.Instance.SetSendDataCallBack(GetDataFromClient);

        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_START, BattleStart);

        IList<int> mCards = StaticData.GetData<TestCardsSDS>(1).cards;

        IList<int> oCards = StaticData.GetData<TestCardsSDS>(2).cards;

        battleServer.ServerStart(mapID, mCards, oCards, true);

        BattleManager.Instance.RequestRefreshData();
    }

    private void BattleStart(int _index)
    {
        SuperFunction.Instance.AddOnceEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_OVER, BattleOver);

        BattleManager.Instance.gameObject.SetActive(true);
    }

    private void BattleOver(int _index)
    {
        BattleEntrance.Instance.Show();
    }

    private void GetDataFromClient(MemoryStream _ms, Action<BinaryReader> _clientReceiveDataCallBack)
    {
        clientReceiveDataCallBack = _clientReceiveDataCallBack;

        _ms.Position = 0;

        using (BinaryReader br = new BinaryReader(_ms))
        {
            battleServer.ServerGetPackage(br, true);
        }
    }
}
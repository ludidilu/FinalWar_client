using FinalWar;
using System.IO;
using System;
using superFunction;
using UnityEngine;

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

    private const int testID = 1;

    private Battle_server battleServer;

    private Action<BinaryReader> clientReceiveDataCallBack;

    private string saveKey;

    public BattleLocal()
    {
        saveKey = string.Format("BattleLocal:{0}", ConfigDictionary.Instance.uid);

        battleServer = new Battle_server(false);

        battleServer.ServerSetCallBack(ServerSendDataCallBack, ServerRoundOverCallBack);
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

    private void ServerRoundOverCallBack(Battle.BattleResult _result)
    {
        if (_result == Battle.BattleResult.NOT_OVER)
        {
            byte[] bytes = battleServer.ToBytes();

            string str = Convert.ToBase64String(bytes);

            PlayerPrefs.SetString(saveKey, str);

            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);

                PlayerPrefs.Save();
            }

            battleServer.ResetData();
        }
    }

    public void Start()
    {
        BattleManager.Instance.SetSendDataCallBack(GetDataFromClient);

        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_START, BattleStart);

        if (PlayerPrefs.HasKey(saveKey))
        {
            string str = PlayerPrefs.GetString(saveKey);

            byte[] bytes = Convert.FromBase64String(str);

            battleServer.FromBytes(bytes);
        }
        else
        {
            TestCardsSDS testCardSDS = StaticData.GetData<TestCardsSDS>(testID);

            battleServer.ServerStart(testCardSDS.mapID, testCardSDS.maxRoundNum, testCardSDS.mCards, testCardSDS.oCards, true);
        }

        BattleManager.Instance.RequestRefreshData();
    }

    private void BattleStart(int _index)
    {
        SuperFunction.Instance.AddOnceEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_QUIT, BattleOver);

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

    public void VerifyBattle()
    {
        Debug.Log(battleServer.VerifyBattle());
    }
}
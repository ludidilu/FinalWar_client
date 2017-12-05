using FinalWar;
using System.IO;
using System;
using System.Collections.Generic;
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
        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.ROUND_OVER, RoundOver);

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
        SuperFunction.Instance.RemoveEventListener(BattleManager.Instance.gameObject, BattleManager.ROUND_OVER, RoundOver);

        BattleEntrance.Instance.Show();

        if (PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.DeleteKey(saveKey);

            PlayerPrefs.Save();
        }

        battleServer.ResetData();
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

    private void RoundOver(int _index)
    {
        byte[] bytes = battleServer.ToBytes();

        string str = Convert.ToBase64String(bytes);

        PlayerPrefs.SetString(saveKey, str);

        PlayerPrefs.Save();
    }

    public void VerifyBattle()
    {
        Debug.Log(battleServer.VerifyBattle());
    }
}
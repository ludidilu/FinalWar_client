using System;
using superFunction;
using UnityEngine;
using FinalWar;
using System.IO;
using System.Collections.Generic;

public class BattleLocal
{
    public const string SAVE_DATA_CHANGE = "saveDataChange";

    private string saveKey;

    private Battle_server battleServer;

    private Action<BinaryReader> clientReceiveDataCallBack;

    public BattleLocal()
    {
        saveKey = string.Format("BattleLocal:{0}", ConfigDictionary.Instance.uid);

        battleServer = new Battle_server(true);

        battleServer.ServerSetCallBack(ServerSendDataCallBack, ServerRoundOverCallBack);

        Dictionary<int, MapSDS> mapDic = StaticData.GetDic<MapSDS>();

        Dictionary<int, HeroSDS> heroDic = StaticData.GetDic<HeroSDS>();

        Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS>();

        Dictionary<int, EffectSDS> effectDic = StaticData.GetDic<EffectSDS>();

        Battle.Init(mapDic, heroDic, auraDic, effectDic);
    }

    private void SaveDataChange(int _index, byte[] _bytes)
    {
        if (_bytes != null)
        {
            string str = Convert.ToBase64String(_bytes);

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
        }
    }

    public void Start()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string str = PlayerPrefs.GetString(saveKey);

            byte[] bytes = Convert.FromBase64String(str);

            StartBattle(bytes);
        }
        else
        {
            UIManager.Instance.Show<BattleChoose, Action<BattleSDS>>(Choose);
        }
    }

    private void Choose(BattleSDS _battleSDS)
    {
        UIManager.Instance.Hide<BattleChoose>();

        StartBattle(_battleSDS);
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
                    SuperFunction.Instance.DispatchEvent(BattleView.battleManagerEventGo, BattleManager.BATTLE_RECEIVE_DATA, br);
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
            battleServer.ResetData();

            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);

                PlayerPrefs.Save();
            }
        }
    }

    private void StartBattle(byte[] _bytes)
    {
        battleServer.FromBytes(_bytes);

        StartBattle();
    }

    private void StartBattle(BattleSDS _battleSDS)
    {
        battleServer.ServerStart(_battleSDS.mapID, _battleSDS.maxRoundNum, _battleSDS.mCards, _battleSDS.oCards, true);

        StartBattle();
    }

    private void StartBattle()
    {
        SuperFunction.Instance.AddEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, ClientSendData);

        UIManager.Instance.Show<BattleView>();
    }

    private void ClientSendData(int _index, MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        clientReceiveDataCallBack = _callBack;

        _ms.Position = 0;

        using (BinaryReader br = new BinaryReader(_ms))
        {
            battleServer.ServerGetPackage(br, true);
        }
    }
}
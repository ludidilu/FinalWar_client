using System;
using superFunction;
using UnityEngine;
using FinalWar;
using System.IO;
using System.Collections.Generic;
using tuple;
using System.Collections;

public class BattleLocal
{
    private string saveKey;

    private Battle_server battleServer;

    private Action<BinaryReader> clientReceiveDataCallBack;

    private int parentUid;

    private bool isGuide;

    public BattleLocal()
    {
        saveKey = string.Format("BattleLocal:{0}", ConfigDictionary.Instance.uid);

        battleServer = new Battle_server(true);

        battleServer.ServerSetCallBack(ServerSendDataCallBack, ServerRoundOverCallBack);

        Dictionary<int, MapSDS> mapDic = StaticData.GetDic<MapSDS>();

        Dictionary<int, HeroSDS> heroDic = StaticData.GetDic<HeroSDS>();

        Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS>();

        Dictionary<int, EffectSDS> effectDic = StaticData.GetDic<EffectSDS>();

        Dictionary<int, BattleSDS> battleDic = StaticData.GetDic<BattleSDS>();

        Battle.Init(mapDic, heroDic, auraDic, effectDic, battleDic);
    }

    public void Start(int _parentUid)
    {
        parentUid = _parentUid;

        if (PlayerPrefs.HasKey(saveKey))
        {
            string str = PlayerPrefs.GetString(saveKey);

            byte[] bytes = Convert.FromBase64String(str);

            StartBattle(bytes);
        }
        else
        {
            UIManager.Instance.ShowInParent<BattleChoose>(new Tuple<Action<BattleSDS>>(new Action<BattleSDS>(Choose)), parentUid);
        }
    }

    public void PlayerRecord()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string str = PlayerPrefs.GetString(saveKey);

            byte[] bytes = Convert.FromBase64String(str);

            IEnumerator enumerator = battleServer.FromBytesAndReplay(bytes);

            SuperFunction.Instance.AddOnceEventListener(BattleView.battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleOver);

            SuperFunction.Instance.AddEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, ClientSendData);

            UIManager.Instance.ShowInParent<BattleView>(new Tuple<bool, int, IEnumerator>(true, 0, enumerator), parentUid);
        }
    }

    private void Choose(BattleSDS _battleSDS)
    {
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
            if (!isGuide)
            {
                byte[] bytes = battleServer.ToBytes();

                string str = Convert.ToBase64String(bytes);

                PlayerPrefs.SetString(saveKey, str);

                PlayerPrefs.Save();
            }
        }
        else
        {
            battleServer.ResetData();

            if (!isGuide && PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);

                PlayerPrefs.Save();
            }
        }
    }

    private void StartBattle(byte[] _bytes)
    {
        battleServer.FromBytes(_bytes);

        StartBattle(0);
    }

    private void StartBattle(BattleSDS _battleSDS)
    {
        battleServer.ServerStart(_battleSDS.ID, _battleSDS.mCards, _battleSDS.oCards, true);

        StartBattle(_battleSDS.guideID);
    }

    private void StartBattle(int _guideID)
    {
        isGuide = _guideID != 0;

        SuperFunction.Instance.AddOnceEventListener(BattleView.battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleOver);

        SuperFunction.Instance.AddEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, ClientSendData);

        UIManager.Instance.ShowInParent<BattleView>(new Tuple<bool, int, IEnumerator>(false, _guideID, null), parentUid);
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

    private void BattleOver(int _index)
    {
        SuperFunction.Instance.RemoveEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, ClientSendData);
    }
}
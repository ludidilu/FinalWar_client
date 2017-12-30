using UnityEngine;
using System.IO;
using superFunction;
using Connection;
using System;

public class BattleOnline : UIBase
{
    private enum PlayerState
    {
        FREE,
        SEARCHING,
        BATTLE
    }

    private enum PackageData
    {
        PVP,
        PVE,
        CANCEL_SEARCH
    }

    [SerializeField]
    private GameObject btPVP;

    [SerializeField]
    private GameObject btPVE;

    [SerializeField]
    private GameObject btCancel;

    [SerializeField]
    private GameObject btQuit;

    private Client client;

    public override void Init()
    {
        base.Init();

        Log.Init(Debug.Log);

        client = new Client();

        client.Init(ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, ConfigDictionary.Instance.uid, ReceivePushData);
    }

    private void ReceivePushData(BinaryReader _br)
    {
        bool isBattle = _br.ReadBoolean();

        if (isBattle)
        {
            SuperFunction.Instance.DispatchEvent(BattleView.battleManagerEventGo, BattleManager.BATTLE_RECEIVE_DATA, _br);
        }
        else
        {
            ReceiveReplyData(_br);
        }
    }

    private void ReceiveReplyData(BinaryReader _br)
    {
        PlayerState playerState = (PlayerState)_br.ReadInt16();

        SetState(playerState);
    }

    private void SetState(PlayerState _state)
    {
        switch (_state)
        {
            case PlayerState.BATTLE:

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                btQuit.SetActive(true);

                SuperFunction.Instance.AddOnceEventListener(BattleView.battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleOver);

                SuperFunction.Instance.AddEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, SendBattleAction);

                UIManager.Instance.Show<BattleView>();

                break;

            case PlayerState.FREE:

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                btQuit.SetActive(true);

                break;

            case PlayerState.SEARCHING:

                btPVP.SetActive(false);

                btPVE.SetActive(false);

                btCancel.SetActive(true);

                btQuit.SetActive(false);

                break;
        }
    }

    public void EnterPVP()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(false);

                bw.Write((short)PackageData.PVP);

                client.Send(ms, ReceiveReplyData);
            }
        }
    }

    public void EnterPVE()
    {
        UIManager.Instance.Show<BattleChoose>(new Action<BattleSDS>(ChooseBattle));
    }

    private void ChooseBattle(BattleSDS _battleSDS)
    {
        UIManager.Instance.Hide<BattleChoose>();

        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(false);

                bw.Write((short)PackageData.PVE);

                bw.Write(_battleSDS.ID);

                client.Send(ms, ReceiveReplyData);
            }
        }
    }

    public void CancelPVP()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(false);

                bw.Write((short)PackageData.CANCEL_SEARCH);

                client.Send(ms, ReceiveReplyData);
            }
        }
    }

    private void SendBattleAction(int _index, MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(true);

                bw.Write(_ms.GetBuffer(), 0, (int)_ms.Length);

                client.Send(ms, _callBack);
            }
        }
    }

    public override bool IsFullScreen()
    {
        return true;
    }

    public override void OnEnter()
    {
        btPVP.SetActive(false);

        btPVE.SetActive(false);

        btCancel.SetActive(false);

        btQuit.SetActive(false);

        client.Connect(ReceiveReplyData);
    }

    public void BattleOver(int _index)
    {
        SuperFunction.Instance.RemoveEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, SendBattleAction);
    }

    public void Quit()
    {
        client.Close();

        UIManager.Instance.Hide(this);
    }

    void Update()
    {
        client.Update();
    }
}

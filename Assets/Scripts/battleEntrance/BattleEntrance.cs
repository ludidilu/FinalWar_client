using UnityEngine;
using System.IO;
using superFunction;
using Connection;
using System;

public class BattleEntrance : MonoBehaviour
{
    enum PlayerState
    {
        FREE,
        SEARCHING,
        BATTLE
    }

    internal enum PackageData
    {
        PVP,
        PVE,
        CANCEL_SEARCH
    }

    [SerializeField]
    private GameObject container;

    [SerializeField]
    private GameObject btPVP;

    [SerializeField]
    private GameObject btPVE;

    [SerializeField]
    private GameObject btCancel;

    private Client client;

    void Awake()
    {
        DebugTool.Init(Debug.Log);

        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_OVER, BattleOver);

        BattleManager.Instance.Init();

        BattleManager.Instance.SetSendDataCallBack(SendBattleAction);

        client = new Client();

        client.Init(ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, ConfigDictionary.Instance.uid, ReceivePushData, ReceiveReplyData);
    }

    private void ReceivePushData(BinaryReader _br)
    {
        bool isBattle = _br.ReadBoolean();

        if (isBattle)
        {
            if (container.activeSelf)
            {
                container.SetActive(false);
            }

            if (!BattleManager.Instance.gameObject.activeSelf)
            {
                SuperFunction.Instance.AddOnceEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_START, BattleStart);

                BattleManager.Instance.RequestRefreshData();
            }
            else
            {
                BattleManager.Instance.ReceiveData(_br);
            }
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

                container.SetActive(false);

                if (!BattleManager.Instance.gameObject.activeSelf)
                {
                    SuperFunction.Instance.AddOnceEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_START, BattleStart);

                    BattleManager.Instance.RequestRefreshData();
                }

                break;

            case PlayerState.FREE:

                container.SetActive(true);

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                break;

            case PlayerState.SEARCHING:

                container.SetActive(true);

                btPVP.SetActive(false);

                btPVE.SetActive(false);

                btCancel.SetActive(true);

                break;
        }
    }

    private void BattleStart(int _index)
    {
        BattleManager.Instance.gameObject.SetActive(true);
    }

    public void EnterPVP()
    {
        SendAction(PackageData.PVP);
    }

    public void EnterPVE()
    {
        SendAction(PackageData.PVE);
    }

    public void CancelPVP()
    {
        SendAction(PackageData.CANCEL_SEARCH);
    }

    private void SendAction(PackageData _data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(false);

                bw.Write((short)_data);

                client.Send(ms, ReceiveReplyData);
            }
        }
    }

    private void SendBattleAction(MemoryStream _ms, Action<BinaryReader> _callBack)
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

    private void BattleOver(int _index)
    {
        container.SetActive(true);

        btPVP.SetActive(true);

        btPVE.SetActive(true);

        btCancel.SetActive(false);
    }

    void Update()
    {
        client.Update();
    }
}

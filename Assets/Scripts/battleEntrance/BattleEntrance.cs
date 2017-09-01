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
    private GameObject panel;

    [SerializeField]
    private GameObject btPVP;

    [SerializeField]
    private GameObject btPVE;

    [SerializeField]
    private GameObject btCancel;

    private Client client;

    void Awake()
    {
        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_OVER, BattleOver);

        BattleManager.Instance.Init();

        BattleManager.Instance.SetSendDataCallBack(SendBattleAction);

        client = new Client();

        client.Init(ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, ConfigDictionary.Instance.uid, ReceiveData, GetActionCallBack);
    }

    private void ReceiveData(BinaryReader _br)
    {
        bool isBattle = _br.ReadBoolean();

        if (isBattle)
        {
            if (!BattleManager.Instance.gameObject.activeSelf)
            {
                return;
            }

            BattleManager.Instance.ReceiveData(_br);
        }
        else
        {
            PlayerState playerState = (PlayerState)_br.ReadUInt16();

            SetState(playerState);
        }

        switch (type)
        {
            case 0:

                short length = _br.ReadInt16();

                byte[] bytes = _br.ReadBytes(length);

                if (!BattleManager.Instance.gameObject.activeSelf)
                {
                    BattleManager.Instance.gameObject.SetActive(true);
                }

                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }

                BattleManager.Instance.ReceiveData(bytes);

                break;

            case 1:

                if (BattleManager.Instance.gameObject.activeSelf)
                {
                    BattleManager.Instance.gameObject.SetActive(false);
                }

                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                panel.SetActive(true);

                btPVP.SetActive(false);

                btPVE.SetActive(false);

                btCancel.SetActive(true);

                break;

            case 2:

                if (BattleManager.Instance.gameObject.activeSelf)
                {
                    BattleManager.Instance.gameObject.SetActive(false);
                }

                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                panel.SetActive(true);

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                break;
        }
    }

    private void SetState(PlayerState _state)
    {
        switch (_state)
        {
            case PlayerState.BATTLE:


                break;

            case PlayerState.FREE:


                break;

            case PlayerState.SEARCHING:


                break;
        }
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

                client.Send(ms, GetActionCallBack);
            }
        }
    }

    private void GetActionCallBack(BinaryReader _br)
    {

    }

    private void SendBattleAction(MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(true);

                bw.Write(_ms.GetBuffer(), 0, (int)_ms.Length);

                client.Send(_ms, _callBack);
            }
        }
    }

    private void BattleOver(int _index)
    {
        gameObject.SetActive(true);

        panel.SetActive(true);

        btPVP.SetActive(true);

        btPVE.SetActive(true);

        btCancel.SetActive(false);
    }
}

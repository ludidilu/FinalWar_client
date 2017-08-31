using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FinalWar;
using System.IO;
using System;

public class BattleLocal
{
    private Battle_server battleServer;

    private byte[] bytes = new byte[ushort.MaxValue];

    public void Start()
    {
        battleServer = new Battle_server(false);

        battleServer.ServerSetCallBack(GetDataFromServer, null);

        //battleServer.ServerStart()

        BattleManager.Instance.SetSendDataCallBack(GetDataFromClient);
    }

    private void GetDataFromClient(MemoryStream _ms)
    {
        Array.Copy(_ms.GetBuffer(), bytes, (int)_ms.Length);

        battleServer.ServerGetPackage(bytes, true);
    }

    private void GetDataFromServer(bool _isMine, MemoryStream _ms)
    {
        if (_isMine)
        {
            Array.Copy(_ms.GetBuffer(), bytes, (int)_ms.Length);

            BattleManager.Instance.ReceiveData(bytes);
        }
    }
}

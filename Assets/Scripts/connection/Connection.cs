using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

public class Connection : MonoBehaviour
{
    private static Connection _Instance;

    public static Connection Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject go = new GameObject("Connection");

                _Instance = go.AddComponent<Connection>();
            }

            return _Instance;
        }
    }

    private const int HEAD_LENGTH = 2;

    private ushort bodyLength = 0;
    private byte[] headBuffer = new byte[HEAD_LENGTH];
    private byte[] bodyBuffer = new byte[ushort.MaxValue];

    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private Action<byte[]> receiveCallBack;

    private bool isReceivingHead = true;

    public bool isConnect { private set; get; }

    public void Init(string _ip, int _port, Action<byte[]> _receiveCallBack, int _uid)
    {
        if (isConnect)
        {
            return;
        }

        receiveCallBack = _receiveCallBack;

        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(_ip), _port);

        socket.BeginConnect(ipe, ConnectCallBack, _uid);
    }

    private void ConnectCallBack(IAsyncResult _result)
    {
        socket.EndConnect(_result);

        isConnect = true;

        socket.BeginSend(BitConverter.GetBytes((int)_result.AsyncState), 0, 4, SocketFlags.None, SendCallBack, null);
    }

    void Update()
    {
        if (!isConnect)
        {
            return;
        }

        if (socket.Poll(10, SelectMode.SelectRead) && socket.Available == 0)
        {
            SuperDebug.Log("Disconnect!");

            isConnect = false;

            return;
        }

        if (isReceivingHead)
        {
            ReceiveHead();
        }
        else
        {
            ReceiveBody();
        }
    }

    private void ReceiveHead()
    {
        if (socket.Available >= HEAD_LENGTH)
        {
            socket.Receive(headBuffer, HEAD_LENGTH, SocketFlags.None);

            isReceivingHead = false;

            bodyLength = BitConverter.ToUInt16(headBuffer, 0);

            ReceiveBody();
        }
    }

    private void ReceiveBody()
    {
        if (socket.Available >= bodyLength)
        {
            socket.Receive(bodyBuffer, bodyLength, SocketFlags.None);

            isReceivingHead = true;

            byte[] result = new byte[bodyLength];

            Array.Copy(bodyBuffer, result, bodyLength);

            receiveCallBack(result);
        }
    }

    public void Send(MemoryStream _ms)
    {
        int length = HEAD_LENGTH + (int)_ms.Length;

        byte[] bytes = new byte[length];

        Array.Copy(BitConverter.GetBytes((ushort)_ms.Length), bytes, HEAD_LENGTH);

        Array.Copy(_ms.GetBuffer(), 0, bytes, HEAD_LENGTH, _ms.Length);

        socket.BeginSend(bytes, 0, length, SocketFlags.None, SendCallBack, null);
    }

    private void SendCallBack(IAsyncResult _result)
    {
        socket.EndSend(_result);
    }
}

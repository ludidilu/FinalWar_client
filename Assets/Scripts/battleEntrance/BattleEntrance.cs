using UnityEngine;
using System.IO;
using superFunction;

public class BattleEntrance : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private GameObject btPVP;

    [SerializeField]
    private GameObject btPVE;

    [SerializeField]
    private GameObject btCancel;

    void Awake()
    {
        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        SuperFunction.Instance.AddEventListener(BattleManager.Instance.gameObject, BattleManager.BATTLE_OVER, BattleOver);

        BattleManager.Instance.Init(SendBattleAction);

        Connection.Instance.Init(ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, ReceiveData, ConfigDictionary.Instance.uid);
    }

    private void ReceiveData(byte[] _bytes)
    {
        using (MemoryStream ms = new MemoryStream(_bytes))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                short type = br.ReadInt16();

                switch (type)
                {
                    case 0:

                        short length = br.ReadInt16();

                        byte[] bytes = br.ReadBytes(length);

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
        }
    }

    public void EnterPVP()
    {
        SendAction(0);
    }

    public void EnterPVE()
    {
        SendAction(1);
    }

    public void CancelPVP()
    {
        SendAction(2);
    }

    private void SendAction(short _type)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((short)1);

                bw.Write(_type);

                Connection.Instance.Send(ms);
            }
        }
    }

    private void SendBattleAction(MemoryStream _ms)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((short)0);

                short length = (short)_ms.Length;

                bw.Write(length);

                bw.Write(_ms.GetBuffer(), 0, length);

                Connection.Instance.Send(ms);
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

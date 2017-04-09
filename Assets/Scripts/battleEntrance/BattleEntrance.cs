using UnityEngine;
using System.IO;
using superFunction;

public class BattleEntrance : MonoBehaviour
{
    [SerializeField]
    private BattleManager battleManager;

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
        ConfigDictionary.Instance.LoadLocalConfig(Application.streamingAssetsPath + "/local.xml");

        StaticData.path = ConfigDictionary.Instance.table_path;

        StaticData.Dispose();

        StaticData.Load<MapSDS>("map");

        Map.Init();

		StaticData.Load<HeroTypeSDS> ("heroType");

        StaticData.Load<HeroSDS>("hero");

        StaticData.Load<SkillSDS>("skill");

        StaticData.Load<AuraSDS>("aura");

        SuperFunction.Instance.AddEventListener(battleManager.gameObject, BattleManager.BATTLE_OVER, BattleOver);

        battleManager.Init(SendBattleAction);

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

                        if (!battleManager.gameObject.activeSelf)
                        {

                            battleManager.gameObject.SetActive(true);
                        }

                        if (gameObject.activeSelf)
                        {

                            gameObject.SetActive(false);
                        }

                        battleManager.ReceiveData(bytes);

                        break;

                    case 1:

                        if (battleManager.gameObject.activeSelf)
                        {

                            battleManager.gameObject.SetActive(false);
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

                        if (battleManager.gameObject.activeSelf)
                        {

                            battleManager.gameObject.SetActive(false);
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

	private void BattleOver(int _index, object[] _objs)
    {
        gameObject.SetActive(true);

        panel.SetActive(true);

        btPVP.SetActive(true);

        btPVE.SetActive(true);

        btCancel.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

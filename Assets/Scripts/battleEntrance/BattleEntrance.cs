using UnityEngine;

public class BattleEntrance : MonoBehaviour
{
    private static BattleEntrance _Instance;

    public static BattleEntrance Instance
    {
        get
        {
            return _Instance;
        }
    }

    [SerializeField]
    private GameObject container;

    void Awake()
    {
        _Instance = this;

        container.SetActive(false);

        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        container.SetActive(true);
        BattleManager.Instance.Init();
    }

    public void Online()
    {
        container.SetActive(false);

        BattleOnline.Instance.Init();
    }

    public void Local()
    {
        container.SetActive(false);

        BattleLocal.Instance.Start();
    }

    public void Show()
    {
        container.SetActive(true);
    }
}

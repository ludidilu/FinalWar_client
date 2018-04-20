public class BattleEntrance : UIPanel
{
    private BattleLocal battleLocal;

    public override void Init()
    {
        base.Init();

        battleLocal = new BattleLocal();
    }

    public void Online()
    {
        UIManager.Instance.ShowInParent<BattleOnline>(1, uid);
    }

    public void Local()
    {
        battleLocal.Start(uid);
    }

    public void PlayRecord()
    {
        battleLocal.PlayerRecord();
    }
}

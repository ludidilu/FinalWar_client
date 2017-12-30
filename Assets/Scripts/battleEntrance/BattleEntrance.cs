public class BattleEntrance : UIBase
{
    private BattleLocal battleLocal;

    public override void Init()
    {
        base.Init();

        battleLocal = new BattleLocal();
    }

    public void Online()
    {
        UIManager.Instance.Show<BattleOnline>();
    }

    public void Local()
    {
        battleLocal.Start();
    }

    public override bool IsFullScreen()
    {
        return true;
    }
}

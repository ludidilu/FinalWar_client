public partial class HeroTypeSDS : CsvBase, IHeroTypeSDS
{
    public bool canDoAction;

    public int attackSpeed;
    public int defenseSpeed;
    public int supportSpeed;

    public int attackTimes;
    public int thread;
    public int supportSpeedBonus;

    public int GetID()
    {
        return ID;
    }

    public bool GetCanDoAction()
    {
        return canDoAction;
    }

    public int GetAttackSpeed()
    {
        return attackSpeed;
    }

    public int GetDefenseSpeed()
    {
        return defenseSpeed;
    }

    public int GetSupportSpeed()
    {
        return supportSpeed;
    }

    public int GetAttackTimes()
    {
        return attackTimes;
    }

    public int GetThread()
    {
        return thread;
    }

    public int GetSupportSpeedBonus()
    {
        return supportSpeedBonus;
    }
}

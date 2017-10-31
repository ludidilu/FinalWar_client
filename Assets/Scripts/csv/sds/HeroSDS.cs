using FinalWar;

public partial class HeroSDS : CsvBase, IHeroSDS
{
    public int hp;
    public int shield;
    public int power;
    public int cost;
    public int heroType;
    public int attack;
    public int[] shootSkills;
    public int[] supportSkills;
    public int[] auras;

    private HeroTypeSDS m_heroTypeFix;

    public HeroTypeSDS heroTypeFix
    {
        get
        {
            if (m_heroTypeFix == null)
            {
                m_heroTypeFix = StaticData.GetData<HeroTypeSDS>(heroType);
            }

            return m_heroTypeFix;
        }
    }

    public int GetID()
    {
        return ID;
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetShield()
    {
        return shield;
    }

    public int GetCost()
    {
        return cost;
    }

    public IHeroTypeSDS GetHeroType()
    {
        return heroTypeFix;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int[] GetShootSkills()
    {
        return shootSkills;
    }

    public int[] GetSupportSkills()
    {
        return supportSkills;
    }

    public int[] GetAuras()
    {
        return auras;
    }
}


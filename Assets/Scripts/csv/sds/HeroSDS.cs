public partial class HeroSDS : CsvBase, IHeroSDS
{
	public int hp;
	public int shield;
	public int power;
	public int cost;
	public int heroType;
	public int attack;
	public int skill;
	public int[] auras;

	public HeroTypeSDS heroTypeFix;

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

	public int GetSkill()
	{
		return skill;
	}

	public int[] GetAuras()
	{
		return auras;
	}

	public override void Fix()
	{
		heroTypeFix = StaticData.GetData<HeroTypeSDS>(heroType);
	}
}


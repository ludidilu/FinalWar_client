public class HeroSDS : CsvBase,IHeroSDS
{
	public int hp;
	public int cost;
	public int attack;
	public int shoot;
	public int defense;
	public int support;
	
	public int GetID()
	{
		return ID;
	}
	public int GetHp()
	{
		return hp;
	}
	public int GetCost()
	{
		return cost;
	}
	public int GetAttack()
	{
		return attack;
	}
	public int GetShoot()
	{
		return shoot;
	}
	public int GetDefense()
	{
		return defense;
	}
	public int GetSupport()
	{
		return support;
	}
}


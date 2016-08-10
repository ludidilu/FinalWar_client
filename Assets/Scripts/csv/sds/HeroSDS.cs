public class HeroSDS : CsvBase,IHeroSDS
{
	public int hp;
	public int cost;
	public int atk;
	public int shoot;
	public int def;
	
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
	public int GetAtk()
	{
		return atk;
	}
	public int GetShoot()
	{
		return shoot;
	}
	public int GetDef()
	{
		return def;
	}
}


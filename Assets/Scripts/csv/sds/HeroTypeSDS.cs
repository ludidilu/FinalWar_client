public partial class HeroTypeSDS : CsvBase, IHeroTypeSDS
{
	public bool canDoAction;
	public int thread;
	public bool canDoDamageWhenDefense;
	public bool canDoDamageWhenSupport;
	public int attackType;
	public bool canLendDamageWhenSupport;

	public bool GetCanDoAction()
	{
		return canDoAction;
	}

	public int GetThread()
	{
		return thread;
	}

	public bool GetCanDoDamageWhenDefense()
	{
		return canDoDamageWhenDefense;
	}

	public bool GetCanDoDamageWhenSupport()
	{
		return canDoDamageWhenSupport;
	}

	public AttackType GetAttackType()
	{
		return (AttackType)attackType;
	}

	public bool GetCanLendDamageWhenSupport()
	{
		return canLendDamageWhenSupport;
	}
}

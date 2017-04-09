public class HeroTypeSDS : CsvBase, IHeroTypeSDS
{
	public bool canDoAction;
	public int thread;
	public bool canDoDamageWhenDefense;
	public bool canAddAbilityWhenDefense;
	public bool canAddAbilieyWhenAttack;
	public bool canDoDamageWhenSupport;
	public bool willBeDamageByDefense;
	public bool willBeDamageBySupport;
	public bool canLendDamageWhenSupport;
	public int additionAttackType;
	public bool canDoAdditionAttackWhenNextToEnemy;

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

	public bool GetCanAddAbilityWhenDefense()
	{
		return canAddAbilityWhenDefense;
	}

	public bool GetCanAddAbilityWhenAttack()
	{
		return canAddAbilieyWhenAttack;
	}

	public bool GetCanDoDamageWhenSupport()
	{
		return canDoDamageWhenSupport;
	}

	public bool GetWillBeDamageByDefense()
	{
		return willBeDamageByDefense;
	}

	public bool GetWillBeDamageBySupport()
	{
		return willBeDamageBySupport;
	}

	public bool GetCanLendDamageWhenSupport()
	{
		return canLendDamageWhenSupport;
	}

	public AdditionAttackType GetAdditionAttackType()
	{
		return (AdditionAttackType)additionAttackType;
	}

	public bool GetCanDoAdditionAttackWhenNextToEnemy()
	{
		return canDoAdditionAttackWhenNextToEnemy;
	}
}

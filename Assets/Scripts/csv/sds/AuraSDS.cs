public class AuraSDS : CsvBase, IAuraSDS
{
	public int auraTarget;
	public int auraEffect;
	public int[] auraDatas;
	
	public AuraTarget GetAuraTarget()
	{
		return (AuraTarget)auraTarget;
	}
	public AuraEffect GetAuraEffect()
	{
		return (AuraEffect)auraEffect;
	}
	public int[] GetAuraDatas()
	{
		return auraDatas;
	}

	public string comment;
}

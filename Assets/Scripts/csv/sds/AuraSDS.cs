public class AuraSDS : CsvBase, IAuraSDS
{
	public int auraTarget;
	public int auraEffect;
	public float[] auraDatas;
	
	public AuraTarget GetAuraTarget()
	{
		return (AuraTarget)auraTarget;
	}
	public AuraEffect GetAuraEffect()
	{
		return (AuraEffect)auraEffect;
	}
	public float[] GetAuraDatas()
	{
		return auraDatas;
	}
}

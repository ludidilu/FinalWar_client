public partial class AuraSDS : CsvBase, IAuraSDS
{
	public int auraEffect;
	public int auraData;

	public AuraEffect GetAuraEffect()
	{
		return (AuraEffect)auraEffect;
	}

	public int GetAuraData()
	{
		return auraData;
	}
}

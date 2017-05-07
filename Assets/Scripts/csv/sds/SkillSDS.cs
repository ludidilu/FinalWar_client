public partial class SkillSDS : CsvBase, ISkillSDS
{
	public int skillEffect;
	public int[] skillDatas;

	public SkillEffect GetSkillEffect()
	{
		return (SkillEffect)skillEffect;
	}

	public int[] GetSkillDatas()
	{
		return skillDatas;
	}
}

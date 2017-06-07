public partial class SkillSDS : CsvBase, ISkillSDS
{
	public bool isStop;
	public int skillEffect;
	public int skillData;

	public bool GetIsStop()
	{
		return isStop;
	}

	public SkillEffect GetSkillEffect()
	{
		return (SkillEffect)skillEffect;
	}

	public int GetSkillData()
	{
		return skillData;
	}
}

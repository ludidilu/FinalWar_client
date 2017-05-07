public partial class HeroSDS : CsvBase, IHeroSDS
{
	public string name;

	private string m_comment;

	public string comment{

		get{

			if(m_comment == null){

				m_comment = string.Empty;

				SkillSDS skillSDS = StaticData.GetData<SkillSDS>(skill);

				m_comment += skillSDS.comment + "\n\n";
			}

			return m_comment;
		}
	}
}
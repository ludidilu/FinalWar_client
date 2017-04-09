public partial class HeroSDS : CsvBase, IHeroSDS
{
	public string name;

	private string m_comment;

	public string comment{

		get{

			if(m_comment == null){

				m_comment = string.Empty;

				for(int i = 0 ; i < skills.Length ; i++){

					SkillSDS skillSDS = StaticData.GetData<SkillSDS>(skills[i]);

					m_comment += skillSDS.comment + "\n\n";
				}

				for(int i = 0 ; i < auras.Length ; i++){

					AuraSDS auraSDS = StaticData.GetData<AuraSDS>(auras[i]);

					m_comment += auraSDS.comment + "\n\n";
				}
			}

			return m_comment;
		}
	}
}
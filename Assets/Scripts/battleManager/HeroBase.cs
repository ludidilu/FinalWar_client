using UnityEngine;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour
{
	public static readonly string[] abilityName = new string[]{string.Empty,"弓","援","防","助","弩","攻","建"};

    [SerializeField]
    private Image frame;

    [SerializeField]
    private Text nameText;

	[SerializeField]
	protected Text ability;

    public HeroSDS sds { get; private set; }

    public int cardUid;

    protected void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        nameText.text = sds.name;

		ability.text = abilityName [(int)_heroSDS.GetAbilityType ()];

        if (!sds.canControl)
        {
            nameText.color = Color.red;
        }
    }

    public void SetFrameVisible(bool _visible)
    {
        frame.gameObject.SetActive(_visible);
    }

    public void SetFrameColor(Color _color)
    {
        frame.color = _color;
    }
}

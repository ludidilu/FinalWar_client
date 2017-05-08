using UnityEngine;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour
{
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

		ability.text = _heroSDS.heroTypeFix.name;
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

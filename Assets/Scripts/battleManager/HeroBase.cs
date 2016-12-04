using UnityEngine;
using UnityEngine.UI;

public class HeroBase : MonoBehaviour
{
    [SerializeField]
    private Image frame;

    [SerializeField]
    private xy3d.tstd.lib.effect.Gradient gradient;

    [SerializeField]
    private Text nameText;

    public HeroSDS sds { get; private set; }

    public int cardUid;

    protected void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        nameText.text = sds.name;

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

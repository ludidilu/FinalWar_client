using UnityEngine;

public class Arrow : ShootArrow
{
    [SerializeField]
    private TextMesh tm;

    [SerializeField]
    private TextMeshOutline tmo;

    public void SetIndex(int _index)
    {
        if (_index < 0)
        {
            tm.gameObject.SetActive(false);
        }
        else
        {
            tm.gameObject.SetActive(true);

            string str = (_index + 1).ToString();

            tm.text = str;

            tmo.SetText(str);

            tm.transform.rotation = Quaternion.identity;
        }
    }

    public override void SetColor(Color _color)
    {
        base.SetColor(_color);

        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, _color.a);
    }
}

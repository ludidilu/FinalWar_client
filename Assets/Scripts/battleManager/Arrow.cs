using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private TextMesh tm;

    [SerializeField]
    private TextMeshOutline tmo;

    public void SetIndex(int _index)
    {
        string str = (_index + 1).ToString();

        tm.text = str;

        tmo.SetText(str);

        tm.transform.rotation = Quaternion.identity;
    }

    public void SetColor(Color _color)
    {
        sr.color = _color;
    }
}

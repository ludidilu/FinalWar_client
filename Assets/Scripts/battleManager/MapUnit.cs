using UnityEngine;

public class MapUnit : MonoBehaviour
{
    [SerializeField]
    private MeshColorControl meshColorControl;

    [SerializeField]
    private SpriteRenderer sr;

    public int index { private set; get; }

    public void Init(int _index)
    {
        index = _index;

        SetIconVisible(false);
    }

    public void SetMainColor(Color _color)
    {
        meshColorControl.SetColor(_color);
    }

    public void SetIconVisible(bool _visible)
    {
        sr.gameObject.SetActive(_visible);
    }

    public void SetIconColor(Color _color)
    {
        sr.color = _color;
    }
}

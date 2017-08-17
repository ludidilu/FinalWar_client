using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    public void SetIndex(int _index)
    {
        sr.material.SetFloat("_Fix", _index);
    }

    public void SetColor(Color _color)
    {
        sr.color = _color;
    }
}

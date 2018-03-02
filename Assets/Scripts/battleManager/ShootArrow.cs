using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    public virtual void SetColor(Color _color)
    {
        sr.color = _color;
    }

    public Color GetColor()
    {
        return sr.color;
    }
}

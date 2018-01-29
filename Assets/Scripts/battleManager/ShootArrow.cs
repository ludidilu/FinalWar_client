using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    public void SetColor(Color _color)
    {
        sr.color = _color;
    }
}

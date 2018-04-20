using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    protected float alpha;

    protected float alphaFix = 1;

    public void SetColor(Color _color)
    {
        alpha = _color.a;

        sr.color = new Color(_color.r, _color.g, _color.b, alpha * alphaFix);
    }

    public virtual void SetAlpha(float _alphaFix)
    {
        alphaFix = _alphaFix;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha * alphaFix);
    }
}

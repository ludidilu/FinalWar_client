using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    // Use this for initialization
    void Awake()
    {
        sr.material = Instantiate<Material>(sr.material);
    }

    public void SetIndex(int _index)
    {
        sr.material.SetFloat("_Fix", _index);
    }

    public void SetColor(Color _color)
    {
        sr.color = _color;
    }
}

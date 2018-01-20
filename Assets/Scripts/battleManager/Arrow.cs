using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private Sprite[] arr;

    public void SetIndex(int _index)
    {
        sr.sprite = arr[_index];
    }

    public void SetColor(Color _color)
    {
        sr.color = _color;
    }
}

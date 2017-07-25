using UnityEngine;
using System;

public class MapUnit : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer mainMr;

    public int index { private set; get; }

    private int index2;

    private Action<int, Color> setColorCb;

    public void Init(int _index, int _index2, Action<int, Color> _setColorCb)
    {
        index = _index;
        index2 = _index2;

        setColorCb = _setColorCb;
    }

    public void SetMainColor(Color _color)
    {
        mainMr.material.SetColor("_Color", _color);

        if (setColorCb != null)
        {
            setColorCb(index2, _color);
        }
    }
}

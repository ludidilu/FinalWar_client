using UnityEngine;
using System;

public class MapUnit : MonoBehaviour
{
    [SerializeField]
    private MeshColorControl meshColorControl;

    public int index { private set; get; }

    public void Init(int _index)
    {
        index = _index;
    }

    public void SetMainColor(Color _color)
    {
        meshColorControl.SetColor(_color);
    }
}

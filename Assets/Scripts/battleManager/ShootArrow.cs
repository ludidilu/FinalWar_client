using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShootArrow : MonoBehaviour
{
    [SerializeField]
    private Image img;

    public void SetColor(Color _color)
    {
        img.color = _color;
    }
}

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshColorControl : MonoBehaviour
{
    private Color color;

    private List<Color> colorList;

    private Mesh m_mesh;

    private Mesh mesh
    {
        get
        {
            if (m_mesh == null)
            {
                m_mesh = GetComponent<MeshFilter>().mesh;

                colorList = new List<Color>();

                for (int i = 0; i < m_mesh.vertexCount; i++)
                {
                    colorList.Add(Color.white);
                }
            }

            return m_mesh;
        }
    }

    public void SetColor(Color _color)
    {
        color = _color;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            colorList[i] = color;
        }

        mesh.SetColors(colorList);
    }

    public Color GetColor()
    {
        return color;
    }
}

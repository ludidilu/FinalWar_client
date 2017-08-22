using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SuperScale9Sprite : MonoBehaviour
{
    [SerializeField]
    private Sprite sp;

    [SerializeField]
    private float width;

    [SerializeField]
    private float height;

    private MeshRenderer mr;

    private MeshFilter mf;

    private Mesh mesh;

    private float W;
    private float H;
    private float L;
    private float R;
    private float T;
    private float B;
    private float OW;
    private float OH;

    private List<Vector3> vertices = new List<Vector3>();

    private List<Vector2> uv = new List<Vector2>();

    // Use this for initialization
    void Awake()
    {
        mr = GetComponent<MeshRenderer>();

        mf = GetComponent<MeshFilter>();

        for (int i = 0; i < 16; i++)
        {
            vertices.Add(new Vector3());

            uv.Add(new Vector2());
        }

        InitMesh();

        mf.mesh = mesh;

        if (sp != null)
        {
            SetSp(sp);
        }
    }

    private void InitMesh()
    {
        mesh = new Mesh();

        mesh.SetVertices(vertices);

        mesh.SetUVs(0, uv);

        int[] triangles = new int[54];

        for (int i = 0; i < 9; i++)
        {
            int m = i % 3 + (i / 3) * 4;

            triangles[i * 6] = m;

            triangles[i * 6 + 1] = m + 1;

            triangles[i * 6 + 2] = m + 4;

            triangles[i * 6 + 3] = m + 4;

            triangles[i * 6 + 4] = m + 1;

            triangles[i * 6 + 5] = m + 5;
        }

        mesh.SetTriangles(triangles, 0);
    }

    public void SetSp(Sprite _sp)
    {
        sp = _sp;

        OW = sp.textureRect.x;
        OH = sp.textureRect.y;
        W = sp.textureRect.width;
        H = sp.textureRect.height;
        L = sp.border.x - sp.textureRectOffset.x;
        R = sp.border.z - (sp.rect.width - sp.textureRectOffset.x - sp.textureRect.width);
        T = sp.border.w - (sp.rect.height - sp.textureRectOffset.y - sp.textureRect.height);
        B = sp.border.y - sp.textureRectOffset.y;

        uv[0] = new Vector2(OW / sp.texture.width, (OH + H) / sp.texture.height);
        uv[1] = new Vector2((OW + L) / sp.texture.width, (OH + H) / sp.texture.height);
        uv[2] = new Vector2((OW + W - R) / sp.texture.width, (OH + H) / sp.texture.height);
        uv[3] = new Vector2((OW + W) / sp.texture.width, (OH + H) / sp.texture.height);

        uv[4] = new Vector2(OW / sp.texture.width, (OH + H - T) / sp.texture.height);
        uv[5] = new Vector2((OW + L) / sp.texture.width, (OH + H - T) / sp.texture.height);
        uv[6] = new Vector2((OW + W - R) / sp.texture.width, (OH + H - T) / sp.texture.height);
        uv[7] = new Vector2((OW + W) / sp.texture.width, (OH + H - T) / sp.texture.height);

        uv[8] = new Vector2(OW / sp.texture.width, (OH + B) / sp.texture.height);
        uv[9] = new Vector2((OW + L) / sp.texture.width, (OH + B) / sp.texture.height);
        uv[10] = new Vector2((OW + W - R) / sp.texture.width, (OH + B) / sp.texture.height);
        uv[11] = new Vector2((OW + W) / sp.texture.width, (OH + B) / sp.texture.height);

        uv[12] = new Vector2(OW / sp.texture.width, OH / sp.texture.height);
        uv[13] = new Vector2((OW + L) / sp.texture.width, OH / sp.texture.height);
        uv[14] = new Vector2((OW + W - R) / sp.texture.width, OH / sp.texture.height);
        uv[15] = new Vector2((OW + W) / sp.texture.width, OH / sp.texture.height);

        mesh.SetUVs(0, uv);

        SetSize(width, height);
    }

    public void SetSize(float _width, float _height)
    {
        width = _width;

        height = _height;

        if (sp == null)
        {
            return;
        }

        float scaleX = (width - L - R) / (W - L - R);

        float scaleY = (height - T - B) / (H - T - B);

        vertices[0] = new Vector3(-((width - L - R) * 0.5f + L) / sp.pixelsPerUnit, ((height - T - B) * 0.5f + T) / sp.pixelsPerUnit);
        vertices[1] = new Vector3(-((width - L - R) * 0.5f) / sp.pixelsPerUnit, ((height - T - B) * 0.5f + T) / sp.pixelsPerUnit);
        vertices[2] = new Vector3(((width - L - R) * 0.5f) / sp.pixelsPerUnit, ((height - T - B) * 0.5f + T) / sp.pixelsPerUnit);
        vertices[3] = new Vector3(((width - L - R) * 0.5f + R) / sp.pixelsPerUnit, ((height - T - B) * 0.5f + T) / sp.pixelsPerUnit);

        vertices[4] = new Vector3(-((width - L - R) * 0.5f + L) / sp.pixelsPerUnit, ((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[5] = new Vector3(-((width - L - R) * 0.5f) / sp.pixelsPerUnit, ((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[6] = new Vector3(((width - L - R) * 0.5f) / sp.pixelsPerUnit, ((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[7] = new Vector3(((width - L - R) * 0.5f + R) / sp.pixelsPerUnit, ((height - T - B) * 0.5f) / sp.pixelsPerUnit);

        vertices[8] = new Vector3(-((width - L - R) * 0.5f + L) / sp.pixelsPerUnit, -((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[9] = new Vector3(-((width - L - R) * 0.5f) / sp.pixelsPerUnit, -((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[10] = new Vector3(((width - L - R) * 0.5f) / sp.pixelsPerUnit, -((height - T - B) * 0.5f) / sp.pixelsPerUnit);
        vertices[11] = new Vector3(((width - L - R) * 0.5f + R) / sp.pixelsPerUnit, -((height - T - B) * 0.5f) / sp.pixelsPerUnit);

        vertices[12] = new Vector3(-((width - L - R) * 0.5f + L) / sp.pixelsPerUnit, -((height - T - B) * 0.5f + B) / sp.pixelsPerUnit);
        vertices[13] = new Vector3(-((width - L - R) * 0.5f) / sp.pixelsPerUnit, -((height - T - B) * 0.5f + B) / sp.pixelsPerUnit);
        vertices[14] = new Vector3(((width - L - R) * 0.5f) / sp.pixelsPerUnit, -((height - T - B) * 0.5f + B) / sp.pixelsPerUnit);
        vertices[15] = new Vector3(((width - L - R) * 0.5f + R) / sp.pixelsPerUnit, -((height - T - B) * 0.5f + B) / sp.pixelsPerUnit);

        mesh.SetVertices(vertices);
    }
}

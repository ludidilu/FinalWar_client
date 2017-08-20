using UnityEngine;
using UnityEditor;

public static class CreatePanel
{
    [MenuItem("Test/Create Map Panel")]
    public static void CreateMapPanel()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];

        vertices[0] = new Vector3(0, 1, 0);
        vertices[1] = new Vector3(0.5f * Mathf.Sqrt(3), 0.5f, 0);
        vertices[2] = new Vector3(0.5f * Mathf.Sqrt(3), -0.5f, 0);
        vertices[3] = new Vector3(0, -1, 0);
        vertices[4] = new Vector3(-0.5f * Mathf.Sqrt(3), -0.5f, 0);
        vertices[5] = new Vector3(-0.5f * Mathf.Sqrt(3), 0.5f, 0);

        int[] triangles = new int[12];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 0;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 0;
        triangles[10] = 4;
        triangles[11] = 5;

        mesh.vertices = vertices;

        mesh.triangles = triangles;

        AssetDatabase.CreateAsset(mesh, "Assets/MapPanel.asset");
    }

    [MenuItem("Test/Create Map Panel2")]
    public static void CreateMapPanel2()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];

        vertices[0] = new Vector3(-1, 0, 0);
        vertices[1] = new Vector3(-0.5f, 0.5f * Mathf.Sqrt(3), 0);
        vertices[2] = new Vector3(0.5f, 0.5f * Mathf.Sqrt(3), 0);
        vertices[3] = new Vector3(1, 0, 0);
        vertices[4] = new Vector3(0.5f, -0.5f * Mathf.Sqrt(3), 0);
        vertices[5] = new Vector3(-0.5f, -0.5f * Mathf.Sqrt(3), 0);

        int[] triangles = new int[12];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 0;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 0;
        triangles[10] = 4;
        triangles[11] = 5;

        mesh.vertices = vertices;

        mesh.triangles = triangles;

        AssetDatabase.CreateAsset(mesh, "Assets/MapPanel.asset");
    }
}

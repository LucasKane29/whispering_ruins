using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeWarning : MonoBehaviour
{
    [Range(3, 64)]
    public int segments = 20;

    [Range(1f, 360f)]
    public float angle = 45f;

    public float radius = 3f;

    void Start() => Generate();

    void Generate()
    {
        Mesh mesh = new Mesh();

        int vertCount = segments + 2;
        Vector3[] verts = new Vector3[vertCount];
        int[] tris = new int[segments * 3];

        verts[0] = Vector3.zero;

        float halfAngle = angle / 2f;
        float step = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.Deg2Rad * (-halfAngle + step * i);
            verts[i + 1] = new Vector3(
                Mathf.Sin(a) * radius,
                0,
                Mathf.Cos(a) * radius
            );
        }

        for (int i = 0; i < segments; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
                Generate();
        };
#endif
    }
}
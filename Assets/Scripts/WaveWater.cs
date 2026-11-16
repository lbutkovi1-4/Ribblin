using UnityEngine;

// More s valovima: mreža točaka koje se dižu i spuštaju po zbroju sinusa.
// Mreža se napravi iz koda (nema je u sceni dok igra ne krene).
[RequireComponent(typeof(MeshFilter))]
public class WaveWater : MonoBehaviour
{
    public float size = 360f;        // širina mora
    public int resolution = 120;     // broj kvadratića po strani
    public float height = 0.4f;      // visina valova
    public float speed = 1.2f;

    Mesh mesh;
    Vector3[] baseVerts, verts;

    void Start()
    {
        mesh = new Mesh { name = "Waves" };
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        int n = resolution + 1;
        baseVerts = new Vector3[n * n];
        var uv = new Vector2[n * n];
        for (int z = 0; z < n; z++)
            for (int x = 0; x < n; x++)
            {
                float fx = x / (float)resolution, fz = z / (float)resolution;
                baseVerts[z * n + x] = new Vector3((fx - 0.5f) * size, 0, (fz - 0.5f) * size);
                uv[z * n + x] = new Vector2(fx, fz);
            }
        var tris = new int[resolution * resolution * 6];
        int i = 0;
        for (int z = 0; z < resolution; z++)
            for (int x = 0; x < resolution; x++)
            {
                int a = z * n + x;
                tris[i++] = a; tris[i++] = a + n; tris[i++] = a + 1;
                tris[i++] = a + 1; tris[i++] = a + n; tris[i++] = a + n + 1;
            }
        verts = (Vector3[])baseVerts.Clone();
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void Update()
    {
        if (mesh == null) return;
        float t = Time.time * speed;
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 b = baseVerts[i];
            float y = Mathf.Sin(b.x * 0.22f + t) * 0.5f
                    + Mathf.Sin(b.z * 0.18f + t * 1.3f) * 0.4f
                    + Mathf.Sin((b.x - b.z) * 0.35f + t * 2.1f) * 0.2f;
            verts[i] = new Vector3(b.x, y * height, b.z);
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }
}

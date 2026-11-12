using System.Collections.Generic;
using UnityEngine;

// Materijali i mali vizualni efekti koji se stvaraju dok igra traje.
public static class Fx
{
    static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();

    public static Material Mat(string name)
    {
        if (cache.TryGetValue(name, out var m) && m != null) return m;
        var gm = GameManager.Instance;
        if (gm != null && gm.fxMaterials != null)
            foreach (var mat in gm.fxMaterials)
                if (mat != null && mat.name == name) { cache[name] = mat; return mat; }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var created = new Material(shader) { name = name };
        created.color = Color.white;
        if (created.HasProperty("_BaseColor")) created.SetColor("_BaseColor", Color.white);
        cache[name] = created;
        return created;
    }

    public static void ClearCache() => cache.Clear();

    public static GameObject Prim(PrimitiveType type, Vector3 pos, Vector3 scale, string mat)
    {
        var go = GameObject.CreatePrimitive(type);
        Object.Destroy(go.GetComponent<Collider>());
        go.transform.position = pos;
        go.transform.localScale = scale;
        var r = go.GetComponent<Renderer>();
        r.sharedMaterial = Mat(mat);
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return go;
    }

    // Neprijatelj se raspadne na komadiće koji odlete i padnu.
    public static void Burst(Vector3 pos, string mat, int count, float size)
    {
        for (int i = 0; i < count; i++)
        {
            var go = Prim(i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Sphere, pos, Vector3.one * Random.Range(0.15f, 0.35f) * size, mat);
            var d = go.AddComponent<Debris>();
            Vector3 dir = Random.insideUnitSphere; dir.y = Mathf.Abs(dir.y) + 0.6f;
            d.velocity = dir.normalized * Random.Range(4f, 8f);
            d.spin = Random.insideUnitSphere * 720f;
        }
        // mali žuti "bljesak" zvjezdica
        for (int i = 0; i < 5; i++)
        {
            var star = Prim(PrimitiveType.Cube, pos + Vector3.up * 0.3f, Vector3.one * 0.18f, "FlagOn");
            var d = star.AddComponent<Debris>();
            float a = i / 5f * Mathf.PI * 2f;
            d.velocity = new Vector3(Mathf.Cos(a), 0.8f, Mathf.Sin(a)) * 5f;
            d.gravity = 2f;
            d.spin = new Vector3(0, 0, 900);
            d.life = 0.5f;
        }
    }

    public static void Puff(Vector3 pos, string mat, float size)
    {
        var go = Prim(PrimitiveType.Sphere, pos, Vector3.one * size * 0.3f, mat);
        go.AddComponent<Puff>().size = size;
    }
}

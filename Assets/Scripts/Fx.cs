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

    public static void Puff(Vector3 pos, string mat, float size)
    {
        var go = Prim(PrimitiveType.Sphere, pos, Vector3.one * size * 0.3f, mat);
        go.AddComponent<Puff>().size = size;
    }
}

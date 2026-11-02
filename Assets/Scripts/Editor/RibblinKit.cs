#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Boje i atmosfera jednog levela.
public class Theme
{
    public string id;
    public Dictionary<string, Color> colors = new Dictionary<string, Color>();
    public Color sky = new Color(0.55f, 0.82f, 1f);
    public Color fog = new Color(0.72f, 0.9f, 1f);
    public float fogStart = 80f, fogEnd = 300f;
    public Color ambSky = new Color(0.72f, 0.86f, 1f);
    public Color ambEq = new Color(0.75f, 0.82f, 0.78f);
    public Color ambGround = new Color(0.5f, 0.46f, 0.4f);
    public Color sun = new Color(1f, 0.96f, 0.88f);
    public float sunIntensity = 1.4f;
    public Vector3 sunEuler = new Vector3(50, -35, 0);
}

// "Kutija s alatima" za slaganje levela iz jednostavnih oblika.
public static class Kit
{
    public const string MatFolder = "Assets/Materials/Generated";
    public static Transform Root;
    public static Theme T;
    public static int FruitCount;
    static int fruitIndex;
    static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();

    static readonly Dictionary<string, Color> palette = new Dictionary<string, Color>
    {
        { "Frog", new Color(0.55f, 0.9f, 0.25f) }, { "FrogDark", new Color(0.35f, 0.7f, 0.2f) },
        { "FrogBelly", new Color(0.9f, 0.95f, 0.55f) }, { "White", new Color(0.97f, 0.97f, 0.95f) },
        { "Black", new Color(0.08f, 0.08f, 0.1f) }, { "Brown", new Color(0.5f, 0.32f, 0.18f) },
        { "Red", new Color(0.95f, 0.22f, 0.25f) }, { "Orange", new Color(1f, 0.58f, 0.15f) },
        { "Yellow", new Color(1f, 0.85f, 0.2f) }, { "Pink", new Color(1f, 0.45f, 0.7f) },
        { "Purple", new Color(0.6f, 0.35f, 0.85f) }, { "Gold", new Color(1f, 0.78f, 0.2f) },
        { "Roof", new Color(0.85f, 0.45f, 0.3f) },
        { "Top", new Color(0.45f, 0.78f, 0.32f) }, { "Side", new Color(0.55f, 0.42f, 0.3f) },
        { "SideDark", new Color(0.42f, 0.32f, 0.24f) }, { "Sand", new Color(0.98f, 0.88f, 0.62f) }, { "SandDark", new Color(0.85f, 0.72f, 0.48f) },
        { "Rock", new Color(0.62f, 0.68f, 0.78f) }, { "RockLight", new Color(0.72f, 0.77f, 0.85f) },
        { "Leaf", new Color(0.25f, 0.72f, 0.45f) }, { "LeafDark", new Color(0.15f, 0.55f, 0.4f) },
        { "Trunk", new Color(0.78f, 0.62f, 0.42f) }, { "TrunkDark", new Color(0.6f, 0.45f, 0.3f) },
        { "Water", new Color(0.2f, 0.85f, 0.85f) },
        { "Crab", new Color(0.95f, 0.35f, 0.25f) }, { "CrabBelly", new Color(1f, 0.75f, 0.55f) }, { "CrabDark", new Color(0.75f, 0.2f, 0.15f) },
        { "Toad", new Color(0.35f, 0.5f, 0.25f) }, { "ToadBelly", new Color(0.85f, 0.8f, 0.5f) },
        { "Golem", new Color(0.22f, 0.2f, 0.22f) }, { "GolemDark", new Color(0.14f, 0.12f, 0.13f) },
        { "Storm", new Color(0.3f, 0.35f, 0.55f) }, { "StormLight", new Color(0.65f, 0.7f, 0.85f) }, { "StormDark", new Color(0.2f, 0.22f, 0.38f) },
        { "SlimeGreen", new Color(0.5f, 0.95f, 0.4f) }, { "SlimePurple", new Color(0.7f, 0.4f, 0.95f) },
        { "SlimeOrange", new Color(1f, 0.45f, 0.15f) }, { "SlimeBlue", new Color(0.4f, 0.7f, 1f) },
        { "Spike", new Color(0.85f, 0.85f, 0.8f) }, { "Mushroom", new Color(0.95f, 0.3f, 0.35f) }, { "MushroomStem", new Color(0.95f, 0.9f, 0.8f) },
        { "Reed", new Color(0.55f, 0.6f, 0.3f) }, { "Cloud", new Color(1f, 1f, 1f) },
        { "Lily", new Color(0.35f, 0.7f, 0.3f) }, { "LilyDark", new Color(0.25f, 0.5f, 0.2f) },
        // efekti (koriste ih skripte dok igra traje)
        { "Tongue", new Color(1f, 0.45f, 0.5f) }, { "Dust", new Color(1f, 1f, 1f) },
        { "Warning", new Color(1f, 0.15f, 0.1f) }, { "WarningDark", new Color(0.35f, 0.05f, 0.05f) },
        { "Lightning", new Color(0.85f, 0.95f, 1f) }, { "Lava", new Color(1f, 0.4f, 0.05f) },
        { "LavaGlow", new Color(1f, 0.5f, 0.1f) }, { "LavaPlane", new Color(1f, 0.35f, 0.05f) },
        { "Shock", new Color(0.7f, 1f, 0.9f) }, { "CrystalGlow", new Color(0.3f, 0.95f, 1f) },
        { "CrystalDim", new Color(0.15f, 0.25f, 0.45f) }, { "PortalCore", new Color(0.55f, 0.4f, 1f) },
        { "FlagOn", new Color(0.3f, 1f, 0.4f) },
        { "Fish", new Color(1f, 0.55f, 0.2f) }, { "FishBlue", new Color(0.3f, 0.6f, 1f) },
        { "Snake", new Color(0.35f, 0.6f, 0.2f) }, { "SnakeStripe", new Color(0.85f, 0.75f, 0.25f) },
        { "Flame", new Color(1f, 0.45f, 0.08f) }, { "FlameCore", new Color(1f, 0.85f, 0.3f) },
        { "Bird", new Color(0.95f, 0.95f, 0.98f) }, { "BirdWing", new Color(0.55f, 0.6f, 0.7f) }, { "Firefly", new Color(1f, 0.95f, 0.4f) },
    };

    static readonly Dictionary<string, float> emissive = new Dictionary<string, float>
    {
        { "Lightning", 2.5f }, { "Lava", 2f }, { "LavaGlow", 3f }, { "LavaPlane", 1.6f }, { "Shock", 1.5f }, { "Flame", 2.5f }, { "FlameCore", 3f },
        { "CrystalGlow", 3f }, { "PortalCore", 2f }, { "Firefly", 3f }, { "Warning", 1.2f }, { "Tongue", 0.3f },
    };

    public static readonly string[] FxKeys =
        { "Tongue", "Dust", "Warning", "WarningDark", "Lightning", "Lava", "Shock", "CrystalGlow", "CrystalDim", "PortalCore", "FlagOn" };

    // ---------- scena ----------

    public static Scene NewScene(Theme theme)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        T = theme;
        mats.Clear();
        FruitCount = 0;
        fruitIndex = 0;
        EnsureFolder("Assets/Materials");
        EnsureFolder(MatFolder);

        var sunGo = new GameObject("Sun");
        var l = sunGo.AddComponent<Light>();
        l.type = LightType.Directional;
        l.color = theme.sun;
        l.intensity = theme.sunIntensity;
        l.shadows = LightShadows.Soft;
        sunGo.transform.rotation = Quaternion.Euler(theme.sunEuler);

        RenderSettings.skybox = null;
        RenderSettings.sun = l;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = theme.ambSky;
        RenderSettings.ambientEquatorColor = theme.ambEq;
        RenderSettings.ambientGroundColor = theme.ambGround;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = theme.fog;
        RenderSettings.fogStartDistance = theme.fogStart;
        RenderSettings.fogEndDistance = theme.fogEnd;

        Root = new GameObject("Level").transform;
        Random.InitState(theme.id.GetHashCode());
        return scene;
    }

    public static Camera MakeCamera(Vector3 pos)
    {
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        camGo.AddComponent<AudioListener>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = T.sky;
        cam.farClipPlane = 500f;
        cam.fieldOfView = 60f;
        camGo.transform.position = pos;
        return cam;
    }

    // ---------- materijali ----------

    public static Material Mat(string key)
    {
        string name;
        Color c;
        if (T != null && T.colors.TryGetValue(key, out c)) name = T.id + "_" + key;
        else if (palette.TryGetValue(key, out c)) name = key;
        else { name = key; c = Color.magenta; }

        if (mats.TryGetValue(name, out var cached)) return cached;
        string path = $"{MatFolder}/{name}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
        mat.color = c;
        float smooth = key.Contains("Water") || key.Contains("Crystal") || key.Contains("Slime") ? 0.75f : 0.12f;
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smooth);
        if (emissive.TryGetValue(key, out float e))
        {
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", c * e);
        }
        EditorUtility.SetDirty(mat);
        mats[name] = mat;
        return mat;
    }

    public static Material[] FxMaterials()
    {
        var list = new List<Material>();
        foreach (var k in FxKeys) list.Add(Mat(k));
        return list.ToArray();
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        int i = path.LastIndexOf('/');
        AssetDatabase.CreateFolder(path.Substring(0, i), path.Substring(i + 1));
    }

    // ---------- osnovni oblici ----------

    public static GameObject P(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, string mat,
        bool collider = true, Vector3? euler = null, bool local = false)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        var col = go.GetComponent<Collider>();
        if (!collider) Object.DestroyImmediate(col);
        else if (type == PrimitiveType.Cylinder) { Object.DestroyImmediate(col); go.AddComponent<MeshCollider>(); }
        go.transform.SetParent(parent ? parent : Root, false);
        if (local)
        {
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(euler ?? Vector3.zero);
        }
        else
        {
            go.transform.position = pos;
            go.transform.rotation = Quaternion.Euler(euler ?? Vector3.zero);
        }
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = Mat(mat);
        return go;
    }

    // vizualni dio (bez collidera), lokalne koordinate
    public static GameObject V(Transform parent, PrimitiveType type, string name, Vector3 localPos, Vector3 scale, string mat, Vector3? euler = null)
        => P(type, name, parent, localPos, scale, mat, false, euler, true);

    public static Transform Group(string name, Vector3 pos, Transform parent = null)
    {
        var g = new GameObject(name).transform;
        g.SetParent(parent ? parent : Root, false);
        g.position = pos;
        return g;
    }

    // ---------- tlo ----------

    // pos = sredina GORNJE površine
    public static Transform Ground(Vector3 top, Vector3 size, string topMat = "Top", string sideMat = "Side", Transform parent = null)
    {
        var g = Group("Ground", top, parent);
        P(PrimitiveType.Cube, "Top", g, new Vector3(0, -0.1f, 0), new Vector3(size.x + 0.25f, 0.2f, size.z + 0.25f), topMat, true, null, true);
        float h = Mathf.Max(0.05f, size.y - 0.2f);
        P(PrimitiveType.Cube, "Side", g, new Vector3(0, -0.2f - h / 2f, 0), new Vector3(size.x, h, size.z), sideMat, true, null, true);
        return g;
    }

    public static Transform RoundGround(Vector3 top, float radius, float depth, string topMat = "Top", string sideMat = "Side", Transform parent = null)
    {
        var g = Group("RoundGround", top, parent);
        P(PrimitiveType.Cylinder, "Top", g, new Vector3(0, -0.1f, 0), new Vector3(radius * 2f + 0.3f, 0.1f, radius * 2f + 0.3f), topMat, true, null, true);
        float h = Mathf.Max(0.05f, depth - 0.2f);
        P(PrimitiveType.Cylinder, "Side", g, new Vector3(0, -0.2f - h / 2f, 0), new Vector3(radius * 2f, h / 2f, radius * 2f), sideMat, true, null, true);
        return g;
    }

    public static Transform FloatIsland(Vector3 top, float radius, string topMat = "Top", string sideMat = "Side")
    {
        var g = RoundGround(top, radius, 1.6f, topMat, sideMat);
        // kamena "kapljica" ispod otoka – cijela ispod gornje površine
        float h1 = radius * 1.1f;
        V(g, PrimitiveType.Sphere, "Under", new Vector3(0, -1.4f - h1 * 0.5f, 0), new Vector3(radius * 1.9f, h1, radius * 1.9f), sideMat);
        V(g, PrimitiveType.Sphere, "Under2", new Vector3(radius * 0.15f, -1.4f - h1 * 0.95f, 0), new Vector3(radius, h1, radius), "SideDark");
        return g;
    }

    public static GameObject Box(string name, Vector3 center, Vector3 size, string mat, float rotY = 0f)
        => P(PrimitiveType.Cube, name, null, center, size, mat, true, new Vector3(0, rotY, 0));

    public static void WaterPlane(float y, string mat = "Water")
    {
        var w = P(PrimitiveType.Plane, "Water", null, new Vector3(0, y, 0), new Vector3(100, 1, 100), mat, false);
        w.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
    }

    // ---------- priroda ----------

    public static void Palm(Vector3 basePos, int seed)
    {
        var old = Random.state;
        Random.InitState(seed * 97 + 13);
        var palm = Group("Palm", basePos);
        palm.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        float lean = Random.Range(4f, 14f);
        int segments = Random.Range(5, 8);
        Vector3 p = basePos;
        Vector3 dir = Quaternion.Euler(lean, palm.eulerAngles.y, 0) * Vector3.up;
        for (int i = 0; i < segments; i++)
        {
            var seg = P(PrimitiveType.Cylinder, "Trunk", palm, p + dir * 0.5f, Vector3.one, i % 2 == 0 ? "Trunk" : "TrunkDark", i == 0);
            float w = Mathf.Lerp(0.55f, 0.35f, i / (float)segments);
            seg.transform.up = dir;
            seg.transform.localScale = new Vector3(w, 0.5f, w);
            p += dir;
            dir = Quaternion.AngleAxis(3f, Vector3.Cross(Vector3.up, dir).normalized) * dir;
        }
        for (int i = 0; i < 6; i++)
        {
            Quaternion r = Quaternion.Euler(0, i * 60f + Random.Range(-10, 10), 0);
            var leaf = P(PrimitiveType.Sphere, "Frond", palm, p + r * Vector3.forward * 1.3f + Vector3.down * 0.25f,
                new Vector3(0.9f, 0.12f, 2.8f), i % 2 == 0 ? "Leaf" : "LeafDark", false);
            leaf.transform.rotation = r * Quaternion.Euler(20, 0, 0);
        }
        for (int i = 0; i < 3; i++)
            P(PrimitiveType.Sphere, "Coconut", palm, p + Quaternion.Euler(0, i * 120, 0) * Vector3.forward * 0.3f + Vector3.down * 0.4f,
                Vector3.one * 0.35f, "Brown", false);
        Random.state = old;
    }

    public static void Bush(Vector3 pos, float size = 1f)
    {
        P(PrimitiveType.Sphere, "Bush", null, pos + Vector3.up * 0.3f * size, new Vector3(1.8f, 1.2f, 1.8f) * size, "Leaf", false);
        P(PrimitiveType.Sphere, "Bush", null, pos + new Vector3(0.9f, 0.2f, 0.4f) * size, new Vector3(1.2f, 0.9f, 1.2f) * size, "LeafDark", false);
    }

    public static void Boulder(Vector3 pos, float size = 2f, string mat = "Rock")
    {
        var go = P(PrimitiveType.Sphere, "Boulder", null, pos, new Vector3(size * 1.1f, size * 0.8f, size), mat);
        go.transform.rotation = Quaternion.Euler(Random.Range(0, 25), Random.Range(0, 360), Random.Range(0, 15));
    }

    public static void DeadTree(Vector3 pos, float height)
    {
        var g = Group("DeadTree", pos);
        P(PrimitiveType.Cylinder, "Trunk", g, new Vector3(0, height / 2f, 0), new Vector3(0.9f, height / 2f, 0.9f), "TrunkDark", true, null, true);
        for (int i = 0; i < 4; i++)
        {
            float y = height * (0.5f + i * 0.13f);
            float a = i * 95f + 20f;
            V(g, PrimitiveType.Cube, "Branch", Quaternion.Euler(0, a, 0) * new Vector3(0, 0, 1.0f) + Vector3.up * y,
                new Vector3(0.25f, 0.25f, 2.2f), "TrunkDark", new Vector3(-25, a, 0));
        }
        V(g, PrimitiveType.Sphere, "Moss", new Vector3(0, height, 0), new Vector3(2.2f, 1.2f, 2.2f), "LeafDark");
    }

    public static void Reeds(Vector3 pos)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 o = new Vector3(Random.Range(-0.6f, 0.6f), 0, Random.Range(-0.6f, 0.6f));
            float h = Random.Range(1.2f, 2.2f);
            P(PrimitiveType.Cylinder, "Reed", null, pos + o + Vector3.up * h / 2f, new Vector3(0.08f, h / 2f, 0.08f), "Reed", false,
                new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8)));
            P(PrimitiveType.Sphere, "ReedTop", null, pos + o + Vector3.up * h, new Vector3(0.18f, 0.45f, 0.18f), "Brown", false);
        }
    }

    public static void MushroomDeco(Vector3 pos, float size)
    {
        P(PrimitiveType.Cylinder, "Stem", null, pos + Vector3.up * 0.5f * size, new Vector3(0.35f, 0.5f, 0.35f) * size, "MushroomStem", false);
        P(PrimitiveType.Sphere, "Cap", null, pos + Vector3.up * 1.0f * size, new Vector3(1.3f, 0.7f, 1.3f) * size, "Mushroom", false);
    }

    public static void Cloud(Vector3 pos, float size)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 o = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-0.3f, 0.3f), Random.Range(-1f, 1f)) * size;
            P(PrimitiveType.Sphere, "Cloud", null, pos + o, new Vector3(2.5f, 1.4f, 2f) * size * Random.Range(0.8f, 1.2f), "Cloud", false);
        }
    }

    public static void CrystalDeco(Vector3 pos, float size, string mat = "LavaGlow")
    {
        P(PrimitiveType.Cube, "Crystal", null, pos + Vector3.up * size * 0.5f, new Vector3(0.5f, 1.4f, 0.5f) * size, mat, false,
            new Vector3(Random.Range(-15, 15), Random.Range(0, 90), Random.Range(-15, 15)));
    }

    public static void Firefly(Vector3 pos)
    {
        var f = P(PrimitiveType.Sphere, "Firefly", null, pos, Vector3.one * 0.15f, "Firefly", false);
        f.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        var b = f.AddComponent<Bobber>();
        b.amplitude = 0.5f;
        b.speed = Random.Range(0.8f, 1.6f);
    }

    // ---------- igra ----------

    static readonly string[] fruitMats = { "Red", "Orange", "Yellow", "Pink", "Purple" };

    public static void Fruit(Vector3 pos)
    {
        var g = Group("Fruit", pos);
        string m = fruitMats[fruitIndex++ % fruitMats.Length];
        V(g, PrimitiveType.Sphere, "Body", Vector3.zero, new Vector3(0.6f, 0.6f, 0.6f), m);
        V(g, PrimitiveType.Cube, "Stem", new Vector3(0, 0.38f, 0), new Vector3(0.06f, 0.2f, 0.06f), "Brown");
        V(g, PrimitiveType.Cube, "Leaf", new Vector3(0.1f, 0.42f, 0), new Vector3(0.22f, 0.04f, 0.12f), "Leaf");
        g.gameObject.AddComponent<Fruit>();
        FruitCount++;
    }

    public static GrapplePoint Grapple(Vector3 pos, int requiredFruit = 0)
    {
        var g = Group("Grapple_Radish", pos);
        var vis = Group("Visual", pos, g);
        V(vis, PrimitiveType.Sphere, "Radish", Vector3.zero, new Vector3(1.6f, 1.5f, 1.6f), "Red");
        V(vis, PrimitiveType.Sphere, "Shine", new Vector3(-0.35f, 0.35f, -0.55f), new Vector3(0.35f, 0.3f, 0.2f), "White");
        V(vis, PrimitiveType.Cube, "Leaf_1", new Vector3(0.2f, 1.05f, 0), new Vector3(0.25f, 0.9f, 0.08f), "Leaf", new Vector3(0, 0, -25));
        V(vis, PrimitiveType.Cube, "Leaf_2", new Vector3(-0.2f, 1.0f, 0.05f), new Vector3(0.25f, 0.8f, 0.08f), "LeafDark", new Vector3(0, 0, 25));
        V(vis, PrimitiveType.Sphere, "Root", new Vector3(0, -0.85f, 0), new Vector3(0.25f, 0.5f, 0.25f), "Red");
        var gp = g.gameObject.AddComponent<GrapplePoint>();
        gp.visual = vis;
        gp.requiredFruit = requiredFruit;
        if (requiredFruit > 0)
        {
            var locked = Group("Locked", pos, g);
            V(locked, PrimitiveType.Sphere, "Stone", Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "RockLight");
            V(locked, PrimitiveType.Cube, "Lock", new Vector3(0, 0, -0.62f), new Vector3(0.5f, 0.45f, 0.1f), "Gold");
            V(locked, PrimitiveType.Cube, "Lock", new Vector3(0, 0, 0.62f), new Vector3(0.5f, 0.45f, 0.1f), "Gold");
            gp.lockedVisual = locked.gameObject;
        }
        return gp;
    }

    public static void Checkpoint(Vector3 pos, bool bossGateSpawn = false)
    {
        var g = Group("Checkpoint", pos);
        var cp = g.gameObject.AddComponent<Checkpoint>();
        V(g, PrimitiveType.Cylinder, "Flagpole", new Vector3(1.5f, 1.2f, 0), new Vector3(0.1f, 1.2f, 0.1f), "White");
        var flag = Group("Flag", pos + new Vector3(1.5f, 2.1f, 0), g);
        V(flag, PrimitiveType.Cube, "Cloth", new Vector3(0.4f, 0, 0), new Vector3(0.8f, 0.5f, 0.05f), "Yellow");
        cp.flag = flag;
        if (bossGateSpawn) Group("BossGateSpawn", pos + Vector3.up * 0.5f);
    }

    public static void Slime(Vector3 pos, float wander, string color = "SlimeGreen", float size = 1f, bool chase = true)
    {
        var g = Group("Slime", pos);
        var model = Group("Model", pos, g);
        V(model, PrimitiveType.Sphere, "Body", new Vector3(0, 0.5f * size, 0), new Vector3(1.4f, 1.0f, 1.4f) * size, color);
        V(model, PrimitiveType.Sphere, "Eye_L", new Vector3(-0.25f, 0.75f, 0.55f) * size, Vector3.one * 0.28f * size, "White");
        V(model, PrimitiveType.Sphere, "Eye_R", new Vector3(0.25f, 0.75f, 0.55f) * size, Vector3.one * 0.28f * size, "White");
        V(model, PrimitiveType.Sphere, "Pupil_L", new Vector3(-0.25f, 0.75f, 0.67f) * size, Vector3.one * 0.13f * size, "Black");
        V(model, PrimitiveType.Sphere, "Pupil_R", new Vector3(0.25f, 0.75f, 0.67f) * size, Vector3.one * 0.13f * size, "Black");
        var s = g.gameObject.AddComponent<Slime>();
        s.model = model;
        s.wanderRadius = wander;
        s.radius = 0.7f * size;
        s.height = 1.0f * size;
        s.chase = chase;
    }

    public static void Spikes(Vector3 center, float length, bool alongX)
    {
        var g = Group("Spikes", center);
        int n = Mathf.Max(1, Mathf.RoundToInt(length / 0.8f));
        for (int i = 0; i < n; i++)
        {
            float o = (i - (n - 1) / 2f) * 0.8f;
            Vector3 lp = alongX ? new Vector3(o, 0, 0) : new Vector3(0, 0, o);
            V(g, PrimitiveType.Cube, "Spike", lp + Vector3.up * 0.3f, new Vector3(0.35f, 0.6f, 0.35f), "Spike", new Vector3(0, 45, 0));
            V(g, PrimitiveType.Cube, "Tip", lp + Vector3.up * 0.7f, new Vector3(0.18f, 0.3f, 0.18f), "Spike", new Vector3(0, 45, 0));
        }
        var zone = Group("DamageZone", center + Vector3.up * 0.45f, g);
        zone.localScale = alongX ? new Vector3(n * 0.8f, 0.9f, 0.5f) : new Vector3(0.5f, 0.9f, n * 0.8f);
        zone.gameObject.AddComponent<DamageZone>().margin = 0.25f;
    }

    public static void Mover(Vector3 top, Vector3 size, Vector3 offset, float period, float phase = 0f, string topMat = "Top", string sideMat = "Side")
    {
        var g = Group("MovingPlatform", top);
        Ground(top, size, topMat, sideMat, g);
        var mp = g.gameObject.AddComponent<MovingPlatform>();
        mp.offset = offset;
        mp.period = period;
        mp.phase = phase;
    }

    public static void LilyMover(Vector3 top, float radius, Vector3 offset, float period, float phase)
    {
        var g = Group("MovingLily", top);
        RoundGround(top, radius, 0.3f, "Lily", "LilyDark", g);
        V(g, PrimitiveType.Sphere, "Flower", new Vector3(radius * 0.5f, 0.15f, 0), new Vector3(0.4f, 0.25f, 0.4f), "Pink");
        var mp = g.gameObject.AddComponent<MovingPlatform>();
        mp.offset = offset;
        mp.period = period;
        mp.phase = phase;
    }

    public static void Lily(Vector3 top, float radius)
    {
        var g = RoundGround(top, radius, 0.3f, "Lily", "LilyDark");
        V(g, PrimitiveType.Sphere, "Flower", new Vector3(radius * 0.5f, 0.15f, 0.2f), new Vector3(0.4f, 0.25f, 0.4f), "Pink");
    }

    public static void Log(Vector3 top, Vector3 size)
    {
        Ground(top, size, "Trunk", "TrunkDark");
    }

    public static void BouncePad(Vector3 pos, float height, float size = 1f)
    {
        var g = Group("BounceMushroom", pos);
        P(PrimitiveType.Cylinder, "Stem", g, new Vector3(0, 0.5f * size, 0), new Vector3(0.6f, 0.5f, 0.6f) * size, "MushroomStem", true, null, true);
        var cap = P(PrimitiveType.Sphere, "Cap", g, new Vector3(0, 1.1f * size, 0), new Vector3(2.4f, 0.9f, 2.4f) * size, "Mushroom", true, null, true);
        V(cap.transform, PrimitiveType.Sphere, "Dot", new Vector3(0.25f, 0.4f, 0.1f), new Vector3(0.2f, 0.25f, 0.2f), "White");
        V(cap.transform, PrimitiveType.Sphere, "Dot", new Vector3(-0.2f, 0.42f, -0.2f), new Vector3(0.15f, 0.2f, 0.15f), "White");
        V(cap.transform, PrimitiveType.Sphere, "Dot", new Vector3(-0.1f, 0.4f, 0.28f), new Vector3(0.12f, 0.2f, 0.12f), "White");
        var bp = g.gameObject.AddComponent<BouncePad>();
        bp.bounceHeight = height;
        bp.cap = cap.transform;
    }

    public static void Geyser(Vector3 pos, float offset, float height = 6f)
    {
        var g = Group("Geyser", pos);
        V(g, PrimitiveType.Cylinder, "Vent", new Vector3(0, 0.03f, 0), new Vector3(2f, 0.04f, 2f), "GolemDark");
        V(g, PrimitiveType.Cylinder, "VentGlow", new Vector3(0, 0.05f, 0), new Vector3(1.2f, 0.04f, 1.2f), "LavaGlow");
        var col = V(g, PrimitiveType.Cylinder, "Fire", Vector3.zero, new Vector3(1.8f, 0.01f, 1.8f), "Lava");
        col.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        var gy = g.gameObject.AddComponent<Geyser>();
        gy.column = col.transform;
        gy.offset = offset;
        gy.height = height;
    }

    public static void SpinnerBar(Vector3 pos, float length, float speed)
    {
        P(PrimitiveType.Cylinder, "SpinnerPost", null, pos + Vector3.up * 0.6f, new Vector3(0.8f, 0.6f, 0.8f), "GolemDark");
        var g = Group("Spinner", pos + Vector3.up * 0.85f);
        var sp = g.gameObject.AddComponent<Spinner>();
        sp.speed = speed;
        var bar = V(g, PrimitiveType.Cube, "Bar", Vector3.zero, new Vector3(length, 0.4f, 0.4f), "Red");
        bar.AddComponent<DamageZone>().margin = 0.3f;
        V(g, PrimitiveType.Sphere, "End", new Vector3(length / 2f, 0, 0), Vector3.one * 0.7f, "Yellow");
        V(g, PrimitiveType.Sphere, "End", new Vector3(-length / 2f, 0, 0), Vector3.one * 0.7f, "Yellow");
    }

    public static GameObject Portal(Vector3 pos, string next, Vector3? faceTowards = null)
    {
        var root = new GameObject("Portal");
        root.transform.SetParent(Root, false);
        root.transform.position = pos;
        if (faceTowards.HasValue)
        {
            Vector3 d = faceTowards.Value - pos; d.y = 0;
            if (d.sqrMagnitude > 0.01f) root.transform.rotation = Quaternion.LookRotation(d);
        }
        var p = root.AddComponent<Portal>();
        p.nextScene = next;
        var ring = Group("Ring", pos + Vector3.up * 2.2f, root.transform);
        for (int i = 0; i < 14; i++)
        {
            float a = i / 14f * Mathf.PI * 2f;
            V(ring, PrimitiveType.Cube, "Stone", new Vector3(Mathf.Cos(a) * 1.9f, Mathf.Sin(a) * 1.9f, 0), Vector3.one * 0.5f, "Gold",
                new Vector3(0, 0, a * Mathf.Rad2Deg));
        }
        V(root.transform, PrimitiveType.Sphere, "Core", new Vector3(0, 2.2f, 0), new Vector3(3.2f, 3.4f, 0.3f), "PortalCore");
        V(root.transform, PrimitiveType.Cylinder, "Base", new Vector3(0, 0.05f, 0), new Vector3(3f, 0.05f, 3f), "Gold");
        p.ring = ring;
        return root;
    }

    // ---------- životinje i ukrasi ----------

    public static void Music(string track)
    {
        new GameObject("Music").AddComponent<MusicPlayer>().trackName = track;
    }

    public static void Fish(Vector3 home, float waterY, string mat = "Fish")
    {
        var g = Group("Fish", home);
        var model = Group("Model", home, g);
        model.localScale = Vector3.one * 1.8f;
        V(model, PrimitiveType.Sphere, "Body", Vector3.zero, new Vector3(0.45f, 0.55f, 1.1f), mat);
        V(model, PrimitiveType.Cube, "Tail", new Vector3(0, 0, -0.62f), new Vector3(0.06f, 0.5f, 0.35f), mat, new Vector3(0, 0, 0));
        V(model, PrimitiveType.Cube, "Fin", new Vector3(0, 0.3f, 0), new Vector3(0.05f, 0.25f, 0.4f), mat);
        V(model, PrimitiveType.Sphere, "Eye", new Vector3(0.18f, 0.08f, 0.35f), Vector3.one * 0.12f, "Black");
        V(model, PrimitiveType.Sphere, "Eye", new Vector3(-0.18f, 0.08f, 0.35f), Vector3.one * 0.12f, "Black");
        var f = g.gameObject.AddComponent<JumpingFish>();
        f.waterY = waterY;
        f.model = model;
    }

    public static void BeachCrab(Vector3 pos, float radius)
    {
        var g = Group("BeachCrab", pos);
        var model = Group("Model", pos, g);
        V(model, PrimitiveType.Sphere, "Shell", new Vector3(0, 0.25f, 0), new Vector3(0.7f, 0.35f, 0.55f), "Crab");
        foreach (float sx in new[] { -1f, 1f })
        {
            V(model, PrimitiveType.Sphere, "Claw", new Vector3(sx * 0.45f, 0.28f, 0.3f), new Vector3(0.25f, 0.2f, 0.3f), "Crab");
            V(model, PrimitiveType.Sphere, "Eye", new Vector3(sx * 0.12f, 0.5f, 0.2f), Vector3.one * 0.1f, "Black");
            for (int i = 0; i < 2; i++)
                V(model, PrimitiveType.Cube, "Leg", new Vector3(sx * 0.4f, 0.1f, -0.1f + i * 0.18f), new Vector3(0.3f, 0.05f, 0.05f), "CrabDark", new Vector3(0, 0, sx * 30));
        }
        var c = g.gameObject.AddComponent<Critter>();
        c.radius = radius;
        c.speed = 1.8f;
        c.sideways = true;
        c.model = model;
    }

    public static void Turtle(Vector3 pos, float radius)
    {
        var g = Group("Turtle", pos);
        var model = Group("Model", pos, g);
        V(model, PrimitiveType.Sphere, "Shell", new Vector3(0, 0.3f, 0), new Vector3(0.9f, 0.45f, 1.1f), "LeafDark");
        V(model, PrimitiveType.Sphere, "Head", new Vector3(0, 0.25f, 0.65f), new Vector3(0.3f, 0.25f, 0.35f), "Leaf");
        foreach (float sx in new[] { -1f, 1f })
            foreach (float sz in new[] { -1f, 1f })
                V(model, PrimitiveType.Sphere, "Leg", new Vector3(sx * 0.4f, 0.1f, sz * 0.35f), new Vector3(0.22f, 0.15f, 0.3f), "Leaf");
        var c = g.gameObject.AddComponent<Critter>();
        c.radius = radius;
        c.speed = 0.5f;
        c.model = model;
    }

    public static void Snake(Vector3 pos, float radius)
    {
        var g = Group("Snake", pos);
        var segs = new List<Transform>();
        int n = 9;
        for (int i = 0; i < n; i++)
        {
            float sz = i == 0 ? 0.42f : Mathf.Lerp(0.36f, 0.16f, i / (float)n);
            var seg = Group(i == 0 ? "Head" : "Body", pos, g);
            V(seg, PrimitiveType.Sphere, "Ball", new Vector3(0, sz * 0.45f, 0), new Vector3(sz, sz * 0.8f, sz * (i == 0 ? 1.3f : 1f)), i % 2 == 0 ? "Snake" : "SnakeStripe");
            if (i == 0)
            {
                V(seg, PrimitiveType.Sphere, "Eye", new Vector3(0.12f, 0.3f, 0.15f), Vector3.one * 0.1f, "Yellow");
                V(seg, PrimitiveType.Sphere, "Eye", new Vector3(-0.12f, 0.3f, 0.15f), Vector3.one * 0.1f, "Yellow");
                V(seg, PrimitiveType.Cube, "Tongue", new Vector3(0, 0.15f, 0.35f), new Vector3(0.05f, 0.02f, 0.2f), "Red");
            }
            segs.Add(seg);
        }
        var sn = g.gameObject.AddComponent<Snake>();
        sn.segments = segs.ToArray();
        sn.radius = radius;
    }

    public static void Flame(Vector3 pos, float size = 1f)
    {
        var g = Group("Flame", pos);
        var parts = new List<Transform>();
        parts.Add(V(g, PrimitiveType.Sphere, "Outer", new Vector3(0, 0.6f, 0) * size, new Vector3(0.9f, 1.5f, 0.9f) * size, "Flame").transform);
        parts.Add(V(g, PrimitiveType.Sphere, "Inner", new Vector3(0, 0.5f, 0) * size, new Vector3(0.5f, 0.9f, 0.5f) * size, "FlameCore").transform);
        parts.Add(V(g, PrimitiveType.Sphere, "Tip", new Vector3(0.1f, 1.3f, 0) * size, new Vector3(0.35f, 0.6f, 0.35f) * size, "Flame").transform);
        foreach (var t in parts) t.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        g.gameObject.AddComponent<Flame>().parts = parts.ToArray();
    }

    public static void Bird(Vector3 center, float radius, float height, float speed)
    {
        var g = Group("Bird", center + new Vector3(radius, height, 0));
        V(g, PrimitiveType.Sphere, "Body", Vector3.zero, new Vector3(0.45f, 0.4f, 1.0f), "Bird");
        V(g, PrimitiveType.Sphere, "Head", new Vector3(0, 0.12f, 0.5f), Vector3.one * 0.32f, "Bird");
        V(g, PrimitiveType.Cube, "Beak", new Vector3(0, 0.1f, 0.72f), new Vector3(0.08f, 0.06f, 0.2f), "Orange");
        var wl = Group("WingL", g.position, g); wl.localPosition = new Vector3(-0.2f, 0.1f, 0);
        var wr = Group("WingR", g.position, g); wr.localPosition = new Vector3(0.2f, 0.1f, 0);
        V(wl, PrimitiveType.Cube, "Wing", new Vector3(-0.6f, 0, 0), new Vector3(1.2f, 0.05f, 0.45f), "BirdWing");
        V(wr, PrimitiveType.Cube, "Wing", new Vector3(0.6f, 0, 0), new Vector3(1.2f, 0.05f, 0.45f), "BirdWing");
        var b = g.gameObject.AddComponent<FlyingBird>();
        b.center = center;
        b.radius = radius;
        b.height = height;
        b.speed = speed;
        b.wingL = wl;
        b.wingR = wr;
    }

    // ---------- žabica, kamera, game manager ----------

    public static Transform FrogModel(Transform parent)
    {
        var model = new GameObject("Model").transform;
        model.SetParent(parent, false);
        V(model, PrimitiveType.Sphere, "Body", new Vector3(0, 0.55f, 0), new Vector3(1.0f, 0.85f, 1.1f), "Frog");
        V(model, PrimitiveType.Sphere, "Belly", new Vector3(0, 0.45f, 0.25f), new Vector3(0.75f, 0.6f, 0.7f), "FrogBelly");
        V(model, PrimitiveType.Sphere, "Eye_L", new Vector3(-0.25f, 1.0f, 0.25f), Vector3.one * 0.38f, "Frog");
        V(model, PrimitiveType.Sphere, "Eye_R", new Vector3(0.25f, 1.0f, 0.25f), Vector3.one * 0.38f, "Frog");
        V(model, PrimitiveType.Sphere, "EyeWhite_L", new Vector3(-0.25f, 1.03f, 0.38f), new Vector3(0.24f, 0.24f, 0.18f), "White");
        V(model, PrimitiveType.Sphere, "EyeWhite_R", new Vector3(0.25f, 1.03f, 0.38f), new Vector3(0.24f, 0.24f, 0.18f), "White");
        V(model, PrimitiveType.Sphere, "Pupil_L", new Vector3(-0.25f, 1.04f, 0.46f), new Vector3(0.11f, 0.13f, 0.06f), "Black");
        V(model, PrimitiveType.Sphere, "Pupil_R", new Vector3(0.25f, 1.04f, 0.46f), new Vector3(0.11f, 0.13f, 0.06f), "Black");
        V(model, PrimitiveType.Cube, "Backpack", new Vector3(0, 0.65f, -0.5f), new Vector3(0.6f, 0.55f, 0.3f), "Brown");
        V(model, PrimitiveType.Cube, "Strap", new Vector3(0, 0.95f, -0.38f), new Vector3(0.62f, 0.08f, 0.08f), "TrunkDark");
        V(model, PrimitiveType.Sphere, "Foot_L", new Vector3(-0.35f, 0.1f, 0.15f), new Vector3(0.35f, 0.15f, 0.5f), "FrogDark");
        V(model, PrimitiveType.Sphere, "Foot_R", new Vector3(0.35f, 0.1f, 0.15f), new Vector3(0.35f, 0.15f, 0.5f), "FrogDark");
        V(model, PrimitiveType.Sphere, "Cheek_L", new Vector3(-0.38f, 0.7f, 0.42f), new Vector3(0.15f, 0.1f, 0.05f), "Pink");
        V(model, PrimitiveType.Sphere, "Cheek_R", new Vector3(0.38f, 0.7f, 0.42f), new Vector3(0.15f, 0.1f, 0.05f), "Pink");
        return model;
    }

    public static GameObject Player(Vector3 spawn, float respawnY, string title, string subtitle, string next, int required, bool showControls, string music)
    {
        var frog = new GameObject("Ribblin_Frog");
        frog.transform.position = spawn;
        var cc = frog.AddComponent<CharacterController>();
        cc.height = 1.2f;
        cc.radius = 0.45f;
        cc.center = new Vector3(0, 0.6f, 0);
        cc.stepOffset = 0.4f;
        cc.skinWidth = 0.05f;
        var fc = frog.AddComponent<FrogController>();
        fc.respawnY = respawnY;
        fc.model = FrogModel(frog.transform);
        var tg = frog.AddComponent<TongueGrapple>();
        tg.tongueMaterial = Mat("Tongue");
        frog.AddComponent<PlayerHealth>();

        var cam = MakeCamera(spawn + new Vector3(0, 3, -7));
        var tpc = cam.gameObject.AddComponent<ThirdPersonCamera>();
        tpc.target = frog.transform;
        fc.cameraTransform = cam.transform;

        var gm = new GameObject("GameManager").AddComponent<GameManager>();
        gm.levelTitle = title;
        gm.levelSubtitle = subtitle;
        gm.nextScene = next;
        gm.fruitRequired = required;
        gm.showControls = showControls;
        gm.fxMaterials = FxMaterials();
        Music(music);
        return frog;
    }
}
#endif

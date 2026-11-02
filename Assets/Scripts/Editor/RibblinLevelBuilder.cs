#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Izbornik u Unityju: Ribblin → Izgradi cijelu igru
// Složi izbornik, 4 levela s bossovima i završni ekran, i postavi ih u Build Settings.
public static class RibblinLevelBuilder
{
    const string Dir = "Assets/Levels";
    static readonly string[] SceneOrder = { "Ribblin_Menu", "Level1_Otok", "Level2_Mocvara", "Level3_Vulkan", "Level4_Nebo", "Ribblin_End" };
    static readonly Dictionary<string, int> fruitReport = new Dictionary<string, int>();

    [MenuItem("Ribblin/Izgradi cijelu igru", priority = 0)]
    public static void BuildAll()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        fruitReport.Clear();
        BuildMenu();
        BuildLevel1();
        BuildLevel2();
        BuildLevel3();
        BuildLevel4();
        BuildEnd();
        SetBuildSettings();
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene($"{Dir}/Ribblin_Menu.unity");
        string report = "";
        foreach (var kv in fruitReport) report += $"{kv.Key}: {kv.Value} voća  ";
        Debug.Log("Ribblin: cijela igra je izgrađena! " + report);
    }

    [MenuItem("Ribblin/Izgradi samo/Izbornik")] static void M0() { Single(BuildMenu); }
    [MenuItem("Ribblin/Izgradi samo/Level 1 – Otok")] static void M1() { Single(BuildLevel1); }
    [MenuItem("Ribblin/Izgradi samo/Level 2 – Močvara")] static void M2() { Single(BuildLevel2); }
    [MenuItem("Ribblin/Izgradi samo/Level 3 – Vulkan")] static void M3() { Single(BuildLevel3); }
    [MenuItem("Ribblin/Izgradi samo/Level 4 – Nebo")] static void M4() { Single(BuildLevel4); }
    [MenuItem("Ribblin/Izgradi samo/Kraj")] static void M5() { Single(BuildEnd); }

    static void Single(System.Action build)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        build();
        SetBuildSettings();
        AssetDatabase.SaveAssets();
    }

    static void Save(UnityEngine.SceneManagement.Scene scene, string name)
    {
        if (!AssetDatabase.IsValidFolder(Dir)) AssetDatabase.CreateFolder("Assets", "Levels");
        EditorSceneManager.SaveScene(scene, $"{Dir}/{name}.unity");
        fruitReport[name] = Kit.FruitCount;
    }

    static void SetBuildSettings()
    {
        var list = new List<EditorBuildSettingsScene>();
        foreach (var n in SceneOrder)
        {
            string path = $"{Dir}/{n}.unity";
            if (System.IO.File.Exists(path)) list.Add(new EditorBuildSettingsScene(path, true));
        }
        EditorBuildSettings.scenes = list.ToArray();
    }

    // =====================================================================
    // LEVEL 1 – TROPSKI OTOK   (boss: Captain Pinchbeard)
    // =====================================================================
    static void BuildLevel1()
    {
        var t = new Theme { id = "L1" };
        var scene = Kit.NewScene(t);
        Kit.WaterPlane(-0.3f);

        // glavni otok
        Kit.Ground(new Vector3(0, 0.3f, 0), new Vector3(60, 4, 60), "Sand", "SandDark");
        Kit.Ground(new Vector3(0, 0.6f, 0), new Vector3(50, 0.3f, 50), "Top", "Top");

        // velika stijena s gljivastim vrhom + stepenice
        Kit.Box("Cliff_Base", new Vector3(-12, 3.6f, 8), new Vector3(10, 6, 9), "Rock");
        Kit.Ground(new Vector3(-12, 7.75f, 8), new Vector3(14, 1.4f, 12), "Top", "Rock");
        Kit.Grapple(new Vector3(-8, 10.5f, 0.5f));
        Kit.Box("Step", new Vector3(-3.5f, 1.4f, 4), new Vector3(3, 1.6f, 3), "Rock");
        Kit.Box("Step", new Vector3(-5.5f, 3.0f, 7.5f), new Vector3(2.6f, 1.6f, 2.6f), "RockLight");
        Kit.Box("Step", new Vector3(-4.6f, 4.8f, 11.5f), new Vector3(2.4f, 1.6f, 2.4f), "Rock");
        Kit.Palm(new Vector3(-15, 7.75f, 11), 99);
        Kit.Bush(new Vector3(-8, 7.75f, 5));
        Kit.Slime(new Vector3(-13, 7.75f, 8), 4f, "SlimeOrange");

        // visoki luk
        Kit.Box("Arch_L", new Vector3(5, 9.6f, 16), new Vector3(3, 18, 3), "RockLight");
        Kit.Box("Arch_R", new Vector3(11, 9.6f, 16), new Vector3(3, 18, 3), "RockLight");
        Kit.Box("Arch_Top", new Vector3(8, 19.6f, 16), new Vector3(9, 2, 3.4f), "RockLight");
        Kit.Grapple(new Vector3(4, 12f, 9));
        Kit.Grapple(new Vector3(8, 24.5f, 14f));

        // istočna stijena
        Kit.Box("Cliff_East", new Vector3(15, 2.5f, -2), new Vector3(8, 4, 8), "Rock");
        Kit.Ground(new Vector3(15, 4.65f, -2), new Vector3(7.8f, 0.3f, 7.8f), "Top", "Top");
        Kit.Grapple(new Vector3(11.5f, 8f, -7));
        Kit.Slime(new Vector3(15, 4.65f, -2), 2.5f, "SlimeOrange");

        // neprijatelji i trnje na tlu
        Kit.Slime(new Vector3(5, 0.6f, -8), 4f, "SlimeOrange");
        Kit.Slime(new Vector3(-15, 0.6f, -12), 5f, "SlimeOrange");
        Kit.Slime(new Vector3(18, 0.6f, -14), 4f, "SlimeOrange");
        Kit.Spikes(new Vector3(8, 0.6f, 5), 4f, true);
        Kit.Spikes(new Vector3(-20, 0.6f, -2), 5f, false);

        // koliba
        Hut(new Vector3(19, 0.6f, -19));

        // sjeverni otočić
        Kit.RoundGround(new Vector3(0, 0.4f, 46), 7f, 3f, "Sand", "SandDark");
        Kit.RoundGround(new Vector3(0, 0.6f, 46), 5f, 0.4f, "Top", "Top");
        Kit.Grapple(new Vector3(0, 6f, 35));
        Kit.Grapple(new Vector3(0, 6f, 41));
        Kit.Checkpoint(new Vector3(-3, 0.6f, 45));
        Kit.Slime(new Vector3(2, 0.6f, 47), 2.5f, "SlimeOrange");

        // zapadni otočić
        Kit.RoundGround(new Vector3(-47, 0.4f, -6), 6f, 3f, "Sand", "SandDark");
        Kit.Grapple(new Vector3(-35, 6f, -5));
        Kit.Grapple(new Vector3(-41, 6.5f, -6));
        Kit.Checkpoint(new Vector3(-46, 0.4f, -9));
        Kit.Boulder(new Vector3(-49, 1.2f, -5), 2.5f);

        // južni put – kamenje u moru
        Kit.Ground(new Vector3(0, 0.8f, -34), new Vector3(3, 2, 3), "Rock", "Rock");
        Kit.Ground(new Vector3(3, 1.4f, -38.5f), new Vector3(2.6f, 2.5f, 2.6f), "RockLight", "RockLight");
        Kit.Ground(new Vector3(-0.5f, 2.0f, -43), new Vector3(2.6f, 3f, 2.6f), "Rock", "Rock");
        Kit.Ground(new Vector3(2.5f, 2.6f, -47.5f), new Vector3(2.4f, 3.5f, 2.4f), "RockLight", "RockLight");
        Kit.RoundGround(new Vector3(0, 0.6f, -56), 5f, 3f, "Sand", "SandDark");
        Kit.Palm(new Vector3(2, 0.6f, -57), 50);

        // ukrasi
        Vector3[] palms =
        {
            new Vector3(-20, 0.6f, -20), new Vector3(-24, 0.3f, -8), new Vector3(-18, 0.6f, 18), new Vector3(8, 0.6f, -8),
            new Vector3(22, 0.6f, 10), new Vector3(26, 0.3f, -4), new Vector3(-6, 0.6f, -24), new Vector3(14, 0.6f, 22),
            new Vector3(-26, 0.3f, 22), new Vector3(2, 0.6f, 48), new Vector3(-46, 0.4f, -3), new Vector3(24, 0.3f, -26),
        };
        for (int i = 0; i < palms.Length; i++) Kit.Palm(palms[i], i);
        foreach (var b in new[] { new Vector3(-16, 0.6f, -14), new Vector3(10, 0.6f, -14), new Vector3(-22, 0.6f, 10), new Vector3(4, 0.6f, 4), new Vector3(20, 0.6f, -10) })
            Kit.Bush(b);
        foreach (var r in new[] { new Vector3(6, 0.9f, -26), new Vector3(-27, 0.5f, 3), new Vector3(27, 0.5f, 18), new Vector3(-10, 0.9f, -12), new Vector3(-2, 0.5f, 28) })
            Kit.Boulder(r, 2f);
        Kit.P(PrimitiveType.Cylinder, "Buoy", null, new Vector3(22, -0.2f, 40), new Vector3(1.4f, 0.8f, 1.4f), "Red");
        Kit.P(PrimitiveType.Cylinder, "BuoyTop", null, new Vector3(22, 1.0f, 40), new Vector3(0.5f, 0.5f, 0.5f), "White");

        // ribe koje skaču oko otoka i galebovi
        Vector3[] fishSpots =
        {
            new Vector3(35, 0, 20), new Vector3(35, 0, -20), new Vector3(-35, 0, 20), new Vector3(-35, 0, -30),
            new Vector3(20, 0, 35), new Vector3(-20, 0, 35), new Vector3(20, 0, -38), new Vector3(-20, 0, -38),
            new Vector3(38, 0, -35), new Vector3(-38, 0, 38), new Vector3(12, 0, -36), new Vector3(-12, 0, -40),
            new Vector3(60, 0, 12), new Vector3(66, 0, -14),
        };
        for (int i = 0; i < fishSpots.Length; i++)
            Kit.Fish(fishSpots[i], -0.3f, i % 3 == 0 ? "FishBlue" : "Fish");
        // ribe tik uz obalu – dobro se vide s otoka
        Vector3[] shoreFish =
        {
            new Vector3(33, 0, -10), new Vector3(33, 0, 12), new Vector3(-33, 0, 8), new Vector3(-33, 0, -16),
            new Vector3(10, 0, 33), new Vector3(-12, 0, 33), new Vector3(-8, 0, -33), new Vector3(16, 0, -33),
            new Vector3(5, 0, 40), new Vector3(-40, 0, 2),
        };
        for (int i = 0; i < shoreFish.Length; i++)
            Kit.Fish(shoreFish[i], -0.3f, i % 2 == 0 ? "Fish" : "FishBlue");

        // rakovi i kornjače na plaži
        foreach (var cp in new[] { new Vector3(27.5f, 0.3f, 6), new Vector3(-27.5f, 0.3f, -12), new Vector3(8, 0.3f, -27.5f),
                                   new Vector3(-14, 0.3f, 27.5f), new Vector3(27.5f, 0.3f, -22), new Vector3(-2, 0.4f, 44) })
            Kit.BeachCrab(cp, 2f);
        Kit.Turtle(new Vector3(-27.5f, 0.3f, 14), 1.5f);
        Kit.Turtle(new Vector3(20, 0.3f, 27.5f), 1.5f);
        Kit.Turtle(new Vector3(1, 0.6f, -56), 1.5f);
        for (int i = 0; i < 5; i++)
            Kit.Bird(new Vector3(0, 0, 0), Random.Range(30f, 45f), Random.Range(14f, 22f), Random.Range(0.12f, 0.25f));

        // voće (21)
        Vector3[] fruits =
        {
            new Vector3(0, 1.4f, -12), new Vector3(-14, 1.4f, -18), new Vector3(16, 1.4f, -12), new Vector3(22, 1.4f, 2),
            new Vector3(-22, 1.4f, -24), new Vector3(10, 1.4f, 12),
            new Vector3(-10, 8.6f, 6), new Vector3(-15, 8.6f, 10), new Vector3(-4.6f, 6.3f, 11.5f),
            new Vector3(8, 21.3f, 16),
            new Vector3(14, 5.5f, -1), new Vector3(16.5f, 5.5f, -3.5f),
            new Vector3(-2, 1.4f, 46), new Vector3(3, 1.4f, 44),
            new Vector3(-45, 1.2f, -8), new Vector3(-48, 1.2f, -3),
            new Vector3(3, 2.4f, -38.5f), new Vector3(2.5f, 3.6f, -47.5f), new Vector3(-1, 1.4f, -55), new Vector3(1.5f, 1.4f, -54),
            new Vector3(19, 1.4f, -19),
        };
        foreach (var f in fruits) Kit.Fruit(f);
        int required = 14;

        // put do bossa (istok)
        Kit.Grapple(new Vector3(36, 6f, 0));
        Kit.Grapple(new Vector3(42, 6.5f, 0));
        Kit.RoundGround(new Vector3(50, 0.5f, 0), 5f, 3f, "Sand", "SandDark");
        Kit.Checkpoint(new Vector3(48, 0.5f, -2), true);
        Kit.Grapple(new Vector3(58, 6.5f, 0), required);
        Kit.Grapple(new Vector3(63.5f, 7f, 0), required);

        // arena
        Vector3 arena = new Vector3(80, 0.5f, 0);
        Kit.RoundGround(arena, 15f, 4f, "Sand", "SandDark");
        Kit.Checkpoint(arena + new Vector3(-12, 0, -3));
        var pillars = new List<Vector3>();
        foreach (var o in new[] { new Vector3(-6, 0, -6), new Vector3(6, 0, -6), new Vector3(-6, 0, 6), new Vector3(6, 0, 6) })
        {
            Vector3 p = arena + o;
            Kit.Box("Pillar", p + Vector3.up * 2f, new Vector3(2.2f, 4, 2.2f), "Rock");
            pillars.Add(p);
        }
        Kit.Palm(arena + new Vector3(10, 0, 9), 70);
        Kit.Palm(arena + new Vector3(11, 0, -8), 71);
        var portal = Kit.Portal(arena + new Vector3(10, 0, 0), "Level2_Mocvara", arena);
        BuildCrab(arena, pillars.ToArray(), portal);

        Kit.Player(new Vector3(0, 0.7f, -20), -1.2f, "Level 1", "Tropical Island", "Level2_Mocvara", required, true, "Level1");
        Save(scene, "Level1_Otok");
    }

    static void Hut(Vector3 pos)
    {
        foreach (var o in new[] { new Vector3(-2, 0, -2), new Vector3(2, 0, -2), new Vector3(-2, 0, 2), new Vector3(2, 0, 2) })
            Kit.P(PrimitiveType.Cylinder, "Pole", null, pos + o + Vector3.up * 1.5f, new Vector3(0.25f, 1.5f, 0.25f), "Trunk");
        Kit.P(PrimitiveType.Cube, "Roof", null, pos + Vector3.up * 3.3f, new Vector3(5.5f, 0.6f, 5.5f), "Roof", true, new Vector3(0, 45, 0));
        Kit.P(PrimitiveType.Cube, "RoofTop", null, pos + Vector3.up * 3.85f, new Vector3(3.2f, 0.6f, 3.2f), "Roof", true, new Vector3(0, 45, 0));
        Kit.P(PrimitiveType.Cube, "Floor", null, pos + Vector3.up * 0.1f, new Vector3(5, 0.2f, 5), "TrunkDark");
    }

    static void BuildCrab(Vector3 pos, Vector3[] pillars, GameObject portal)
    {
        var root = Kit.Group("Boss_Crab", pos);
        root.rotation = Quaternion.Euler(0, -90, 0);
        var model = Kit.Group("Model", pos, root);
        model.localRotation = Quaternion.identity;
        Kit.V(model, PrimitiveType.Sphere, "Shell", new Vector3(0, 1.3f, 0), new Vector3(4f, 1.8f, 3.2f), "Crab");
        Kit.V(model, PrimitiveType.Sphere, "Belly", new Vector3(0, 1.0f, 0.3f), new Vector3(3.4f, 1.2f, 2.8f), "CrabBelly");
        foreach (float sx in new[] { -1f, 1f })
        {
            Kit.V(model, PrimitiveType.Cylinder, "Stalk", new Vector3(sx * 0.6f, 2.3f, 1.0f), new Vector3(0.15f, 0.4f, 0.15f), "Crab");
            Kit.V(model, PrimitiveType.Sphere, "Eye", new Vector3(sx * 0.6f, 2.75f, 1.0f), Vector3.one * 0.5f, "White");
            Kit.V(model, PrimitiveType.Sphere, "Pupil", new Vector3(sx * 0.6f, 2.8f, 1.22f), Vector3.one * 0.2f, "Black");
            Kit.V(model, PrimitiveType.Sphere, "Claw", new Vector3(sx * 2.3f, 1.4f, 1.4f), new Vector3(1.3f, 1.0f, 1.6f), "Crab");
            Kit.V(model, PrimitiveType.Cube, "Pincer", new Vector3(sx * 2.3f, 1.6f, 2.35f), new Vector3(0.4f, 0.35f, 0.9f), "CrabDark", new Vector3(-15, 0, 0));
            Kit.V(model, PrimitiveType.Cube, "Pincer", new Vector3(sx * 2.3f, 1.2f, 2.35f), new Vector3(0.4f, 0.35f, 0.9f), "CrabDark", new Vector3(15, 0, 0));
            for (int i = 0; i < 3; i++)
                Kit.V(model, PrimitiveType.Cube, "Leg", new Vector3(sx * 2.1f, 0.55f, -0.8f + i * 0.8f), new Vector3(1.4f, 0.25f, 0.25f), "CrabDark", new Vector3(0, 0, sx * 30));
        }
        Kit.V(model, PrimitiveType.Cylinder, "HatBrim", new Vector3(0, 2.25f, -0.3f), new Vector3(1.6f, 0.06f, 1.6f), "Black");
        Kit.V(model, PrimitiveType.Cylinder, "Hat", new Vector3(0, 2.6f, -0.3f), new Vector3(1.0f, 0.35f, 1.0f), "Black");
        Kit.V(model, PrimitiveType.Cylinder, "HatBand", new Vector3(0, 2.4f, -0.3f), new Vector3(1.05f, 0.06f, 1.05f), "Red");

        var stars = Kit.Group("StunStars", pos + Vector3.up * 3.6f, root);
        for (int i = 0; i < 5; i++)
        {
            float a = i / 5f * Mathf.PI * 2f;
            Kit.V(stars, PrimitiveType.Sphere, "Star", new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 1.1f, Vector3.one * 0.35f, "Yellow");
        }
        stars.gameObject.SetActive(false);

        var boss = root.gameObject.AddComponent<CrabBoss>();
        boss.bossName = "Captain Pinchbeard";
        boss.maxHealth = 3;
        boss.arenaCenter = pos;
        boss.arenaRadius = 15f;
        boss.portal = portal;
        boss.pillars = pillars;
        boss.model = model;
        boss.stunStars = stars.gameObject;
        boss.hint = "Lure the Captain into a stone pillar, then jump on his head!";
    }

    // =====================================================================
    // LEVEL 2 – MOČVARA   (boss: King Croakus)
    // =====================================================================
    static void BuildLevel2()
    {
        var t = new Theme
        {
            id = "L2",
            sky = new Color(0.58f, 0.66f, 0.6f), fog = new Color(0.5f, 0.6f, 0.5f), fogStart = 30f, fogEnd = 170f,
            ambSky = new Color(0.6f, 0.72f, 0.62f), ambEq = new Color(0.45f, 0.55f, 0.45f), ambGround = new Color(0.3f, 0.3f, 0.25f),
            sun = new Color(1f, 0.85f, 0.65f), sunIntensity = 1.15f, sunEuler = new Vector3(35, -60, 0),
        };
        t.colors["Top"] = new Color(0.32f, 0.52f, 0.25f);
        t.colors["Side"] = new Color(0.33f, 0.25f, 0.17f);
        t.colors["Water"] = new Color(0.22f, 0.36f, 0.24f);
        t.colors["Leaf"] = new Color(0.3f, 0.55f, 0.25f);
        t.colors["LeafDark"] = new Color(0.2f, 0.4f, 0.2f);
        t.colors["Trunk"] = new Color(0.42f, 0.33f, 0.24f);
        t.colors["TrunkDark"] = new Color(0.27f, 0.21f, 0.16f);
        t.colors["Rock"] = new Color(0.42f, 0.44f, 0.4f);
        var scene = Kit.NewScene(t);
        Kit.WaterPlane(0f);

        // S0 – početni otok
        Kit.RoundGround(new Vector3(0, 1, 0), 9f, 4f);
        Kit.DeadTree(new Vector3(-5, 1, 3), 6f);
        Kit.Reeds(new Vector3(6, 1, -5));
        Kit.MushroomDeco(new Vector3(-4, 1, -5), 0.8f);
        Kit.Fruit(new Vector3(3, 2, 4));
        Kit.Fruit(new Vector3(-6, 2, -2));

        // S1 – lopoči
        Kit.Lily(new Vector3(2, 0.25f, 12), 1.7f);
        Kit.Lily(new Vector3(-1, 0.25f, 16.5f), 1.7f);
        Kit.Lily(new Vector3(2.5f, 0.25f, 21), 1.7f);
        Kit.Lily(new Vector3(0, 0.25f, 25.5f), 1.7f);
        Kit.Fruit(new Vector3(2.5f, 1.4f, 21));
        Kit.Fruit(new Vector3(-1, 1.4f, 16.5f));

        // S2 – otok s gljivom-trampolinom
        Kit.RoundGround(new Vector3(0, 1, 36), 7f, 4f);
        Kit.Slime(new Vector3(-3, 1, 36), 3f, "SlimePurple");
        Kit.Slime(new Vector3(3, 1, 34), 3f, "SlimePurple");
        Kit.BouncePad(new Vector3(4, 1, 38), 9f);
        Kit.P(PrimitiveType.Cylinder, "TreeTrunk", null, new Vector3(6.5f, 4.5f, 41.5f), new Vector3(1.2f, 3.5f, 1.2f), "TrunkDark");
        Kit.Ground(new Vector3(5.5f, 8f, 41.5f), new Vector3(4.5f, 0.6f, 3.5f), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(4.5f, 9f, 41.5f));
        Kit.Fruit(new Vector3(6.5f, 9f, 41.5f));
        Kit.Reeds(new Vector3(-5, 1, 39));
        Kit.Fruit(new Vector3(-4, 2, 33));

        // S3 – lopoči koji plutaju
        Kit.LilyMover(new Vector3(-4, 0.25f, 47), 1.8f, new Vector3(8, 0, 0), 4.5f, 0f);
        Kit.LilyMover(new Vector3(-4, 0.25f, 52), 1.8f, new Vector3(8, 0, 0), 4.5f, 0.5f);
        Kit.LilyMover(new Vector3(-4, 0.25f, 57), 1.8f, new Vector3(8, 0, 0), 4.5f, 0f);
        Kit.Fruit(new Vector3(0, 1.6f, 52));

        // S4 – otok sa starim stablom
        Kit.RoundGround(new Vector3(0, 1, 68), 7f, 4f);
        Kit.Checkpoint(new Vector3(0, 1, 63));
        Kit.Spikes(new Vector3(2, 1, 68), 4f, true);
        Kit.Slime(new Vector3(-3, 1, 70), 3f, "SlimePurple");
        Kit.Slime(new Vector3(4, 1, 71), 2.5f, "SlimePurple");
        Kit.DeadTree(new Vector3(-4, 1, 72), 9f);
        Kit.Grapple(new Vector3(-3, 9.5f, 70));
        Kit.Ground(new Vector3(-7, 11f, 74), new Vector3(5, 0.6f, 5), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(-8, 12f, 75));
        Kit.Fruit(new Vector3(-6, 12f, 73));
        Kit.Grapple(new Vector3(-11, 15f, 79));
        Kit.Ground(new Vector3(-13, 13f, 84), new Vector3(3.5f, 0.6f, 3.5f), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(-13, 14f, 84));
        Kit.Fruit(new Vector3(3, 2, 65));

        // S5 – balvani
        Kit.Log(new Vector3(0, 0.6f, 79), new Vector3(1.4f, 1f, 5));
        Kit.Log(new Vector3(2.5f, 0.6f, 85), new Vector3(1.4f, 1f, 4));
        Kit.Mover(new Vector3(2.5f, 0.6f, 90), new Vector3(1.4f, 1f, 3), new Vector3(-5, 0, 0), 3.5f, 0f, "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(0, 1.8f, 80));
        Kit.Fruit(new Vector3(2.5f, 1.8f, 85));

        // S6 – polje gljiva
        Kit.RoundGround(new Vector3(0, 1, 100), 8f, 4f);
        Kit.Checkpoint(new Vector3(-3, 1, 94));
        Kit.SpinnerBar(new Vector3(0, 1, 101), 9f, 75f);
        Kit.BouncePad(new Vector3(5.5f, 1, 103), 9f);
        Kit.BouncePad(new Vector3(-5.5f, 1, 103), 9f);
        Kit.Ground(new Vector3(8.5f, 7.5f, 107), new Vector3(3, 0.6f, 3), "Trunk", "TrunkDark");
        Kit.Ground(new Vector3(-8.5f, 7.5f, 107), new Vector3(3, 0.6f, 3), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(8.5f, 8.5f, 107));
        Kit.Fruit(new Vector3(-8.5f, 8.5f, 107));
        Kit.Slime(new Vector3(-4, 1, 97), 3f, "SlimePurple");
        Kit.Fruit(new Vector3(0, 2, 96));
        Kit.MushroomDeco(new Vector3(6, 1, 96), 1.2f);
        Kit.MushroomDeco(new Vector3(-6, 1, 98), 0.9f);

        int required = 12;
        // put do bossa
        Kit.Checkpoint(new Vector3(3, 1, 106), true);
        Kit.Grapple(new Vector3(0, 6.5f, 114), required);
        Kit.Grapple(new Vector3(0, 7f, 120), required);

        // arena
        Vector3 arena = new Vector3(0, 1, 137);
        Kit.RoundGround(arena, 15f, 4f);
        Kit.Checkpoint(arena + new Vector3(-3, 0, -12));
        for (int i = 0; i < 8; i++)
        {
            float a = i / 8f * Mathf.PI * 2f;
            Kit.Reeds(arena + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 14f);
        }
        Kit.DeadTree(arena + new Vector3(12, 0, 8), 7f);
        var portal = Kit.Portal(arena + new Vector3(0, 0, 11), "Level3_Vulkan", arena);
        BuildToad(arena + new Vector3(0, 0, 4), arena, portal);

        // zmije uz rubove otoka
        foreach (var sp in new[] { new Vector3(-5, 1, -4), new Vector3(5, 1, 33), new Vector3(-4, 1, 64), new Vector3(5, 1, 96), new Vector3(10, 1, 132), new Vector3(-10, 1, 143) })
            Kit.Snake(sp, 2.5f);

        // krijesnice i trska
        for (int i = 0; i < 40; i++)
            Kit.Firefly(new Vector3(Random.Range(-15f, 15f), Random.Range(1.5f, 5f), Random.Range(-5f, 150f)));
        for (int i = 0; i < 14; i++)
            Kit.Reeds(new Vector3(Random.Range(-25f, 25f), 0, Random.Range(0f, 150f)) + Vector3.right * (Random.value > 0.5f ? 12f : -12f));

        Kit.Player(new Vector3(0, 1.1f, -4), -0.6f, "Level 2", "Murky Swamp", "Level3_Vulkan", required, false, "Level2");
        Save(scene, "Level2_Mocvara");
    }

    static void BuildToad(Vector3 pos, Vector3 arena, GameObject portal)
    {
        var root = Kit.Group("Boss_ToadKing", pos);
        root.rotation = Quaternion.Euler(0, 180, 0);
        var model = Kit.Group("Model", pos, root);
        model.localRotation = Quaternion.identity;
        Kit.V(model, PrimitiveType.Sphere, "Body", new Vector3(0, 1.6f, 0), new Vector3(4.4f, 3.2f, 4f), "Toad");
        Kit.V(model, PrimitiveType.Sphere, "Belly", new Vector3(0, 1.3f, 0.9f), new Vector3(3.6f, 2.4f, 2.8f), "ToadBelly");
        foreach (float sx in new[] { -1f, 1f })
        {
            Kit.V(model, PrimitiveType.Sphere, "Eye", new Vector3(sx * 1.1f, 3.0f, 1.2f), Vector3.one * 1.2f, "Toad");
            Kit.V(model, PrimitiveType.Sphere, "EyeWhite", new Vector3(sx * 1.1f, 3.15f, 1.65f), Vector3.one * 0.75f, "Yellow");
            Kit.V(model, PrimitiveType.Sphere, "Pupil", new Vector3(sx * 1.1f, 3.2f, 1.98f), new Vector3(0.35f, 0.5f, 0.2f), "Black");
            Kit.V(model, PrimitiveType.Sphere, "Foot", new Vector3(sx * 1.8f, 0.25f, 1.4f), new Vector3(1.4f, 0.5f, 1.8f), "Toad");
            Kit.V(model, PrimitiveType.Sphere, "Wart", new Vector3(sx * 1.6f, 2.2f, -0.6f), Vector3.one * 0.5f, "LeafDark");
        }
        Kit.V(model, PrimitiveType.Cube, "Mouth", new Vector3(0, 1.95f, 1.95f), new Vector3(2.6f, 0.1f, 0.2f), "Black");
        Kit.V(model, PrimitiveType.Cylinder, "Crown", new Vector3(0, 3.35f, -0.1f), new Vector3(1.6f, 0.25f, 1.6f), "Gold");
        for (int i = 0; i < 5; i++)
        {
            float a = i / 5f * Mathf.PI * 2f;
            Kit.V(model, PrimitiveType.Cube, "CrownTip", new Vector3(Mathf.Cos(a) * 0.65f, 3.8f, Mathf.Sin(a) * 0.65f - 0.1f), new Vector3(0.25f, 0.55f, 0.25f), "Gold", new Vector3(0, a * Mathf.Rad2Deg, 0));
        }
        Kit.V(model, PrimitiveType.Sphere, "Gem", new Vector3(0, 3.4f, 0.72f), Vector3.one * 0.35f, "Red");

        var tired = Kit.Group("Tired", pos, root);
        int k = 0;
        foreach (var o in new[] { new Vector3(1.5f, 4.6f, 0), new Vector3(2.1f, 5.3f, 0), new Vector3(2.8f, 6.1f, 0) })
        {
            var z = Kit.V(tired, PrimitiveType.Cube, "Z", o, Vector3.one * (0.3f + k++ * 0.12f), "White");
            z.AddComponent<Bobber>().amplitude = 0.2f;
        }
        tired.gameObject.SetActive(false);

        model.localScale = Vector3.one * 0.8f;
        var crown = Kit.Group("CrownPoint", pos, model);
        crown.localPosition = new Vector3(0, 4.4f, -0.1f);
        var cvis = Kit.Group("Visual", pos, crown);
        cvis.localPosition = Vector3.zero;
        Kit.V(cvis, PrimitiveType.Sphere, "Orb", Vector3.zero, Vector3.one * 0.7f, "CrystalGlow");
        var crownGp = crown.gameObject.AddComponent<GrapplePoint>();
        crownGp.visual = cvis;
        crownGp.bob = false;
        crownGp.targetable = false;

        var boss = root.gameObject.AddComponent<ToadKingBoss>();
        boss.crownPoint = crownGp;
        boss.bossName = "King Croakus";
        boss.maxHealth = 3;
        boss.arenaCenter = arena;
        boss.arenaRadius = 15f;
        boss.portal = portal;
        boss.model = model;
        boss.tiredFx = tired.gameObject;
        boss.hint = "Jump over the shockwaves! When the King gets tired, jump on his head or grab his crown with your tongue.";
    }

    // =====================================================================
    // LEVEL 3 – VULKAN   (boss: Magmor the Molten)
    // =====================================================================
    static void BuildLevel3()
    {
        var t = new Theme
        {
            id = "L3",
            sky = new Color(0.3f, 0.15f, 0.12f), fog = new Color(0.42f, 0.2f, 0.12f), fogStart = 40f, fogEnd = 220f,
            ambSky = new Color(0.6f, 0.38f, 0.3f), ambEq = new Color(0.45f, 0.3f, 0.25f), ambGround = new Color(0.7f, 0.3f, 0.1f),
            sun = new Color(1f, 0.72f, 0.5f), sunIntensity = 1.25f, sunEuler = new Vector3(42, 20, 0),
        };
        t.colors["Top"] = new Color(0.3f, 0.26f, 0.28f);
        t.colors["Side"] = new Color(0.18f, 0.15f, 0.16f);
        t.colors["SideDark"] = new Color(0.12f, 0.1f, 0.1f);
        t.colors["Rock"] = new Color(0.27f, 0.23f, 0.24f);
        t.colors["RockLight"] = new Color(0.38f, 0.33f, 0.33f);
        var scene = Kit.NewScene(t);
        Kit.WaterPlane(0f, "LavaPlane");

        // S0 – start
        Kit.Ground(new Vector3(0, 3, 0), new Vector3(14, 6, 14));
        Kit.Fruit(new Vector3(4, 4, 3));
        Kit.CrystalDeco(new Vector3(-5, 3, -5), 1.5f);
        Kit.CrystalDeco(new Vector3(5, 3, -4), 1.2f);

        // S1 – bazaltni stupovi
        float[] heights = { 3.6f, 4.3f, 5.0f, 5.7f, 6.4f, 6.5f };
        Vector3[] cols = { new Vector3(0, 0, 10), new Vector3(3, 0, 14), new Vector3(0, 0, 18), new Vector3(-3, 0, 22), new Vector3(0, 0, 26), new Vector3(3, 0, 30) };
        for (int i = 0; i < cols.Length; i++)
            Kit.RoundGround(new Vector3(cols[i].x, heights[i], cols[i].z), 1.6f, heights[i] + 1f);
        Kit.Fruit(new Vector3(3, 5.3f, 14));
        Kit.Fruit(new Vector3(-3, 6.7f, 22));

        // S2 – platforma s gejzirima
        Kit.Ground(new Vector3(0, 6.5f, 40), new Vector3(12, 7.5f, 10));
        Kit.Checkpoint(new Vector3(-4, 6.5f, 36.5f));
        Kit.Geyser(new Vector3(-2.5f, 6.5f, 40), 0f);
        Kit.Geyser(new Vector3(1.5f, 6.5f, 42), 1.4f);
        Kit.Geyser(new Vector3(4, 6.5f, 38.5f), 2.6f);
        Kit.Slime(new Vector3(3, 6.5f, 43), 2.5f, "SlimeOrange");
        Kit.Fruit(new Vector3(-4.5f, 7.5f, 43.5f));
        Kit.Fruit(new Vector3(4.5f, 7.5f, 43.5f));

        // S3 – pokretne platforme iznad lave
        Kit.Mover(new Vector3(0, 6.5f, 50), new Vector3(3, 1, 3), new Vector3(0, 0, 6), 5f, 0f);
        Kit.Mover(new Vector3(-5, 6.5f, 62), new Vector3(3, 1, 3), new Vector3(10, 0, 0), 5f, 0.25f);
        Kit.Fruit(new Vector3(0, 8.2f, 59));

        // S4 – vrtuljak
        Kit.Ground(new Vector3(0, 6.5f, 72), new Vector3(14, 7.5f, 12));
        Kit.SpinnerBar(new Vector3(0, 6.5f, 72), 11f, 85f);
        Kit.Slime(new Vector3(-4, 6.5f, 75), 2f, "SlimeOrange");
        Kit.Slime(new Vector3(4.5f, 6.5f, 69), 2f, "SlimeOrange");
        Kit.Fruit(new Vector3(5.5f, 7.5f, 76.5f));
        Kit.Fruit(new Vector3(-5.5f, 7.5f, 67.5f));

        // S5 – uspon na liticu (jezikom ili po stupovima)
        Kit.Ground(new Vector3(0, 15, 86), new Vector3(14, 16, 12));
        Kit.Grapple(new Vector3(0, 10f, 77));
        Kit.Grapple(new Vector3(0, 19f, 78.5f));
        Kit.RoundGround(new Vector3(-6, 8.6f, 73.5f), 1.3f, 3f);
        Kit.RoundGround(new Vector3(-6.5f, 10.6f, 76.5f), 1.3f, 5f);
        Kit.RoundGround(new Vector3(-6, 12.6f, 79.2f), 1.3f, 7f);
        Kit.Fruit(new Vector3(-6, 13.7f, 79.2f));

        // S6 – vrh litice
        Kit.Checkpoint(new Vector3(-3, 15, 83));
        Kit.Slime(new Vector3(3, 15, 88), 3f, "SlimeOrange");
        Kit.Fruit(new Vector3(5, 16, 90));
        Kit.Fruit(new Vector3(-5, 16, 90));
        Kit.CrystalDeco(new Vector3(-5.5f, 15, 84), 1.4f);
        Kit.CrystalDeco(new Vector3(5.5f, 15, 83), 1.1f);

        // most s gejzirima
        Kit.Ground(new Vector3(0, 15, 100), new Vector3(3, 1.5f, 16));
        Kit.Geyser(new Vector3(0, 15, 96), 0f, 5f);
        Kit.Geyser(new Vector3(0, 15, 100), 1.2f, 5f);
        Kit.Geyser(new Vector3(0, 15, 104), 2.4f, 5f);
        Kit.Fruit(new Vector3(0, 16, 98));

        // S7 – ispred bossa + skriveni otočić
        Kit.Ground(new Vector3(0, 15, 114), new Vector3(10, 16, 10));
        Kit.Checkpoint(new Vector3(-3, 15, 111), true);
        Kit.Grapple(new Vector3(10, 19f, 114));
        Kit.RoundGround(new Vector3(17, 14f, 114), 3f, 15f);
        Kit.Fruit(new Vector3(16, 15f, 113));
        Kit.Fruit(new Vector3(18, 15f, 115));
        Kit.Fruit(new Vector3(-4, 16, 117));

        Kit.Fruit(new Vector3(0, 8f, 50));
        Kit.Fruit(new Vector3(-5, 6f, 2));
        Kit.Fruit(new Vector3(0, 16, 112));
        int required = 12;
        Kit.Grapple(new Vector3(0, 20f, 125), required);
        Kit.Grapple(new Vector3(0, 20.5f, 131), required);

        // arena
        Vector3 arena = new Vector3(0, 15, 148);
        Kit.RoundGround(arena, 16f, 17f);
        Kit.Checkpoint(arena + new Vector3(-3, 0, -13));
        for (int i = 0; i < 10; i++)
        {
            float a = i / 10f * Mathf.PI * 2f;
            Kit.CrystalDeco(arena + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 15f, Random.Range(1f, 1.8f));
        }
        var portal = Kit.Portal(arena + new Vector3(0, 0, 12), "Level4_Nebo", arena);
        BuildGolem(arena, portal);

        // plamenovi na rubovima i u lavi
        foreach (var fp in new[] {
            new Vector3(-6.5f, 3, 6), new Vector3(6.5f, 3, -6), new Vector3(-5.5f, 6.5f, 44.5f), new Vector3(5.5f, 6.5f, 35.5f),
            new Vector3(-6.5f, 6.5f, 77.5f), new Vector3(6.5f, 6.5f, 66.5f), new Vector3(6.5f, 15, 91.5f), new Vector3(-6.5f, 15, 81),
            new Vector3(4.5f, 15, 118.5f), new Vector3(-4.5f, 15, 109.5f) })
            Kit.Flame(fp, 1f);
        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(10f, 40f) * (Random.value > 0.5f ? 1f : -1f);
            Kit.Flame(new Vector3(x, 0f, Random.Range(-10f, 170f)), Random.Range(1.2f, 2.5f));
        }
        for (int i = 0; i < 12; i++)
        {
            float a2 = i / 12f * Mathf.PI * 2f;
            Kit.Flame(arena + new Vector3(Mathf.Cos(a2), 0, Mathf.Sin(a2)) * 15.3f, 0.9f);
        }

        Kit.Player(new Vector3(0, 3.1f, -4), 0.5f, "Level 3", "Volcano", "Level4_Nebo", required, false, "Level3");
        Save(scene, "Level3_Vulkan");
    }

    static void BuildGolem(Vector3 pos, GameObject portal)
    {
        var root = Kit.Group("Boss_Magmor", pos);
        root.rotation = Quaternion.Euler(0, 180, 0);
        var model = Kit.Group("Model", pos, root);
        model.localRotation = Quaternion.identity;
        foreach (float sx in new[] { -1f, 1f })
        {
            Kit.V(model, PrimitiveType.Cube, "Leg", new Vector3(sx * 0.9f, 1.2f, 0), new Vector3(1.2f, 2.4f, 1.4f), "Golem");
            Kit.V(model, PrimitiveType.Cube, "Arm", new Vector3(sx * 2.25f, 3.6f, 0), new Vector3(1f, 2.6f, 1f), "Golem", new Vector3(0, 0, sx * 8));
            Kit.V(model, PrimitiveType.Cube, "Eye", new Vector3(sx * 0.35f, 5.55f, 0.72f), new Vector3(0.3f, 0.18f, 0.1f), "LavaGlow");
        }
        Kit.V(model, PrimitiveType.Cube, "Torso", new Vector3(0, 3.7f, 0), new Vector3(3.2f, 2.6f, 2f), "Golem");
        Kit.V(model, PrimitiveType.Cube, "Crack", new Vector3(0, 3.7f, 1.01f), new Vector3(0.15f, 2f, 0.05f), "LavaGlow", new Vector3(0, 0, 12));
        Kit.V(model, PrimitiveType.Cube, "Crack", new Vector3(0.7f, 4.0f, 1.01f), new Vector3(0.1f, 1.2f, 0.05f), "LavaGlow", new Vector3(0, 0, -30));
        Kit.V(model, PrimitiveType.Cube, "Crack", new Vector3(-0.8f, 3.3f, 1.01f), new Vector3(0.1f, 1f, 0.05f), "LavaGlow", new Vector3(0, 0, 40));
        Kit.V(model, PrimitiveType.Cube, "Head", new Vector3(0, 5.5f, 0.1f), new Vector3(1.6f, 1.3f, 1.3f), "Golem");
        Kit.V(model, PrimitiveType.Cube, "Fist_L", new Vector3(-2.35f, 1.9f, 0.2f), new Vector3(1.3f, 1.1f, 1.3f), "GolemDark");
        var hand = Kit.V(model, PrimitiveType.Cube, "Fist_R", new Vector3(2.35f, 1.9f, 0.2f), new Vector3(1.3f, 1.1f, 1.3f), "GolemDark");

        var crystals = new List<GrapplePoint>();
        foreach (var lp in new[] { new Vector3(-2.3f, 5.5f, 0), new Vector3(2.3f, 5.5f, 0), new Vector3(0, 4.3f, -1.5f) })
        {
            var c = Kit.Group("Crystal", pos, model);
            c.localPosition = lp;
            var vis = Kit.Group("Visual", pos, c);
            vis.localPosition = Vector3.zero;
            Kit.V(vis, PrimitiveType.Cube, "Shard", Vector3.zero, new Vector3(1.0f, 1.8f, 1.0f), "CrystalDim", new Vector3(15, 45, 15));
            Kit.V(vis, PrimitiveType.Cube, "Shard", new Vector3(0.45f, -0.1f, 0.1f), new Vector3(0.6f, 1.1f, 0.6f), "CrystalDim", new Vector3(-20, 20, 30));
            var gp = c.gameObject.AddComponent<GrapplePoint>();
            gp.visual = vis;
            gp.bob = false;
            gp.targetable = false;
            crystals.Add(gp);
        }

        var boss = root.gameObject.AddComponent<GolemBoss>();
        boss.bossName = "Magmor the Molten";
        boss.maxHealth = 3;
        boss.arenaCenter = pos;
        boss.arenaRadius = 16f;
        boss.portal = portal;
        boss.model = model;
        boss.throwHand = hand.transform;
        boss.crystals = crystals.ToArray();
        boss.hint = "Stay out of the red circles! When Magmor overheats, grab a glowing crystal on him with your tongue.";
    }

    // =====================================================================
    // LEVEL 4 – NEBO   (boss: Tempestra)
    // =====================================================================
    static void BuildLevel4()
    {
        var t = new Theme
        {
            id = "L4",
            sky = new Color(0.62f, 0.78f, 1f), fog = new Color(0.88f, 0.9f, 1f), fogStart = 70f, fogEnd = 320f,
            ambSky = new Color(0.82f, 0.86f, 1f), ambEq = new Color(0.85f, 0.85f, 0.92f), ambGround = new Color(0.7f, 0.7f, 0.78f),
            sun = new Color(1f, 0.97f, 0.9f), sunIntensity = 1.4f, sunEuler = new Vector3(55, -20, 0),
        };
        t.colors["Top"] = new Color(0.55f, 0.85f, 0.45f);
        t.colors["Side"] = new Color(0.62f, 0.5f, 0.42f);
        t.colors["SideDark"] = new Color(0.45f, 0.36f, 0.3f);
        t.colors["Rock"] = new Color(0.75f, 0.72f, 0.8f);
        var scene = Kit.NewScene(t);

        // more oblaka ispod
        // oblaci samo sa strane (daleko od staze i arene), nikad ispod platformi
        for (int i = 0; i < 50; i++)
        {
            float x = Random.Range(32f, 80f) * (Random.value > 0.5f ? 1f : -1f);
            Kit.Cloud(new Vector3(x, Random.Range(-4f, 14f), Random.Range(-40f, 200f)), Random.Range(2f, 4f));
        }
        for (int i = 0; i < 8; i++)
            Kit.Bird(new Vector3(-3, 0, 80), Random.Range(40f, 60f), Random.Range(18f, 32f), Random.Range(0.1f, 0.2f));

        // S0 – start
        Kit.FloatIsland(new Vector3(0, 10, 0), 8f);
        Kit.Palm(new Vector3(-5, 10, 3), 400);
        Kit.Bush(new Vector3(4, 10, -3));
        Kit.Fruit(new Vector3(3, 11, 4));
        Kit.Fruit(new Vector3(-4, 11, -3));

        // S1 – jezikom preko praznine
        Kit.Grapple(new Vector3(0, 15f, 14));
        Kit.Grapple(new Vector3(4, 15.5f, 22));
        Kit.FloatIsland(new Vector3(8, 11, 30), 5f);
        Kit.Fruit(new Vector3(2, 15.5f, 18));
        Kit.Fruit(new Vector3(8, 12, 30));

        // S2 – dizalo
        Kit.Mover(new Vector3(8, 11, 38f), new Vector3(3, 0.8f, 3), new Vector3(0, 6, 0), 5f, 0.75f, "Top", "Side");
        Kit.FloatIsland(new Vector3(8, 17, 48), 5f);
        Kit.Slime(new Vector3(6, 17, 47), 2.5f, "SlimeBlue");

        // S3 – trampolin do visokog otoka
        Kit.BouncePad(new Vector3(10, 17, 50), 12f);
        Kit.FloatIsland(new Vector3(6, 26, 58), 4f);
        Kit.Fruit(new Vector3(5, 27, 57));
        Kit.Fruit(new Vector3(7, 27, 59));
        Kit.Grapple(new Vector3(2, 29f, 70));
        Kit.Grapple(new Vector3(-4, 28f, 83));

        // S4 – otok s vrtuljkom
        Kit.FloatIsland(new Vector3(-6, 20, 92), 7f);
        Kit.Checkpoint(new Vector3(-9, 20, 87));
        Kit.SpinnerBar(new Vector3(-6, 20, 93), 10f, 90f);
        Kit.Slime(new Vector3(-2, 20, 96), 2.5f, "SlimeBlue");
        Kit.Slime(new Vector3(-10, 20, 96), 2.5f, "SlimeBlue");
        Kit.Fruit(new Vector3(-1, 21, 89));
        Kit.Fruit(new Vector3(-11, 21, 90));

        // S5 – pokretni oblaci
        Kit.Mover(new Vector3(-10, 20, 103.5f), new Vector3(3, 0.6f, 3), new Vector3(8, 0, 0), 4f, 0f, "Top", "Side");
        Kit.Mover(new Vector3(-2, 20, 109), new Vector3(3, 0.6f, 3), new Vector3(-8, 0, 0), 4f, 0f, "Top", "Side");
        Kit.Fruit(new Vector3(-6, 21.8f, 106));

        // S6 – ispred bossa + bonus otočić
        Kit.FloatIsland(new Vector3(-6, 20, 118), 6f);
        Kit.Checkpoint(new Vector3(-9, 20, 114), true);
        Kit.Grapple(new Vector3(1, 25f, 118));
        Kit.FloatIsland(new Vector3(7, 22, 118), 3f);
        Kit.Fruit(new Vector3(6.5f, 23, 117));
        Kit.Fruit(new Vector3(7.5f, 23, 119.5f));
        Kit.Fruit(new Vector3(-3, 21, 121));
        Kit.Palm(new Vector3(-9, 20, 121), 401);

        Kit.Fruit(new Vector3(-6, 21, 97));
        Kit.Fruit(new Vector3(10, 18, 47));
        Kit.Fruit(new Vector3(4, 16.5f, 26));
        Kit.Fruit(new Vector3(-2, 21.8f, 109));
        int required = 11;
        Kit.Grapple(new Vector3(-6, 25f, 128), required);
        Kit.Grapple(new Vector3(-6, 25.5f, 134), required);

        // arena
        Vector3 arena = new Vector3(-6, 20, 151);
        Kit.FloatIsland(arena, 16f);
        Kit.Checkpoint(arena + new Vector3(-3, 0, -13));
        for (int i = 0; i < 6; i++)
        {
            float a = i / 6f * Mathf.PI * 2f + 0.3f;
            Kit.Box("Column", arena + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 15f + Vector3.up * 2.5f, new Vector3(1.2f, 5f, 1.2f), "Rock");
        }
        var portal = Kit.Portal(arena + new Vector3(0, 0, 12), "Ribblin_End", arena);
        BuildBird(arena, portal);

        Kit.Player(new Vector3(0, 10.1f, -4), -12f, "Level 4", "Sky Islands", "Ribblin_End", required, false, "Level4");
        Save(scene, "Level4_Nebo");
    }

    static void BuildBird(Vector3 arena, GameObject portal)
    {
        Vector3 pos = arena + new Vector3(10, 9, 0);
        var root = Kit.Group("Boss_Oluja", pos);
        var model = Kit.Group("Model", pos, root);
        Kit.V(model, PrimitiveType.Sphere, "Body", new Vector3(0, 1.2f, 0), new Vector3(2.2f, 1.8f, 3.2f), "Storm");
        Kit.V(model, PrimitiveType.Sphere, "Belly", new Vector3(0, 1.0f, 0.3f), new Vector3(1.8f, 1.4f, 2.4f), "StormLight");
        Kit.V(model, PrimitiveType.Sphere, "Head", new Vector3(0, 2.0f, 1.5f), Vector3.one * 1.3f, "Storm");
        Kit.V(model, PrimitiveType.Cube, "Beak", new Vector3(0, 1.9f, 2.3f), new Vector3(0.35f, 0.3f, 0.8f), "Orange");
        Kit.V(model, PrimitiveType.Sphere, "Eye", new Vector3(-0.38f, 2.2f, 1.98f), Vector3.one * 0.3f, "Yellow");
        Kit.V(model, PrimitiveType.Sphere, "Eye", new Vector3(0.38f, 2.2f, 1.98f), Vector3.one * 0.3f, "Yellow");
        Kit.V(model, PrimitiveType.Cube, "Crest", new Vector3(0, 2.8f, 1.4f), new Vector3(0.15f, 0.7f, 0.5f), "Yellow", new Vector3(-20, 0, 0));
        Kit.V(model, PrimitiveType.Cube, "Tail", new Vector3(0, 1.4f, -1.9f), new Vector3(1.2f, 0.12f, 1.4f), "StormDark", new Vector3(-10, 0, 0));
        var wingL = Kit.Group("Wing_L", pos, model); wingL.localPosition = new Vector3(-1f, 1.6f, 0);
        var wingR = Kit.Group("Wing_R", pos, model); wingR.localPosition = new Vector3(1f, 1.6f, 0);
        Kit.V(wingL, PrimitiveType.Cube, "Feathers", new Vector3(-2.2f, 0, 0), new Vector3(4f, 0.15f, 1.8f), "StormDark");
        Kit.V(wingR, PrimitiveType.Cube, "Feathers", new Vector3(2.2f, 0, 0), new Vector3(4f, 0.15f, 1.8f), "StormDark");

        var back = Kit.Group("BackPoint", pos, model);
        back.localPosition = new Vector3(0, 2.6f, -0.3f);
        var vis = Kit.Group("Visual", pos, back);
        vis.localPosition = Vector3.zero;
        Kit.V(vis, PrimitiveType.Sphere, "Orb", Vector3.zero, Vector3.one * 0.8f, "CrystalGlow");
        var gp = back.gameObject.AddComponent<GrapplePoint>();
        gp.visual = vis;
        gp.bob = false;
        gp.targetable = false;

        var boss = root.gameObject.AddComponent<StormBirdBoss>();
        boss.bossName = "Tempestra, Queen of Storms";
        boss.maxHealth = 4;
        boss.arenaCenter = arena;
        boss.arenaRadius = 16f;
        boss.portal = portal;
        boss.model = model;
        boss.wingL = wingL;
        boss.wingR = wingR;
        boss.backPoint = gp;
        boss.hint = "Run from the red circles! When Tempestra lands dizzy, jump on her or grab her with your tongue.";
    }

    // =====================================================================
    // IZBORNIK I KRAJ
    // =====================================================================
    static void BuildMenu()
    {
        var scene = Kit.NewScene(new Theme { id = "Menu" });
        Kit.WaterPlane(-0.3f);
        Kit.RoundGround(new Vector3(0, 0.3f, 0), 11f, 3f, "Sand", "SandDark");
        Kit.RoundGround(new Vector3(0, 0.6f, 0), 8f, 0.4f, "Top", "Top");
        Kit.Palm(new Vector3(-4, 0.6f, 3), 1);
        Kit.Palm(new Vector3(5, 0.6f, -2), 2);
        Kit.Palm(new Vector3(-2, 0.6f, -6), 3);
        Kit.Bush(new Vector3(3, 0.6f, 4));
        Kit.Boulder(new Vector3(-6, 0.8f, -1), 1.6f);
        var frog = new GameObject("Frog").transform;
        frog.position = new Vector3(0, 0.6f, 0);
        frog.rotation = Quaternion.Euler(0, 160, 0);
        Kit.FrogModel(frog).gameObject.AddComponent<Bobber>().amplitude = 0.12f;
        Kit.Grapple(new Vector3(3, 4, 2));
        var cam = Kit.MakeCamera(new Vector3(0, 8, -18));
        var orbit = cam.gameObject.AddComponent<MenuCameraOrbit>();
        orbit.center = new Vector3(0, 0.6f, 0);
        orbit.radius = 14f;
        orbit.height = 5f;
        new GameObject("Menu").AddComponent<MainMenuUI>();
        Kit.Music("Menu");
        Save(scene, "Ribblin_Menu");
    }

    static void BuildEnd()
    {
        var scene = Kit.NewScene(new Theme { id = "End", sky = new Color(1f, 0.75f, 0.55f), fog = new Color(1f, 0.82f, 0.65f), sun = new Color(1f, 0.85f, 0.65f), sunEuler = new Vector3(20, -40, 0) });
        Kit.WaterPlane(-0.3f);
        Kit.RoundGround(new Vector3(0, 0.3f, 0), 11f, 3f, "Sand", "SandDark");
        Kit.RoundGround(new Vector3(0, 0.6f, 0), 8f, 0.4f, "Top", "Top");
        Kit.Palm(new Vector3(-4, 0.6f, 3), 5);
        Kit.Palm(new Vector3(5, 0.6f, -2), 6);
        var frog = new GameObject("Frog").transform;
        frog.position = new Vector3(0, 0.6f, 0);
        frog.rotation = Quaternion.Euler(0, 160, 0);
        var m = Kit.FrogModel(frog);
        m.gameObject.AddComponent<Bobber>().amplitude = 0.3f;
        Kit.V(m, PrimitiveType.Cylinder, "Crown", new Vector3(0, 1.35f, 0.15f), new Vector3(0.45f, 0.08f, 0.45f), "Gold");
        for (int i = 0; i < 5; i++)
        {
            float a = i / 5f * Mathf.PI * 2f;
            Kit.V(m, PrimitiveType.Cube, "Tip", new Vector3(Mathf.Cos(a) * 0.18f, 1.5f, Mathf.Sin(a) * 0.18f + 0.15f), new Vector3(0.08f, 0.18f, 0.08f), "Gold");
        }
        for (int i = 0; i < 12; i++)
        {
            float a = i / 12f * Mathf.PI * 2f;
            Kit.Fruit(new Vector3(Mathf.Cos(a) * 4f, 1.5f, Mathf.Sin(a) * 4f));
        }
        var cam = Kit.MakeCamera(new Vector3(0, 8, -18));
        var orbit = cam.gameObject.AddComponent<MenuCameraOrbit>();
        orbit.center = new Vector3(0, 0.6f, 0);
        orbit.radius = 12f;
        orbit.height = 4f;
        new GameObject("End").AddComponent<EndScreenUI>();
        Kit.Music("End");
        Save(scene, "Ribblin_End");
    }
}
#endif

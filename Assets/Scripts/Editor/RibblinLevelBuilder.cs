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
        Kit.SetDifficulty(3.0f, 9f, false);     // Level 1
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

        // nove vrste neprijatelja: pčele i biljke koje pljuju
        Kit.Flyer(new Vector3(-16, 3f, -6), "bee");
        Kit.Flyer(new Vector3(12, 3.5f, 8), "bee");
        Kit.Flyer(new Vector3(-12, 10.5f, 10), "bee");
        Kit.Flyer(new Vector3(0, 3f, 45), "bee");
        Kit.Spitter(new Vector3(-18, 0.6f, 6), "plant");
        Kit.Spitter(new Vector3(20, 0.6f, -4), "plant");
        Kit.Spitter(new Vector3(-9, 7.75f, 12), "plant");
        Kit.Slime(new Vector3(-8, 0.6f, 20), 4f, "SlimeOrange", 1f, true, true);

        // zamke: šiljci koji iskaču i njihalo
        Kit.PopSpikes(new Vector3(0, 0.6f, -4), new Vector3(4, 0, 2), 3f, 0f);
        Kit.PopSpikes(new Vector3(-4, 0.6f, 1), new Vector3(3, 0, 2), 3f, 1.5f);
        Kit.Pendulum(new Vector3(26, 0.3f, 0), 5f, 60f, 2.6f, 0f, "TrunkDark", 90f);

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
        Kit.FallingPlat(new Vector3(3, 1.4f, -38.5f), new Vector3(2.6f, 1f, 2.6f), "RockLight", "RockLight");
        Kit.FallingPlat(new Vector3(-0.5f, 2.0f, -43), new Vector3(2.6f, 1f, 2.6f), "Rock", "Rock");
        Kit.FallingPlat(new Vector3(2.5f, 2.6f, -47.5f), new Vector3(2.4f, 1f, 2.4f), "RockLight", "RockLight");
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
                                   new Vector3(-14, 0.3f, 27.5f), new Vector3(27.5f, 0.3f, -22), new Vector3(-5.8f, 0.4f, 46) })
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
        int required = 14;   // Level 1: 14 od 21

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
        boss.roarSound = "crab_roar"; boss.attackSound = "crab_attack"; boss.hurtSound = "crab_hurt";
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
        Kit.SetDifficulty(3.4f, 10f, false);    // Level 2
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
        Kit.Flyer(new Vector3(0, 3.5f, 52), "mosquito");
        Kit.Spitter(new Vector3(-5, 1, 71), "swamp");
        Kit.Flyer(new Vector3(-3, 4f, 100), "mosquito");
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
        Kit.Slime(new Vector3(4, 1, 99), 3f, "SlimePurple");
        Kit.Slime(new Vector3(-2, 1, 36), 3f, "SlimePurple");
        Kit.Slime(new Vector3(0, 1, 2), 4f, "SlimePurple");
        Kit.MushroomDeco(new Vector3(6, 1, 96), 1.2f);
        Kit.MushroomDeco(new Vector3(-6, 1, 98), 0.9f);

        // S7 – balvani koji se ruše
        Kit.FallingPlat(new Vector3(2, 0.6f, 112), new Vector3(2.2f, 1f, 2.8f), "Trunk", "TrunkDark");
        Kit.FallingPlat(new Vector3(-1.5f, 0.6f, 116.5f), new Vector3(2.2f, 1f, 2.8f), "Trunk", "TrunkDark");
        Kit.FallingPlat(new Vector3(2, 0.6f, 121), new Vector3(2.2f, 1f, 2.8f), "Trunk", "TrunkDark");
        Kit.FallingPlat(new Vector3(-1, 0.6f, 125.5f), new Vector3(2.2f, 1f, 2.8f), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(-1.5f, 1.8f, 116.5f));
        Kit.Fruit(new Vector3(-1, 1.8f, 125.5f));

        // S8 – otok s biljkama i komarcima
        Kit.RoundGround(new Vector3(0, 1, 136), 8f, 4f);
        Kit.Checkpoint(new Vector3(-3, 1, 130.5f));
        Kit.Spitter(new Vector3(-5, 1, 139), "swamp");
        Kit.Spitter(new Vector3(5, 1, 141), "swamp");
        Kit.Flyer(new Vector3(-2, 3.5f, 137), "mosquito");
        Kit.Flyer(new Vector3(4, 3.5f, 133), "mosquito");
        Kit.Pendulum(new Vector3(0, 1, 136), 5f, 60f, 2.4f, 0f, "TrunkDark");
        Kit.PopSpikes(new Vector3(0, 1, 141), new Vector3(4, 0, 2), 2.8f, 0.7f);
        Kit.Fruit(new Vector3(-6, 2, 134));
        Kit.Fruit(new Vector3(6, 2, 136));
        Kit.Fruit(new Vector3(0, 2, 143));
        Kit.DeadTree(new Vector3(6, 1, 131), 6f);

        // S9 – lopoči i jezik
        Kit.Lily(new Vector3(3, 0.25f, 147.5f), 1.7f);
        Kit.LilyMover(new Vector3(-4, 0.25f, 153), 1.8f, new Vector3(8, 0, 0), 4f, 0.25f);
        Kit.Lily(new Vector3(-2, 0.25f, 158.5f), 1.7f);
        Kit.Grapple(new Vector3(1, 6f, 163));
        Kit.Grapple(new Vector3(2, 6.5f, 168));
        Kit.Fruit(new Vector3(3, 1.5f, 147.5f));
        Kit.Fruit(new Vector3(-2, 1.5f, 158.5f));

        // S10 – veliki otok s kućicom na stablu
        Kit.RoundGround(new Vector3(5, 1, 180), 10f, 4f);
        Kit.Checkpoint(new Vector3(2, 1, 172.5f));
        Kit.Slime(new Vector3(0, 1, 180), 4f, "SlimePurple", 1f, true, true);
        Kit.Slime(new Vector3(9, 1, 183), 3.5f, "SlimePurple", 1f, true, true);
        Kit.Slime(new Vector3(4, 1, 176), 4f, "SlimePurple");
        Kit.Slime(new Vector3(10, 1, 177), 3f, "SlimePurple");
        Kit.Flyer(new Vector3(6, 4f, 184), "mosquito");
        Kit.Flyer(new Vector3(0, 4f, 186), "mosquito");
        Kit.PopSpikes(new Vector3(5, 1, 181), new Vector3(6, 0, 1.5f), 2.6f, 0f);
        Kit.PopSpikes(new Vector3(5, 1, 184.5f), new Vector3(6, 0, 1.5f), 2.6f, 1.3f);
        Kit.BouncePad(new Vector3(11, 1, 179), 9f);
        Kit.P(PrimitiveType.Cylinder, "TreeTrunk", null, new Vector3(13, 4.5f, 185), new Vector3(1.4f, 3.5f, 1.4f), "TrunkDark");
        Kit.Ground(new Vector3(12, 8f, 185), new Vector3(5, 0.6f, 4.5f), "Trunk", "TrunkDark");
        Kit.Fruit(new Vector3(11, 9f, 184));
        Kit.Fruit(new Vector3(13.5f, 9f, 186));
        Kit.Fruit(new Vector3(-3, 2, 183));
        Kit.Fruit(new Vector3(8, 2, 188));
        Kit.Fruit(new Vector3(1, 2, 175));
        Kit.Fruit(new Vector3(10, 2, 173));
        Kit.DeadTree(new Vector3(-2, 1, 186), 8f);
        Kit.MushroomDeco(new Vector3(9, 1, 172), 1f);
        // bonus otočić na zapadu
        Kit.Grapple(new Vector3(-9, 6f, 178));
        Kit.RoundGround(new Vector3(-16, 1, 178), 4f, 4f);
        Kit.Fruit(new Vector3(-16, 2, 177));
        Kit.Fruit(new Vector3(-17, 2, 179.5f));
        Kit.Grapple(new Vector3(-10, 6.5f, 180));

        // S11 – dugi balvan s njihalima
        Kit.Log(new Vector3(5, 0.8f, 200), new Vector3(2.4f, 1.2f, 20));
        Kit.Pendulum(new Vector3(5, 0.8f, 195), 5f, 60f, 2.2f, 0f, "TrunkDark");
        Kit.Pendulum(new Vector3(5, 0.8f, 200), 5f, 60f, 2.2f, 0.33f, "TrunkDark");
        Kit.Pendulum(new Vector3(5, 0.8f, 205), 5f, 60f, 2.2f, 0.66f, "TrunkDark");
        Kit.Fruit(new Vector3(5, 2.2f, 197.5f));
        Kit.Fruit(new Vector3(5, 2.2f, 202.5f));

        // S12 – ispred bossa
        Kit.RoundGround(new Vector3(5, 1, 220), 7f, 4f);
        Kit.Checkpoint(new Vector3(2, 1, 215), true);
        Kit.Spitter(new Vector3(9, 1, 222), "swamp");
        Kit.Fruit(new Vector3(1, 2, 223));
        Kit.DeadTree(new Vector3(9, 1, 217), 6f);

        int required = Mathf.RoundToInt(Kit.FruitCount * 0.8f);   // Level 2: 80 % voća
        Kit.Grapple(new Vector3(5, 6.5f, 232), required);
        Kit.Grapple(new Vector3(5, 7f, 238), required);

        // arena
        Vector3 arena = new Vector3(5, 1, 255);
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
        foreach (var sp in new[] { new Vector3(-5, 1, -4), new Vector3(5, 1, 33), new Vector3(-4, 1, 64), new Vector3(5, 1, 96), new Vector3(-5, 1, 140), new Vector3(12, 1, 176), new Vector3(-1, 1, 222), new Vector3(15, 1, 250), new Vector3(-5, 1, 262) })
            Kit.Snake(sp, 2.5f);

        // krijesnice i trska
        for (int i = 0; i < 70; i++)
            Kit.Firefly(new Vector3(Random.Range(-15f, 20f), Random.Range(1.5f, 5f), Random.Range(-5f, 270f)));
        for (int i = 0; i < 26; i++)
            Kit.Reeds(new Vector3(Random.Range(-25f, 25f), 0, Random.Range(0f, 270f)) + Vector3.right * (Random.value > 0.5f ? 16f : -14f));

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
        boss.roarSound = "toad_roar"; boss.attackSound = "toad_attack"; boss.hurtSound = "toad_hurt";
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
            sky = new Color(0.2f, 0.16f, 0.3f), fog = new Color(0.28f, 0.22f, 0.36f), fogStart = 60f, fogEnd = 280f,
            ambSky = new Color(0.55f, 0.55f, 0.75f), ambEq = new Color(0.45f, 0.42f, 0.55f), ambGround = new Color(0.55f, 0.32f, 0.2f),
            sun = new Color(1f, 0.9f, 0.8f), sunIntensity = 1.35f, sunEuler = new Vector3(45, 25, 0),
        };
        // bazalt: hladne sive boje, da se lava, voće i neprijatelji jasno vide
        t.colors["Top"] = new Color(0.46f, 0.47f, 0.54f);
        t.colors["Side"] = new Color(0.24f, 0.25f, 0.3f);
        t.colors["SideDark"] = new Color(0.16f, 0.17f, 0.21f);
        t.colors["Rock"] = new Color(0.3f, 0.31f, 0.37f);
        t.colors["RockLight"] = new Color(0.4f, 0.41f, 0.48f);
        var scene = Kit.NewScene(t);
        Kit.SetDifficulty(3.9f, 12f, true);     // Level 3: skaču na žabicu
        Kit.WaterPlane(0f, "LavaPlane");

        // S0 – start
        Kit.Ground(new Vector3(0, 3, 0), new Vector3(14, 6, 14));
        Kit.Fruit(new Vector3(4, 4, 3));
        Kit.CrystalDeco(new Vector3(-5, 3, -5), 1.5f, "CrystalGlow");
        Kit.CrystalDeco(new Vector3(5, 3, -4), 1.2f, "CrystalGlow");

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
        Kit.Slime(new Vector3(-3, 6.5f, 38), 2.5f, "SlimeOrange");
        Kit.Slime(new Vector3(0, 6.5f, 77), 2f, "SlimeOrange");
        Kit.Slime(new Vector3(-3, 15, 115), 2.5f, "SlimeOrange");
        Kit.Slime(new Vector3(0, 15, 100), 1f, "SlimeOrange");
        Kit.Fruit(new Vector3(4.5f, 7.5f, 43.5f));

        // S3 – pokretne platforme iznad lave
        Kit.Mover(new Vector3(0, 6.5f, 50), new Vector3(3, 1, 3), new Vector3(0, 0, 6), 5f, 0f);
        Kit.Mover(new Vector3(-5, 6.5f, 62), new Vector3(3, 1, 3), new Vector3(10, 0, 0), 5f, 0.25f);
        Kit.Fruit(new Vector3(0, 8.2f, 59));

        // S4 – vrtuljak
        Kit.Ground(new Vector3(0, 6.5f, 72), new Vector3(14, 7.5f, 12));
        Kit.SpinnerBar(new Vector3(0, 6.5f, 72), 11f, 85f);
        Kit.Flyer(new Vector3(-3, 9.5f, 40), "bat");
        Kit.Spitter(new Vector3(5.5f, 6.5f, 77), "lava");
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
        Kit.CrystalDeco(new Vector3(-5.5f, 15, 84), 1.4f, "CrystalGlow");
        Kit.CrystalDeco(new Vector3(5.5f, 15, 83), 1.1f, "CrystalGlow");

        // most s gejzirima
        Kit.Ground(new Vector3(0, 15, 100), new Vector3(3, 1.5f, 16));
        Kit.Geyser(new Vector3(0, 15, 96), 0f, 5f);
        Kit.Geyser(new Vector3(0, 15, 100), 1.2f, 5f);
        Kit.Geyser(new Vector3(0, 15, 104), 2.4f, 5f);
        Kit.Fruit(new Vector3(0, 16, 98));

        // S7 – ispred bossa + skriveni otočić
        Kit.Ground(new Vector3(0, 15, 114), new Vector3(10, 16, 10));
        Kit.Checkpoint(new Vector3(-3, 15, 111));
        Kit.Grapple(new Vector3(10, 19f, 114));
        Kit.RoundGround(new Vector3(17, 14f, 114), 3f, 15f);
        Kit.Fruit(new Vector3(16, 15f, 113));
        Kit.Fruit(new Vector3(18, 15f, 115));
        Kit.Fruit(new Vector3(-4, 16, 117));

        Kit.Fruit(new Vector3(0, 8f, 50));
        Kit.Fruit(new Vector3(-5, 6f, 2));
        Kit.Fruit(new Vector3(0, 16, 112));
        // S8 – rijeka lave s kamenjem koje se ruši
        Kit.FallingPlat(new Vector3(2, 15, 123), new Vector3(2.4f, 1f, 2.4f), "Rock", "Basalt");
        Kit.FallingPlat(new Vector3(-2, 15.5f, 127.5f), new Vector3(2.4f, 1f, 2.4f), "RockLight", "Basalt");
        Kit.FallingPlat(new Vector3(2, 16f, 132), new Vector3(2.4f, 1f, 2.4f), "Rock", "Basalt");
        Kit.FallingPlat(new Vector3(-1.5f, 16f, 136.5f), new Vector3(2.4f, 1f, 2.4f), "RockLight", "Basalt");
        Kit.FallingPlat(new Vector3(1, 16f, 141f), new Vector3(2.4f, 1f, 2.4f), "Rock", "Basalt");
        Kit.Fruit(new Vector3(-2, 16.8f, 127.5f));
        Kit.Fruit(new Vector3(-1.5f, 17.3f, 136.5f));

        // S9 – utvrda s topovima i šišmišima
        Kit.Ground(new Vector3(0, 16, 152), new Vector3(14, 17, 12));
        Kit.Checkpoint(new Vector3(-4, 16, 147.5f));
        Kit.Spitter(new Vector3(-5, 16, 156), "lava");
        Kit.Spitter(new Vector3(5, 16, 155), "lava");
        Kit.Flyer(new Vector3(0, 19f, 151), "bat");
        Kit.Flyer(new Vector3(4, 19f, 148), "bat");
        Kit.PopSpikes(new Vector3(0, 16, 150.5f), new Vector3(6, 0, 1.5f), 2.5f, 0f);
        Kit.PopSpikes(new Vector3(0, 16, 154f), new Vector3(6, 0, 1.5f), 2.5f, 1.25f);
        Kit.Slime(new Vector3(-3, 16, 152), 2.5f, "SlimeOrange", 1f, true, true);
        Kit.Fruit(new Vector3(-6, 17, 150));
        Kit.Fruit(new Vector3(6, 17, 150));
        Kit.Fruit(new Vector3(0, 17, 157));

        // S10 – toranj: penji se po stupovima ili jezikom
        Kit.Ground(new Vector3(0, 30, 170), new Vector3(10, 31, 10));
        float[] th = { 18.2f, 20.4f, 22.6f, 24.8f, 27f, 28.8f };
        float[] tz = { 160f, 163.5f, 167f, 170.5f, 174f, 177.2f };
        for (int i = 0; i < th.Length; i++) Kit.RoundGround(new Vector3(-7, th[i], tz[i]), 1.3f, th[i] + 1f);
        Kit.Grapple(new Vector3(0, 25f, 160.5f));
        Kit.Grapple(new Vector3(0, 33.5f, 163.5f));
        Kit.Fruit(new Vector3(-7, 23.7f, 167f));
        Kit.Checkpoint(new Vector3(-3, 30, 167));
        Kit.Flyer(new Vector3(2, 33f, 171), "bat");
        Kit.Slime(new Vector3(1, 30, 172), 2.5f, "SlimeOrange", 1f, true, true);
        Kit.Fruit(new Vector3(3.5f, 31, 173.5f));
        Kit.Fruit(new Vector3(-3.5f, 31, 173.5f));

        // S11 – most s njihalima i gejzirima
        Kit.Ground(new Vector3(0, 30, 189), new Vector3(3, 1.5f, 28));
        Kit.Pendulum(new Vector3(0, 30, 181), 5f, 60f, 2.2f, 0f, "Basalt");
        Kit.Geyser(new Vector3(0, 30, 186), 0.5f, 5f);
        Kit.Pendulum(new Vector3(0, 30, 191), 5f, 60f, 2.2f, 0.5f, "Basalt");
        Kit.Geyser(new Vector3(0, 30, 196), 1.7f, 5f);
        Kit.Fruit(new Vector3(0, 31, 183.5f));
        Kit.Fruit(new Vector3(0, 31, 193.5f));

        // S12 – ispred bossa
        Kit.Ground(new Vector3(0, 30, 209), new Vector3(12, 31, 12));
        Kit.Checkpoint(new Vector3(-4, 30, 205), true);
        Kit.Spitter(new Vector3(4, 30, 212), "lava");
        Kit.Flyer(new Vector3(-2, 33, 211), "bat");
        Kit.Fruit(new Vector3(-4.5f, 31, 213));

        int required = Mathf.RoundToInt(Kit.FruitCount * 0.85f);   // Level 3: 85 % voća
        Kit.Grapple(new Vector3(0, 35f, 221), required);
        Kit.Grapple(new Vector3(0, 35.5f, 227), required);

        // arena
        Vector3 arena = new Vector3(0, 30, 245);
        Kit.RoundGround(arena, 16f, 32f);
        Kit.Checkpoint(arena + new Vector3(-3, 0, -13));
        for (int i = 0; i < 10; i++)
        {
            float a = i / 10f * Mathf.PI * 2f;
            Kit.CrystalDeco(arena + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 15f, Random.Range(1f, 1.8f), "CrystalGlow");
        }
        var portal = Kit.Portal(arena + new Vector3(0, 0, 12), "Level4_Nebo", arena);
        BuildGolem(arena, portal);

        // vulkan u pozadini, bazaltni šiljci, iskre iz lave
        Kit.Volcano(new Vector3(-70, 0, 120), 38f, 55f);
        Kit.Volcano(new Vector3(80, 0, 40), 22f, 30f);
        for (int i = 0; i < 40; i++)
        {
            float x = Random.Range(14f, 45f) * (Random.value > 0.5f ? 1f : -1f);
            Vector3 sp = new Vector3(x, -1f, Random.Range(-15f, 270f));
            if (Vector3.Distance(new Vector3(sp.x, 0, sp.z), new Vector3(17, 0, 114)) < 8f) continue;
            if (Vector3.Distance(new Vector3(sp.x, 0, sp.z), new Vector3(arena.x, 0, arena.z)) < 22f) continue;
            Kit.BasaltSpire(sp, Random.Range(6f, 18f), Random.Range(2f, 4.5f));
        }
        Kit.Embers(new Vector3(0, 0, 40), 25f, 30);
        Kit.Embers(new Vector3(0, 0, 110), 25f, 30);
        Kit.Embers(new Vector3(0, 0, 190), 25f, 30);
        Kit.Volcano(new Vector3(70, 0, 220), 30f, 45f);

        // zmajevi: veliki lete iznad levela, mali šetaju po platformama
        Kit.FlyingDragon(new Vector3(0, 0, 60), 45f, 30f, 0.15f, "DragonGreen", 2.2f);
        Kit.FlyingDragon(new Vector3(0, 0, 110), 38f, 38f, -0.12f, "DragonPurple", 2.6f);
        Kit.FlyingDragon(new Vector3(-10, 0, 150), 30f, 42f, 0.18f, "DragonTeal", 1.8f);
        Kit.FlyingDragon(new Vector3(0, 0, 220), 40f, 50f, -0.14f, "DragonPurple", 2.4f);
        Kit.FlyingDragon(new Vector3(20, 0, 20), 50f, 45f, -0.1f, "DragonGreen", 3f);
        Kit.WalkingDragon(new Vector3(-4.5f, 3, -3), 2f, "DragonGreen");
        Kit.WalkingDragon(new Vector3(4, 6.5f, 36.5f), 1.5f, "DragonTeal");
        Kit.WalkingDragon(new Vector3(-5, 6.5f, 70), 1.5f, "DragonPurple");
        Kit.WalkingDragon(new Vector3(-4, 15, 89), 2f, "DragonGreen");
        Kit.WalkingDragon(new Vector3(3, 15, 112), 1.5f, "DragonTeal");
        Kit.WalkingDragon(new Vector3(4, 16, 158), 1.5f, "DragonGreen");
        Kit.WalkingDragon(new Vector3(3, 30, 207), 1.5f, "DragonTeal");
        Kit.WalkingDragon(arena + new Vector3(10, 0, -6), 2f, "DragonPurple");

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
        foreach (var lp in new[] { new Vector3(-2.3f, 5.5f, 0), new Vector3(2.3f, 5.5f, 0), new Vector3(0, 4.3f, -1.5f),
                                   new Vector3(-1.2f, 2.2f, 1.0f), new Vector3(1.2f, 2.2f, 1.0f) })
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
        boss.roarSound = "golem_roar"; boss.attackSound = "golem_attack"; boss.hurtSound = "golem_hurt";
        boss.bossName = "Magmor the Molten";
        boss.maxHealth = 3;
        boss.arenaCenter = pos;
        boss.arenaRadius = 16f;
        boss.portal = portal;
        boss.model = model;
        boss.throwHand = hand.transform;
        boss.crystals = crystals.ToArray();
        boss.hint = "Stay out of the red circles! When Magmor overheats, quickly grab one of his 5 glowing crystals with your tongue.";
    }

    // =====================================================================
    // LEVEL 4 – NEBO   (boss: Tempestra)
    // =====================================================================
    static void BuildLevel4()
    {
        // Nebesko kraljevstvo u zlatnom zalazu sunca: mramor, zlato i baloni – sasvim drukčije od otoka
        var t = new Theme
        {
            id = "L4",
            sky = new Color(0.98f, 0.8f, 0.52f), fog = new Color(1f, 0.9f, 0.72f), fogStart = 80f, fogEnd = 360f,
            ambSky = new Color(1f, 0.92f, 0.8f), ambEq = new Color(0.92f, 0.86f, 0.75f), ambGround = new Color(0.6f, 0.55f, 0.5f),
            sun = new Color(1f, 0.85f, 0.65f), sunIntensity = 1.35f, sunEuler = new Vector3(28, -50, 0),
        };
        // zlatni zalazak sunca: krem mramor, zlato i bijeli oblaci (bez roza i dugih boja)
        t.colors["Top"] = new Color(0.93f, 0.88f, 0.76f);     // krem kamen
        t.colors["Side"] = new Color(0.82f, 0.78f, 0.72f);    // mramor
        t.colors["SideDark"] = new Color(0.62f, 0.58f, 0.54f);
        t.colors["Rock"] = new Color(0.95f, 0.93f, 0.9f);
        t.colors["Cloud"] = new Color(1f, 1f, 1f);
        t.colors["Balloon1"] = new Color(0.85f, 0.25f, 0.2f);
        var scene = Kit.NewScene(t);
        Kit.SetDifficulty(4.5f, 14f, true);     // Level 4: najbrži i najagresivniji

        // oblaci i ptice samo sa strane
        for (int i = 0; i < 70; i++)
        {
            float x = Random.Range(35f, 95f) * (Random.value > 0.5f ? 1f : -1f);
            Kit.Cloud(new Vector3(x, Random.Range(-6f, 30f), Random.Range(-40f, 300f)), Random.Range(2.5f, 5f));
        }
        for (int i = 0; i < 10; i++)
            Kit.Bird(new Vector3(0, 0, 130), Random.Range(45f, 70f), Random.Range(20f, 40f), Random.Range(0.08f, 0.18f) * (i % 2 == 0 ? 1f : -1f));

        // S0 – početni otok s hramom
        Kit.FloatIsland(new Vector3(0, 10, 0), 8f);
        Kit.Temple(new Vector3(-4, 10, 2), 1.6f);
        Kit.Windmill(new Vector3(5, 10, -3));
        Kit.Fruit(new Vector3(3, 11, 4));
        Kit.Fruit(new Vector3(-4, 11, -4));
        Kit.Slime(new Vector3(2, 10, 1), 3f, "SlimeBlue");

        // S1 – mramorni most s iskačućim šiljcima
        Kit.Bridge(new Vector3(0, 10, 8), 22f, 3f);
        Kit.PopSpikes(new Vector3(0, 10, 14), new Vector3(3, 0, 1.5f), 2.4f, 0f);
        Kit.PopSpikes(new Vector3(0, 10, 22), new Vector3(3, 0, 1.5f), 2.4f, 1.2f);
        Kit.Fruit(new Vector3(0, 11, 18));
        Kit.Fruit(new Vector3(0, 11, 26));

        // S2 – otok s duhovima i oblakom koji pljuje
        Kit.FloatIsland(new Vector3(0, 10, 37), 7f);
        Kit.Flyer(new Vector3(-3, 13, 37), "sprite");
        Kit.Flyer(new Vector3(3, 13, 40), "sprite");
        Kit.Spitter(new Vector3(4, 10, 41), "cloud");
        Kit.Slime(new Vector3(-2, 10, 35), 3f, "SlimeBlue");
        Kit.MarbleColumn(new Vector3(-5, 10, 41), 5f);
        Kit.Fruit(new Vector3(-4, 11, 34));
        Kit.Fruit(new Vector3(4, 11, 35));

        // S3 – baloni
        Kit.Balloon(new Vector3(0, 10, 47.5f), new Vector3(0, 6, 0), 5f, 0.75f, "Balloon1");
        Kit.Balloon(new Vector3(-3, 16, 54), new Vector3(6, 0, 0), 4.5f, 0f, "Balloon2");
        Kit.Balloon(new Vector3(6, 16, 61), new Vector3(0, 3, 0), 4f, 0.25f, "Balloon3");
        Kit.Fruit(new Vector3(0, 18, 54));
        Kit.Fruit(new Vector3(6, 21, 61));

        // S4 – otok s vjetrenjačom
        Kit.FloatIsland(new Vector3(6, 16, 72), 6f);
        Kit.Checkpoint(new Vector3(3, 16, 68));
        Kit.Windmill(new Vector3(9, 16, 75));
        Kit.Slime(new Vector3(6, 16, 72), 3f, "SlimeBlue", 1f, true, true);
        Kit.Spitter(new Vector3(2, 16, 75), "cloud");
        Kit.Fruit(new Vector3(9, 17, 70));
        Kit.Fruit(new Vector3(3, 17, 76));

        // S5 – jezikom preko velike praznine (duhovi patroliraju)
        Kit.Grapple(new Vector3(6, 22, 84));
        Kit.Grapple(new Vector3(2, 24, 93));
        Kit.Grapple(new Vector3(-2, 23, 102));
        Kit.Flyer(new Vector3(4, 21, 89), "sprite");
        Kit.Flyer(new Vector3(0, 22, 98), "sprite");
        Kit.Fruit(new Vector3(4, 23.5f, 88.5f));
        Kit.Fruit(new Vector3(0, 23.5f, 97.5f));

        // S6 – mramorni trg sa zamkama
        Kit.FloatIsland(new Vector3(-4, 18, 115), 8f);
        Kit.Checkpoint(new Vector3(-7, 18, 109));
        Kit.Pendulum(new Vector3(-4, 18, 113), 5f, 60f, 2.2f, 0f, "Marble");
        Kit.Pendulum(new Vector3(-4, 18, 118), 5f, 60f, 2.2f, 0.5f, "Marble");
        Kit.PopSpikes(new Vector3(-8, 18, 115.5f), new Vector3(2, 0, 4), 2.6f, 0.6f);
        Kit.Slime(new Vector3(-1, 18, 116), 3f, "SlimeBlue");
        Kit.Slime(new Vector3(-7, 18, 119), 3f, "SlimeBlue");
        Kit.Temple(new Vector3(0, 18, 120), 1.4f);
        Kit.Fruit(new Vector3(-9, 19, 112));
        Kit.Fruit(new Vector3(1, 19, 111));
        Kit.Fruit(new Vector3(-4, 19, 121));

        // S7 – stepenice od oblaka koje se ruše
        Kit.FallingPlat(new Vector3(-4, 19, 126.5f), new Vector3(2.6f, 0.8f, 2.6f), "Cloud", "Cloud");
        Kit.FallingPlat(new Vector3(0, 20, 131), new Vector3(2.6f, 0.8f, 2.6f), "Cloud", "Cloud");
        Kit.FallingPlat(new Vector3(-3, 21, 135.5f), new Vector3(2.6f, 0.8f, 2.6f), "Cloud", "Cloud");
        Kit.FallingPlat(new Vector3(1, 22, 140), new Vector3(2.6f, 0.8f, 2.6f), "Cloud", "Cloud");
        Kit.FallingPlat(new Vector3(-2, 23, 144.5f), new Vector3(2.6f, 0.8f, 2.6f), "Cloud", "Cloud");
        Kit.Fruit(new Vector3(0, 21.5f, 131));
        Kit.Fruit(new Vector3(1, 23.5f, 140));

        // S8 – zlatni otok s trampolinom do hrama u zraku
        Kit.FloatIsland(new Vector3(0, 24, 156), 7f);
        Kit.Checkpoint(new Vector3(-3, 24, 151));
        Kit.Spitter(new Vector3(-4, 24, 159), "cloud");
        Kit.Spitter(new Vector3(4, 24, 160), "cloud");
        Kit.Flyer(new Vector3(0, 27, 157), "sprite");
        Kit.Flyer(new Vector3(-3, 27, 154), "sprite");
        Kit.Slime(new Vector3(2, 24, 154), 3f, "SlimeBlue", 1f, true, true);
        Kit.BouncePad(new Vector3(4, 24, 153), 12f);
        Kit.FloatIsland(new Vector3(9, 35, 160), 3.5f);
        Kit.Temple(new Vector3(9, 35, 160), 1.2f);
        Kit.Fruit(new Vector3(8, 36, 159));
        Kit.Fruit(new Vector3(10, 36, 161));
        Kit.Fruit(new Vector3(-5, 25, 156));

        // S9 – dugi mramorni most s vrtuljkom i njihalom
        Kit.Bridge(new Vector3(0, 24, 163), 30f, 3f);
        Kit.SpinnerBar(new Vector3(0, 24, 171), 6f, 110f);
        Kit.Pendulum(new Vector3(0, 24, 179), 5f, 60f, 2f, 0.2f, "Marble");
        Kit.PopSpikes(new Vector3(0, 24, 186), new Vector3(3, 0, 1.5f), 2.2f, 0.4f);
        Kit.Fruit(new Vector3(0, 25, 175));
        Kit.Fruit(new Vector3(0, 25, 183));

        // S10 – ispred bossa
        Kit.FloatIsland(new Vector3(0, 24, 199), 7f);
        Kit.Checkpoint(new Vector3(-3, 24, 194), true);
        Kit.Temple(new Vector3(4, 24, 201), 1.4f);
        Kit.Flyer(new Vector3(-3, 27, 201), "sprite");
        Kit.Fruit(new Vector3(-4, 25, 202));

        int required = Kit.FruitCount - 2;   // Level 4: sve voće osim dva
        Kit.Grapple(new Vector3(0, 29f, 211), required);
        Kit.Grapple(new Vector3(0, 29.5f, 217), required);

        // arena – mramorni otok sa zlatnim stupovima
        Vector3 arena = new Vector3(0, 24, 235);
        Kit.FloatIsland(arena, 16f);
        Kit.Checkpoint(arena + new Vector3(-3, 0, -13));
        for (int i = 0; i < 8; i++)
        {
            float a = i / 8f * Mathf.PI * 2f + 0.3f;
            Kit.MarbleColumn(arena + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 15f, 6f);
        }
        var portal = Kit.Portal(arena + new Vector3(0, 0, 12), "Ribblin_End", arena);
        BuildBird(arena, portal);

        Kit.Player(new Vector3(0, 10.1f, -4), -12f, "Level 4", "Sky Kingdom", "Ribblin_End", required, false, "Level4");
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
        boss.roarSound = "bird_roar"; boss.attackSound = "bird_attack"; boss.hurtSound = "bird_hurt";
        boss.bossName = "Tempestra, Queen of Storms";
        boss.maxHealth = 4;
        boss.arenaCenter = arena;
        boss.arenaRadius = 16f;
        boss.portal = portal;
        boss.model = model;
        boss.wingL = wingL;
        boss.wingR = wingR;
        boss.backPoint = gp;
        boss.hint = "Dodge her feathers, fight the storm wind and her dive! When she lands dizzy, jump on her or grab her with your tongue.";
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

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Upravlja levelom: voće, vrijeme, srca, poruke, pauza, kraj levela.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool Paused { get; private set; }
    public static bool LevelDone { get; private set; }
    public static bool IsGameOver { get; private set; }
    public static bool InputEnabled => !Paused && !LevelDone && !IsGameOver;
    public static int Collected => Instance ? Instance.collected : 0;

    [Header("Level")]
    public string levelTitle = "Level 1";
    public string levelSubtitle = "Tropical Island";
    public string nextScene = "";
    public string menuScene = "Ribblin_Menu";
    public int fruitRequired = 10;
    public bool showControls = true;

    public Material[] fxMaterials;

    int total;
    int collected;
    float timer;
    string message;
    float messageUntil;
    float titleUntil;
    float completeTimer = -1f;
    float overlayAt;         // kad se pojavi natpis "Level complete" (nakon što portal usiše žabicu)
    float gameOverTime;      // koliko dugo traje GAME OVER
    string pendingScene;
    TongueGrapple grapple;

    void Awake()
    {
        Instance = this;
        Paused = false;
        LevelDone = false;
        IsGameOver = false;
        Time.timeScale = 1f;
        Fx.ClearCache();
        Sfx.Init();
    }

    void Start()
    {
        titleUntil = Time.time + 3.5f;
        if (showControls)
            ShowMessage("WASD move · Space jump (x2) · Shift run · Mouse camera · Left click tongue · Esc pause", 9f);
        LockCursor(true);
        grapple = FindAnyObjectByType<TongueGrapple>();
    }

    public static void RegisterFruit() { if (Instance) Instance.total++; }

    public static void CollectFruit()
    {
        if (!Instance) return;
        Instance.collected++;
        Sfx.Play("collect", 0.5f);
        if (Instance.collected == Instance.fruitRequired)
        {
            ShowMessage("You collected enough fruit – the path to the boss is unlocked!", 3.5f);
            Sfx.Play("unlock", 0.6f);
        }
    }

    public static void ShowMessage(string text, float seconds = 2f)
    {
        if (!Instance || string.IsNullOrEmpty(text)) return;
        Instance.message = text;
        Instance.messageUntil = Time.time + seconds;
    }

    // delay = koliko sekundi čekati prije natpisa (portal prvo usiše žabicu)
    public static void CompleteLevel(string next, float delay = 0f)
    {
        if (!Instance || LevelDone || IsGameOver) return;
        LevelDone = true;
        GameStats.AddLevel(Instance.timer, Instance.collected, Instance.total);
        Sfx.Play("win", 0.7f);
        Instance.pendingScene = string.IsNullOrEmpty(next) ? "Ribblin_End" : next;
        Instance.completeTimer = 4f + delay;
        Instance.overlayAt = Time.unscaledTime + delay;
    }

    // Sva tri srca su izgubljena: GAME OVER, razina kreće ispočetka.
    public static void GameOver()
    {
        if (!Instance || IsGameOver || LevelDone) return;
        IsGameOver = true;
        Instance.gameOverTime = 0f;
        Sfx.Play("hurt", 0.8f);
        ThirdPersonCamera.Shake(0.5f);
        Time.timeScale = 0.35f;   // kratko usporenje
    }

    static void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        var kb = Keyboard.current;

        if (LevelDone)
        {
            completeTimer -= Time.unscaledDeltaTime;
            if (completeTimer <= 0f) SceneManager.LoadScene(pendingScene);
            return;
        }

        if (IsGameOver)
        {
            gameOverTime += Time.unscaledDeltaTime;
            if (gameOverTime > 1.2f && Time.timeScale > 0f) { Time.timeScale = 0f; LockCursor(false); }
            if (gameOverTime > 1.6f && kb != null && (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)) RestartLevel();
            return;
        }

        if (kb != null && kb.escapeKey.wasPressedThisFrame) SetPaused(!Paused);
        if (!Paused) timer += Time.deltaTime;

        var mouse = Mouse.current;
        if (!Paused && mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            LockCursor(true);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Tipke za testiranje: F1 = do vrata bossa, F4 = u arenu, F2 = udari bossa, F3 = preskoči level, F5 = sljedeći checkpoint, F6 = bez štete
        if (kb != null && !Paused)
        {
            if (kb.f1Key.wasPressedThisFrame)
            {
                var spawn = GameObject.Find("BossGateSpawn");
                var frog = FrogController.Instance;
                if (spawn && frog)
                {
                    collected = Mathf.Max(collected, fruitRequired);
                    frog.SetCheckpoint(spawn.transform.position);
                    frog.Respawn();
                }
            }
            if (kb.f2Key.wasPressedThisFrame && BossBase.Current) BossBase.Current.DebugHit();
            if (kb.f4Key.wasPressedThisFrame && BossBase.Current && FrogController.Instance)
            {
                var boss = BossBase.Current;
                Vector3 dir = boss.transform.position - boss.arenaCenter; dir.y = 0;
                if (dir.sqrMagnitude < 0.01f) dir = Vector3.forward;
                Vector3 p = boss.arenaCenter - dir.normalized * (boss.arenaRadius * 0.6f) + Vector3.up * 0.5f;
                collected = Mathf.Max(collected, fruitRequired);
                FrogController.Instance.SetCheckpoint(p);
                FrogController.Instance.Respawn();
            }
            if (kb.f3Key.wasPressedThisFrame) CompleteLevel(nextScene);
            if (kb.f5Key.wasPressedThisFrame) JumpToNextCheckpoint();
            if (kb.f6Key.wasPressedThisFrame)
            {
                PlayerHealth.God = !PlayerHealth.God;
                ShowMessage(PlayerHealth.God ? "Test mode: no damage" : "Test mode off", 1.5f);
            }
        }
#endif
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // F5: premjesti žabicu na sljedeći checkpoint (redom kako su postavljeni u levelu)
    int debugCheckpoint = -1;
    void JumpToNextCheckpoint()
    {
        var all = FindObjectsByType<Checkpoint>();
        System.Array.Sort(all, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        if (all.Length == 0 || !FrogController.Instance) return;
        debugCheckpoint = (debugCheckpoint + 1) % all.Length;
        Vector3 p = all[debugCheckpoint].transform.position + Vector3.up * 0.5f;
        FrogController.Instance.SetCheckpoint(p);
        FrogController.Instance.Respawn();
        ShowMessage("Checkpoint " + (debugCheckpoint + 1) + " / " + all.Length, 1.5f);
    }
#endif

    public void SetPaused(bool value)
    {
        if (Paused && !value) GameSettings.Save();   // spremi postavke kad se izađe iz pauze
        Paused = value;
        Time.timeScale = value ? 0f : 1f;
        LockCursor(!value);
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnGUI()
    {
        float s = Ui.S;
        var big = Ui.Label(34);
        var mid = Ui.Label(28, TextAnchor.MiddleCenter);

        // srca
        var hp = PlayerHealth.Instance;
        if (hp)
        {
            for (int i = 0; i < hp.maxHearts; i++)
            {
                bool full = i < hp.Hearts;
                Ui.Text(new Rect((28 + i * 52) * s, 14 * s, 60 * s, 60 * s), "♥", Ui.Label(52),
                    full ? new Color(1f, 0.3f, 0.38f) : new Color(0.3f, 0.3f, 0.3f, 0.8f));
            }
        }

        // voće i vrijeme
        string bossName = BossBase.Current ? BossBase.Current.bossName : "Boss";
        string goal = collected >= fruitRequired ? "  – path to the boss is open!" : $"  ({bossName}: {fruitRequired})";
        Ui.Text(new Rect(28 * s, 74 * s, 900 * s, 50 * s), $"Fruit: {collected} / {total}{goal}", big);
        int m = (int)(timer / 60), sec = (int)(timer % 60);
        Ui.Text(new Rect(28 * s, 116 * s, 500 * s, 50 * s), $"Time: {m:00}:{sec:00}", Ui.Label(28));

        // nišan
        if (!Paused && !LevelDone && !IsGameOver)
        {
            if (grapple == null) grapple = FindAnyObjectByType<TongueGrapple>();
            bool target = grapple && grapple.HasTarget;
            float cx = Screen.width / 2f, cy = Screen.height / 2f;
            float size = (target ? 14f : 6f) * s;
            Color c = target ? new Color(1f, 0.35f, 0.3f) : new Color(1f, 1f, 1f, 0.8f);
            Ui.Box(new Rect(cx - size / 2f, cy - size / 2f, size, size), c);
            if (target)
            {
                float g = 16f * s, l = 10f * s, w = 3f * s;
                Ui.Box(new Rect(cx - g - l, cy - w / 2, l, w), c);
                Ui.Box(new Rect(cx + g, cy - w / 2, l, w), c);
                Ui.Box(new Rect(cx - w / 2, cy - g - l, w, l), c);
                Ui.Box(new Rect(cx - w / 2, cy + g, w, l), c);
            }
        }

        // naslov levela
        if (Time.time < titleUntil && !LevelDone)
        {
            float a = Mathf.Clamp01((titleUntil - Time.time) / 0.8f);
            var old = GUI.color;
            GUI.color = new Color(1, 1, 1, a);
            Ui.Text(new Rect(0, Screen.height * 0.22f, Screen.width, 90 * s), levelTitle, Ui.Label(80, TextAnchor.MiddleCenter));
            Ui.Text(new Rect(0, Screen.height * 0.22f + 90 * s, Screen.width, 60 * s), levelSubtitle, Ui.Label(44, TextAnchor.MiddleCenter), new Color(1f, 0.9f, 0.5f));
            GUI.color = old;
        }

        // poruka
        if (!string.IsNullOrEmpty(message) && Time.time < messageUntil && !Paused && !IsGameOver)
        {
            var r = new Rect(Screen.width * 0.1f, Screen.height - 150 * s, Screen.width * 0.8f, 70 * s);
            Ui.Box(r, new Color(0, 0, 0, 0.35f));
            Ui.Text(r, message, mid);
        }

        // pauza
        if (Paused)
        {
            Ui.Box(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, 0.55f));
            Ui.Text(Ui.Centered(250, 900, 100), "PAUSED", Ui.Label(80, TextAnchor.MiddleCenter));
            if (Ui.Button(Ui.Centered(420, 460, 80), "Resume")) SetPaused(false);
            if (Ui.Button(Ui.Centered(520, 460, 80), "Restart level"))
            {
                SetPaused(false);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if (Ui.Button(Ui.Centered(620, 460, 80), "Main menu"))
            {
                SetPaused(false);
                SceneManager.LoadScene(menuScene);
            }
            // postavke
            GameSettings.MusicVolume = Ui.Slider(Ui.Centered(740, 900, 60), "Music", GameSettings.MusicVolume, 0f, 1f);
            GameSettings.SfxVolume = Ui.Slider(Ui.Centered(810, 900, 60), "Sounds", GameSettings.SfxVolume, 0f, 1f);
            GameSettings.MouseSensitivity = Ui.Slider(Ui.Centered(880, 900, 60), "Mouse", GameSettings.MouseSensitivity, 0.3f, 2f);
        }

        // GAME OVER
        if (IsGameOver)
        {
            float a = Mathf.Clamp01(gameOverTime / 0.8f);
            Ui.Box(new Rect(0, 0, Screen.width, Screen.height), new Color(0.25f, 0.02f, 0.04f, 0.7f * a));
            var old = GUI.color;
            GUI.color = new Color(1, 1, 1, a);
            Ui.Text(Ui.Centered(260, 1200, 140), "GAME OVER", Ui.Label(130, TextAnchor.MiddleCenter), new Color(1f, 0.3f, 0.3f));
            Ui.Text(Ui.Centered(410, 1200, 60), "Ribblin lost all three hearts.", Ui.Label(40, TextAnchor.MiddleCenter));
            GUI.color = old;
            if (gameOverTime > 1.6f)
            {
                if (Ui.Button(Ui.Centered(540, 460, 85), "Try again")) RestartLevel();
                if (Ui.Button(Ui.Centered(645, 460, 85), "Main menu")) { Time.timeScale = 1f; SceneManager.LoadScene(menuScene); }
                Ui.Text(Ui.Centered(760, 1200, 50), "The level starts again from the beginning.", Ui.Label(30, TextAnchor.MiddleCenter), new Color(1f, 1f, 1f, 0.8f));
            }
            return;
        }

        // kraj levela
        if (LevelDone && Time.unscaledTime >= overlayAt)
        {
            Ui.Box(new Rect(0, 0, Screen.width, Screen.height), new Color(0.1f, 0.3f, 0.15f, 0.55f));
            Ui.Text(Ui.Centered(300, 1200, 110), $"{levelTitle} complete!", Ui.Label(84, TextAnchor.MiddleCenter), new Color(1f, 0.9f, 0.4f));
            Ui.Text(Ui.Centered(430, 1200, 60), $"Fruit: {collected} / {total}     Time: {m:00}:{sec:00}", Ui.Label(40, TextAnchor.MiddleCenter));
        }
    }
}

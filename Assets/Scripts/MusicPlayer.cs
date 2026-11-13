using UnityEngine;

// Pušta glazbu za scenu (Assets/Resources/Music/<trackName>.mp3).
// Kad krene borba s bossom, glazba se pretopi u "Boss", a nakon pobjede natrag.
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    public string trackName = "Menu";
    public float volume = 0.35f;
    AudioSource main, boss;
    bool bossMode;

    void Awake() { Instance = this; }

    void Start()
    {
        main = MakeSource(Resources.Load<AudioClip>("Music/" + trackName));
        if (main) main.Play();
    }

    AudioSource MakeSource(AudioClip clip)
    {
        if (clip == null) return null;
        var s = gameObject.AddComponent<AudioSource>();
        s.clip = clip;
        s.loop = true;
        s.volume = 0f;
        s.playOnAwake = false;
        return s;
    }

    public static void SetBossMode(bool on)
    {
        if (!Instance) return;
        Instance.bossMode = on;
        if (on && Instance.boss == null)
        {
            Instance.boss = Instance.MakeSource(Resources.Load<AudioClip>("Music/Boss"));
            if (Instance.boss) Instance.boss.Play();
        }
    }

    void Update()
    {
        float pause = (GameManager.Paused ? 0.4f : 1f) * GameSettings.MusicVolume;
        float k = Time.unscaledDeltaTime * 0.35f;
        bool useBoss = bossMode && boss != null;
        if (main) main.volume = Mathf.MoveTowards(main.volume, useBoss ? 0f : volume * pause, k);
        if (boss) boss.volume = Mathf.MoveTowards(boss.volume, useBoss ? volume * pause : 0f, k);
    }
}

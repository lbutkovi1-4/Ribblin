using UnityEngine;

// Postavke igrača (glasnoća i osjetljivost miša). Pamte se između pokretanja igre (PlayerPrefs).
public static class GameSettings
{
    const string MusicKey = "MusicVolume";
    const string SfxKey = "SfxVolume";
    const string MouseKey = "MouseSensitivity";

    static bool loaded;
    static float music = 1f, sfx = 1f, mouse = 1f;

    static void Load()
    {
        if (loaded) return;
        music = PlayerPrefs.GetFloat(MusicKey, 1f);
        sfx = PlayerPrefs.GetFloat(SfxKey, 1f);
        mouse = PlayerPrefs.GetFloat(MouseKey, 1f);
        loaded = true;
    }

    // 0 – 1
    public static float MusicVolume
    {
        get { Load(); return music; }
        set { Load(); music = Mathf.Clamp01(value); PlayerPrefs.SetFloat(MusicKey, music); }
    }

    // 0 – 1
    public static float SfxVolume
    {
        get { Load(); return sfx; }
        set { Load(); sfx = Mathf.Clamp01(value); PlayerPrefs.SetFloat(SfxKey, sfx); }
    }

    // množitelj osjetljivosti kamere: 0.3 (sporo) – 2 (brzo)
    public static float MouseSensitivity
    {
        get { Load(); return mouse; }
        set { Load(); mouse = Mathf.Clamp(value, 0.3f, 2f); PlayerPrefs.SetFloat(MouseKey, mouse); }
    }

    public static void Save() => PlayerPrefs.Save();
}

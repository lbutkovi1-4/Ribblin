using System.Collections.Generic;
using UnityEngine;

// Zvučni efekti. Svi zvukovi su prave snimke iz besplatnih CC0 paketa
// (Kenney Impact/Interface Sounds i "80 CC0 creature SFX" od rubberduck),
// spremljene u Assets/Resources/Sfx/<ime>.ogg
public static class Sfx
{
    static AudioSource source;
    static readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();

    public static void Init()
    {
        if (source != null) return;
        var go = new GameObject("Sfx");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public static void Play(string name, float volume = 0.6f)
    {
        if (string.IsNullOrEmpty(name)) return;
        if (source == null) Init();
        if (!cache.TryGetValue(name, out var clip))
        {
            clip = Resources.Load<AudioClip>("Sfx/" + name);
            cache[name] = clip;
        }
        if (clip == null) return;
        source.pitch = 1f;
        source.PlayOneShot(clip, volume);
    }
}

using System.Collections.Generic;
using UnityEngine;

// Zvučni efekti koji se generiraju u kodu (ne trebaju datoteke).
public static class Sfx
{
    static AudioSource source;
    static Dictionary<string, AudioClip> clips;
    const int Rate = 44100;
    static System.Random rng = new System.Random(7);

    public static void Init()
    {
        if (source != null) return;
        var go = new GameObject("Sfx");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        clips = new Dictionary<string, AudioClip>();
        Build();
    }

    public static void Play(string name, float volume = 0.6f)
    {
        if (source == null) Init();
        if (clips.TryGetValue(name, out var c)) source.PlayOneShot(c, volume);
    }

    static float Noise() => (float)(rng.NextDouble() * 2.0 - 1.0);
    static float Sweep(float f0, float f1, float d, float t) => Mathf.Sin(2f * Mathf.PI * (f0 * t + (f1 - f0) * t * t / (2f * d)));
    static float Env(float t, float d) => Mathf.Clamp01(t * 300f) * Mathf.Clamp01(1f - t / d);
    static float Tone(float f, float t) => Mathf.Sin(2f * Mathf.PI * f * t);

    static void Add(string name, float dur, System.Func<float, float> f, bool lowpass = false)
    {
        int n = Mathf.CeilToInt(dur * Rate);
        var data = new float[n];
        float lp = 0f;
        for (int i = 0; i < n; i++)
        {
            float v = f(i / (float)Rate);
            if (lowpass) { lp += (v - lp) * 0.08f; v = lp * 2.5f; }
            data[i] = Mathf.Clamp(v, -1f, 1f) * 0.6f;
        }
        var clip = AudioClip.Create(name, n, 1, Rate, false);
        clip.SetData(data, 0);
        clips[name] = clip;
    }

    static float Notes(float t, float noteLen, params float[] freqs)
    {
        int i = Mathf.Min(freqs.Length - 1, (int)(t / noteLen));
        float lt = t - i * noteLen;
        float len = i == freqs.Length - 1 ? noteLen * 3f : noteLen;
        return (Tone(freqs[i], t) * 0.7f + Tone(freqs[i] * 2f, t) * 0.15f) * Env(lt, len);
    }

    static void Build()
    {
        Add("jump", 0.14f, t => Sweep(320, 640, 0.14f, t) * Env(t, 0.14f));
        Add("jump2", 0.14f, t => Sweep(520, 980, 0.14f, t) * Env(t, 0.14f));
        Add("collect", 0.25f, t => (t < 0.07f ? Tone(988, t) : Tone(1319, t)) * Env(t, 0.25f));
        Add("hurt", 0.3f, t => (Sweep(420, 110, 0.3f, t) * 0.7f + Noise() * 0.3f) * Env(t, 0.3f));
        Add("stomp", 0.2f, t => (Sweep(240, 60, 0.2f, t) + Noise() * 0.2f) * Env(t, 0.2f));
        Add("tongue", 0.14f, t => (Noise() * 0.5f + Sweep(900, 300, 0.14f, t) * 0.4f) * Env(t, 0.14f), true);
        Add("gulp", 0.18f, t => Sweep(380, 120, 0.18f, t) * Env(t, 0.18f));
        Add("boing", 0.35f, t => Mathf.Sin(2f * Mathf.PI * (180f * t + 500f * t * t) + 2f * Mathf.Sin(2f * Mathf.PI * 14f * t)) * Env(t, 0.35f));
        Add("checkpoint", 0.3f, t => Notes(t, 0.07f, 660, 880, 1100));
        Add("unlock", 0.45f, t => Notes(t, 0.08f, 523, 659, 784, 1047));
        Add("win", 1.2f, t => Notes(t, 0.16f, 523, 659, 784, 1047, 784, 1047));
        Add("bosshit", 0.4f, t => (Sweep(160, 40, 0.4f, t) + Noise() * 0.4f) * Env(t, 0.4f));
        Add("thunder", 0.7f, t => Noise() * Env(t, 0.7f) * 1.5f, true);
        Add("splash", 0.4f, t => Noise() * Env(t, 0.4f) * 1.2f, true);
        Add("whoosh", 0.35f, t => Noise() * Mathf.Sin(Mathf.PI * t / 0.35f) * 1.3f, true);
        Add("roar", 0.9f, t => (Sweep(110, 70, 0.9f, t) * 0.6f + Tone(57, t) * 0.3f + Noise() * 0.25f) * Env(t, 0.9f));
        Add("geyser", 0.5f, t => Noise() * Env(t, 0.5f), true);
    }
}

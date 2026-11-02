using UnityEngine;

// Kamen lave koji boss baca u luku prema žabici.
public class LavaRock : MonoBehaviour
{
    Vector3 from, to;
    float duration, t, arc;

    public static void Spawn(Vector3 from, Vector3 to, float duration)
    {
        var go = Fx.Prim(PrimitiveType.Sphere, from, Vector3.one * 1.2f, "Lava");
        var r = go.AddComponent<LavaRock>();
        r.from = from;
        r.to = to;
        r.duration = duration;
        r.arc = Mathf.Max(4f, Vector3.Distance(from, to) * 0.35f);
        TimedStrike.Spawn(to, 1.9f, duration, "rock");
        Sfx.Play("whoosh", 0.4f);
    }

    void Update()
    {
        if (GameManager.Paused) return;
        t += Time.deltaTime / duration;
        transform.position = Vector3.Lerp(from, to, t) + Vector3.up * Mathf.Sin(Mathf.PI * Mathf.Clamp01(t)) * arc;
        transform.Rotate(200f * Time.deltaTime, 90f * Time.deltaTime, 0);
        if (t >= 1f) Destroy(gameObject);
    }
}

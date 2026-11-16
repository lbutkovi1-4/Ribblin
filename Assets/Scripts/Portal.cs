using System.Collections.Generic;
using UnityEngine;

// Portal na kraju levela – pojavi se kad pobijediš bossa.
// Vrtlog se vrti, iskrice se uvlače u sredinu, a kad žabica priđe,
// portal je povuče k sebi, zavrti i "usiše" (kao warp u Crash Bandicootu).
public class Portal : MonoBehaviour
{
    public string nextScene;
    public Transform ring;        // vanjski zlatni prsten
    public Transform ringInner;   // unutarnji prsten (vrti se na drugu stranu)
    public Transform swirl;       // spiralni krakovi vrtloga
    public Transform core;        // svjetleća sredina
    public float pullRadius = 6f; // odakle počinje vući žabicu
    public float enterRadius = 2.2f;

    enum S { Idle, Warp, Done }
    S s = S.Idle;
    float t;
    float sparkTimer;
    Vector3 coreBase = Vector3.one;
    Vector3 frogStartScale;
    float warpAngle, warpDist;
    class Spark { public Transform tr; public float angle, radius; }
    readonly List<Spark> sparks = new List<Spark>();

    Vector3 Center => core ? core.position : transform.position + Vector3.up * 2.2f;

    void OnEnable()
    {
        if (core) coreBase = core.localScale;
        Fx.Puff(Center, "PortalCore", 4f);
        Fx.Burst(Center, "PortalCore", 14, 1.4f);
        Sfx.Play("unlock", 0.6f);
    }

    void Update()
    {
        if (GameManager.Paused) return;
        float dt = Time.deltaTime;
        t += dt;

        // vrtnja dijelova portala (brže dok usisava žabicu)
        float spin = s == S.Warp ? 3f : 1f;
        if (ring) ring.Rotate(0, 0, 60f * spin * dt, Space.Self);
        if (ringInner) ringInner.Rotate(0, 0, -110f * spin * dt, Space.Self);
        if (swirl) swirl.Rotate(0, 0, 220f * spin * dt, Space.Self);
        if (core) core.localScale = coreBase * (1f + Mathf.Sin(t * 5f) * 0.06f);

        UpdateSparks(dt);

        var frog = FrogController.Instance;
        if (!frog || s == S.Done) return;
        Vector3 fp = frog.transform.position + Vector3.up * 0.5f;

        if (s == S.Idle)
        {
            float d = Vector3.Distance(fp, Center);
            // blago vuče žabicu prema sebi
            if (d < pullRadius && !GameManager.LevelDone && !GameManager.IsGameOver)
            {
                Vector3 pull = (Center - fp); pull.y = 0;
                frog.Wind = pull.normalized * Mathf.Lerp(7f, 1.5f, d / pullRadius);
            }
            if (d < enterRadius && !GameManager.IsGameOver) StartWarp(frog);
        }
        else if (s == S.Warp)
        {
            // žabica kruži po spirali prema sredini, smanjuje se i nestane
            float k = Mathf.Clamp01(t / 1.3f);
            warpAngle += Mathf.Lerp(6f, 22f, k) * dt;
            float r = Mathf.Lerp(warpDist, 0f, k * k);
            Vector3 local = new Vector3(Mathf.Cos(warpAngle) * r, Mathf.Sin(warpAngle) * r - 0.5f * (1f - k), 0.6f * (1f - k));
            frog.transform.position = Center + transform.rotation * local;
            frog.transform.Rotate(0, 900f * dt, 0, Space.World);
            frog.transform.localScale = frogStartScale * Mathf.Max(0.02f, 1f - k);
            if (k >= 1f)
            {
                s = S.Done;
                foreach (var rend in frog.GetComponentsInChildren<Renderer>()) rend.enabled = false;
                Fx.Burst(Center, "PortalCore", 18, 1.6f);
                Fx.Puff(Center, "Dust", 5f);
                ThirdPersonCamera.Shake(0.4f);
            }
        }
    }

    void StartWarp(FrogController frog)
    {
        s = S.Warp;
        t = 0f;
        frog.Wind = Vector3.zero;
        // žabica više ne upravlja sama sobom
        var cc = frog.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        var tongue = frog.GetComponent<TongueGrapple>();
        if (tongue) tongue.enabled = false;
        frog.enabled = false;
        frogStartScale = frog.transform.localScale;
        Vector3 off = Quaternion.Inverse(transform.rotation) * (frog.transform.position + Vector3.up * 0.5f - Center);
        warpAngle = Mathf.Atan2(off.y, off.x);
        warpDist = Mathf.Max(0.8f, new Vector2(off.x, off.y).magnitude);
        Sfx.Play("jump", 0.7f);
        GameManager.CompleteLevel(nextScene, 1.9f);
    }

    // iskrice se stvaraju na rubu i spiralno uvlače u sredinu
    void UpdateSparks(float dt)
    {
        sparkTimer -= dt;
        if (sparkTimer <= 0f)
        {
            sparkTimer = s == S.Warp ? 0.02f : 0.07f;
            var go = Fx.Prim(PrimitiveType.Cube, Center, Vector3.one * Random.Range(0.12f, 0.25f), Random.value > 0.5f ? "PortalCore" : "Gold");
            go.name = "Spark";
            go.transform.SetParent(transform, true);
            sparks.Add(new Spark { tr = go.transform, angle = Random.value * Mathf.PI * 2f, radius = Random.Range(2.6f, 3.6f) });
        }
        for (int i = sparks.Count - 1; i >= 0; i--)
        {
            var sp = sparks[i];
            if (!sp.tr) { sparks.RemoveAt(i); continue; }
            sp.angle += (2.5f + 3f / Mathf.Max(0.3f, sp.radius)) * dt;
            sp.radius -= (s == S.Warp ? 4f : 1.6f) * dt;
            if (sp.radius <= 0.1f) { Destroy(sp.tr.gameObject); sparks.RemoveAt(i); continue; }
            Vector3 local = new Vector3(Mathf.Cos(sp.angle) * sp.radius, Mathf.Sin(sp.angle) * sp.radius, 0.25f);
            sp.tr.position = Center + transform.rotation * local;
            sp.tr.Rotate(0, 0, 400f * dt);
        }
    }
}

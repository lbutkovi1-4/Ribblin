using UnityEngine;

// Crveni krug upozorenja na tlu, a nakon kratkog vremena udar (munja ili kamen lave).
public class TimedStrike : MonoBehaviour
{
    float delay;
    float radius;
    string kind;
    float t;
    Transform warn;
    Transform ring;
    bool done;

    public static void Spawn(Vector3 groundPos, float radius, float delay, string kind)
    {
        var go = new GameObject("Strike_" + kind);
        go.transform.position = groundPos;
        var s = go.AddComponent<TimedStrike>();
        s.radius = radius;
        s.delay = delay;
        s.kind = kind;
    }

    void Start()
    {
        ring = Fx.Prim(PrimitiveType.Cylinder, transform.position + Vector3.up * 0.04f, new Vector3(radius * 2f, 0.01f, radius * 2f), "WarningDark").transform;
        warn = Fx.Prim(PrimitiveType.Cylinder, transform.position + Vector3.up * 0.06f, Vector3.zero, "Warning").transform;
        ring.SetParent(transform, true);
        warn.SetParent(transform, true);
    }

    void Update()
    {
        if (GameManager.Paused) return;
        t += Time.deltaTime;
        if (!done)
        {
            float k = Mathf.Clamp01(t / delay);
            warn.localScale = new Vector3(radius * 2f * k, 0.012f, radius * 2f * k);
            if (t >= delay) Strike();
        }
        else if (t > delay + 0.3f) Destroy(gameObject);
    }

    void Strike()
    {
        done = true;
        if (kind == "lightning")
        {
            var bolt = Fx.Prim(PrimitiveType.Cylinder, transform.position + Vector3.up * 15f, new Vector3(0.6f, 15f, 0.6f), "Lightning");
            Destroy(bolt, 0.25f);
            Fx.Puff(transform.position + Vector3.up * 0.5f, "Lightning", radius * 2f);
            Sfx.Play("thunder", 0.6f);
            ThirdPersonCamera.Shake(0.25f);
        }
        else
        {
            Fx.Puff(transform.position + Vector3.up * 0.5f, "Lava", radius * 2.2f);
            Sfx.Play("stomp", 0.6f);
            ThirdPersonCamera.Shake(0.2f);
        }

        var frog = FrogController.Instance;
        if (frog)
        {
            Vector3 fp = frog.transform.position;
            Vector3 a = fp; a.y = 0;
            Vector3 b = transform.position; b.y = 0;
            if (Vector3.Distance(a, b) < radius + 0.35f && fp.y - transform.position.y < 2.5f && fp.y - transform.position.y > -1f)
                PlayerHealth.Instance?.Damage(1, transform.position);
        }
    }
}

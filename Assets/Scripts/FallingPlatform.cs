using UnityEngine;

// Zamka: platforma koja se zatrese kad staneš na nju, pa padne. Nakon nekoliko sekundi se vrati.
public class FallingPlatform : MonoBehaviour
{
    public float shakeTime = 0.6f;
    public float respawnTime = 4f;
    enum S { Idle, Shaking, Falling, Gone }
    S s = S.Idle;
    Vector3 start;
    float t, vy;
    Collider[] cols;
    Renderer[] rends;

    void Start()
    {
        start = transform.position;
        cols = GetComponentsInChildren<Collider>();
        rends = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (GameManager.Paused) return;
        t += Time.deltaTime;
        var frog = FrogController.Instance;
        switch (s)
        {
            case S.Idle:
                if (frog && frog.IsGrounded && frog.GroundTransform && frog.GroundTransform.IsChildOf(transform))
                {
                    s = S.Shaking; t = 0f;
                    Sfx.Play("crack", 0.5f);
                }
                break;
            case S.Shaking:
                transform.position = start + Random.insideUnitSphere * 0.07f;
                if (t > shakeTime) { s = S.Falling; t = 0f; vy = 0f; transform.position = start; }
                break;
            case S.Falling:
                vy -= 20f * Time.deltaTime;
                transform.position += Vector3.up * vy * Time.deltaTime;
                if (t > 1.6f) { s = S.Gone; t = 0f; Show(false); }
                break;
            case S.Gone:
                if (t > respawnTime) { transform.position = start; Show(true); s = S.Idle; Fx.Puff(start, "Dust", 2f); }
                break;
        }
    }

    void Show(bool v)
    {
        foreach (var c in cols) if (c) c.enabled = v;
        foreach (var r in rends) if (r) r.enabled = v;
    }
}

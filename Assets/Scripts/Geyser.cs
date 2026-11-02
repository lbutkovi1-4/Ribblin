using UnityEngine;

// Vatreni gejzir: neko vrijeme miruje, zatim mjehuri (upozorenje), pa izbaci stup vatre.
public class Geyser : MonoBehaviour
{
    public float idleTime = 2.2f;
    public float warnTime = 0.9f;
    public float activeTime = 1.2f;
    public float offset = 0f;
    public float height = 6f;
    public float radius = 0.9f;
    public Transform column;

    float t;
    float current;
    bool wasActive;

    void Start() { t = offset; }

    void Update()
    {
        if (GameManager.Paused || !column) return;
        t += Time.deltaTime;
        float cycle = idleTime + warnTime + activeTime;
        float c = Mathf.Repeat(t, cycle);
        float h;
        bool active = false;
        if (c < idleTime) h = 0.05f;
        else if (c < idleTime + warnTime) h = 0.4f + Mathf.Sin(Time.time * 40f) * 0.15f;
        else { h = height; active = true; }

        current = Mathf.Lerp(current, h, 18f * Time.deltaTime);
        float w = radius * 2f * (active ? 1f : 0.6f);
        column.localScale = new Vector3(w, Mathf.Max(0.01f, current / 2f), w);
        column.localPosition = Vector3.up * current / 2f;

        var frog = FrogController.Instance;
        if (active && !wasActive && frog && Vector3.Distance(frog.transform.position, transform.position) < 14f)
            Sfx.Play("geyser", 0.4f);
        wasActive = active;

        if (active && frog)
        {
            Vector3 fp = frog.transform.position;
            Vector3 a = fp; a.y = 0;
            Vector3 b = transform.position; b.y = 0;
            if (Vector3.Distance(a, b) < radius + 0.4f && fp.y < transform.position.y + current && fp.y > transform.position.y - 1f)
                PlayerHealth.Instance?.Damage(1, transform.position);
        }
    }
}

using UnityEngine;

// Leteći neprijatelj (pčela, šišmiš, olujni duh). Lebdi, a kad te vidi zuji i obruši se na tebe.
// Pobijedi ga skokom na njega ili jezikom.
public class FlyerEnemy : Enemy
{
    public float range = 11f;
    public float diveSpeed = 13f;
    public float radius = 0.6f;
    public float wander = 2.5f;
    public float cooldownTime = 2.5f;
    public Transform model, wingL, wingR;

    enum S { Hover, Windup, Dive, Return }
    S s = S.Hover;
    Vector3 home;
    Vector3 diveDir;
    float t;
    float cooldown = 1f;
    float seed;

    public override Vector3 Center => transform.position;

    void Start()
    {
        home = transform.position;
        seed = Random.value * 10f;
    }

    void Update()
    {
        if (dead || GameManager.Paused) return;
        float flap = Mathf.Sin(Time.time * 30f) * 40f;
        if (wingL) wingL.localRotation = Quaternion.Euler(0, 0, flap);
        if (wingR) wingR.localRotation = Quaternion.Euler(0, 0, -flap);

        var frog = FrogController.Instance;
        Vector3 fp = frog ? frog.transform.position + Vector3.up * 0.6f : home;
        cooldown -= Time.deltaTime;
        t += Time.deltaTime;

        switch (s)
        {
            case S.Hover:
                {
                    float a = Time.time * 0.8f + seed;
                    Vector3 target = home + new Vector3(Mathf.Cos(a), Mathf.Sin(a * 2f) * 0.2f, Mathf.Sin(a)) * wander;
                    transform.position = Vector3.MoveTowards(transform.position, target, 3f * Time.deltaTime);
                    Face(target - transform.position);
                    if (frog && cooldown <= 0f && Vector3.Distance(fp, transform.position) < range && Vector3.Distance(fp, home) < range + 4f)
                    {
                        s = S.Windup; t = 0f;
                        Sfx.Play("buzz", 0.5f);
                    }
                }
                break;
            case S.Windup:
                Face(fp - transform.position);
                if (model) model.localPosition = Random.insideUnitSphere * 0.08f;
                if (t > 0.6f)
                {
                    if (model) model.localPosition = Vector3.zero;
                    diveDir = (fp - transform.position).normalized;
                    s = S.Dive; t = 0f;
                }
                break;
            case S.Dive:
                transform.position += diveDir * diveSpeed * Time.deltaTime;
                Face(diveDir);
                if (t > 0.9f || Physics.CheckSphere(transform.position, 0.3f, ~0, QueryTriggerInteraction.Ignore)) { s = S.Return; t = 0f; }
                break;
            case S.Return:
                transform.position = Vector3.MoveTowards(transform.position, home, 5f * Time.deltaTime);
                Face(home - transform.position);
                if (Vector3.Distance(transform.position, home) < 0.3f) { s = S.Hover; cooldown = cooldownTime; }
                break;
        }

        if (frog)
        {
            Vector3 p = frog.transform.position;
            float d = Vector3.Distance(p + Vector3.up * 0.6f, transform.position);
            if (d < radius + 0.7f)
            {
                if (p.y > transform.position.y - 0.4f && frog.Velocity.y < 0.5f && !frog.IsGrounded)
                {
                    frog.Bounce(2.8f);
                    Sfx.Play("stomp", 0.6f);
                    Kill();
                }
                else PlayerHealth.Instance?.Damage(1, transform.position);
            }
        }
    }

    void Face(Vector3 dir)
    {
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
    }
}

using UnityEngine;

// Mali zmaj iz vulkana. Lebdi, a kad ugleda žabicu, napuhne se i bljune vatru prema njoj.
// Pobijedi ga skokom na leđa ili ga pojedi jezikom.
public class FireDragon : Enemy
{
    public float range = 13f;
    public float wander = 2f;
    public float breathTime = 1.0f;    // koliko dugo bljuje vatru
    public float cooldownTime = 2.6f;
    public float flameSpeed = 10f;
    public Transform model, wingL, wingR, mouth;

    enum S { Hover, Windup, Breathe }
    S s = S.Hover;
    Vector3 home;
    float t, cooldown = 1.5f, seed, flameTimer;
    Vector3 baseScale;

    public override Vector3 Center => transform.position;

    void Start()
    {
        home = transform.position;
        seed = Random.value * 10f;
        if (model) baseScale = model.localScale;
    }

    void Update()
    {
        if (dead || GameManager.Paused) return;
        float flap = Mathf.Sin(Time.time * 14f + seed) * 35f;
        if (wingL) wingL.localRotation = Quaternion.Euler(0, 0, flap);
        if (wingR) wingR.localRotation = Quaternion.Euler(0, 0, -flap);

        var frog = FrogController.Instance;
        Vector3 fp = frog ? frog.transform.position + Vector3.up * 0.5f : home;
        float dist = Vector3.Distance(fp, transform.position);
        cooldown -= Time.deltaTime;
        t += Time.deltaTime;

        switch (s)
        {
            case S.Hover:
                {
                    float a = Time.time * 0.7f + seed;
                    Vector3 target = home + new Vector3(Mathf.Cos(a), Mathf.Sin(a * 1.7f) * 0.3f, Mathf.Sin(a)) * wander;
                    transform.position = Vector3.MoveTowards(transform.position, target, 2.5f * Time.deltaTime);
                    Face(frog && dist < range + 3f ? fp - transform.position : target - transform.position, 5f);
                    if (frog && cooldown <= 0f && dist < range && !GameManager.IsGameOver)
                    {
                        s = S.Windup; t = 0f;
                        Sfx.Play("buzz", 0.4f);
                    }
                }
                break;

            case S.Windup:
                // napuhne se i zatrese – upozorenje
                Face(fp - transform.position, 10f);
                if (model)
                {
                    model.localScale = baseScale * (1f + Mathf.Clamp01(t / 0.8f) * 0.25f);
                    model.localPosition = Random.insideUnitSphere * 0.05f;
                }
                if (t > 0.8f)
                {
                    if (model) { model.localScale = baseScale; model.localPosition = Vector3.zero; }
                    s = S.Breathe; t = 0f;
                    Sfx.Play("geyser", 0.5f);
                }
                break;

            case S.Breathe:
                Face(fp - transform.position, 4f);
                flameTimer -= Time.deltaTime;
                if (flameTimer <= 0f)
                {
                    flameTimer = 0.07f;
                    Vector3 from = mouth ? mouth.position : transform.position + transform.forward * 0.8f;
                    Vector3 dir = (fp - from).normalized;
                    dir = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-7f, 7f), 0) * dir;
                    var p = Projectile.Spawn(from, dir * flameSpeed, Random.value > 0.4f ? "Flame" : "FlameCore", Random.Range(0.35f, 0.55f));
                    p.life = range / flameSpeed + 0.2f;
                    p.grow = 1.8f;
                }
                if (t > breathTime) { s = S.Hover; cooldown = cooldownTime; }
                break;
        }

        // dodir: skok odozgo ga uništi, sa strane boli
        if (frog)
        {
            Vector3 p = frog.transform.position;
            float d = Vector3.Distance(p + Vector3.up * 0.5f, transform.position);
            if (d < 1.2f)
            {
                if (p.y > transform.position.y - 0.3f && frog.Velocity.y < 0.5f && !frog.IsGrounded)
                {
                    frog.Bounce(3f);
                    Sfx.Play("stomp", 0.6f);
                    Kill();
                }
                else PlayerHealth.Instance?.Damage(1, transform.position);
            }
        }
    }

    void Face(Vector3 dir, float speed)
    {
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime);
    }
}

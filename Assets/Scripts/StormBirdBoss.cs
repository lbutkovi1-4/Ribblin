using UnityEngine;

// BOSS 4 – Tempestra, Queen of Storms.
// Moći: 1) rafal perja u lepezi (izbjegni), 2) olujni vjetar koji te gura prema rubu,
// 3) obrušavanje na tebe. Nakon obrušavanja ostane ošamućena – skoči na nju ili je uhvati jezikom.
public class StormBirdBoss : BossBase
{
    public float flyHeight = 8f;
    public float circleRadius = 10f;
    public float flySpeed = 0.6f;
    public GrapplePoint backPoint;
    public Transform model;
    public Transform wingL, wingR;
    public float dizzyTime = 4.5f;
    public float topHeight = 2.4f;
    public string gustSound = "bird_gust";

    enum S { Fly, Volley, Gust, DiveWindup, Dive, Dizzy, Rise }
    S s = S.Fly;
    int nextAttack;
    float angle;
    int volleysLeft;
    float volleyTimer;
    Vector3 moveFrom, moveTo, windDir;
    float streakTimer;

    protected override void Start()
    {
        base.Start();
        if (backPoint)
        {
            backPoint.targetable = false;
            backPoint.Grappled += _ => { if (s == S.Dizzy) DoHit(true); };
        }
    }

    protected override void OnActivate()
    {
        StartRise();
    }

    Vector3 CirclePoint(float a) => arenaCenter + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * circleRadius + Vector3.up * flyHeight;
    bool Angry => health <= maxHealth / 2;

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        float flapSpeed = s == S.Gust ? 18f : 9f;
        float flap = s == S.Dizzy ? 10f : Mathf.Sin(Time.time * flapSpeed) * 35f;
        if (wingL) wingL.localRotation = Quaternion.Euler(0, 0, flap);
        if (wingR) wingR.localRotation = Quaternion.Euler(0, 0, -flap);

        switch (s)
        {
            case S.Fly:
                angle += flySpeed * Time.deltaTime * (Angry ? 1.4f : 1f);
                MoveTo(CirclePoint(angle), 3f);
                if (stateTimer <= 0f) ChooseAttack();
                break;

            case S.Volley:
                FaceTowards(PlayerPos, 6f);
                volleyTimer -= Time.deltaTime;
                if (volleyTimer <= 0f && volleysLeft > 0)
                {
                    ShootFeathers(Angry ? 7 : 5);
                    volleysLeft--;
                    volleyTimer = Angry ? 0.6f : 0.85f;
                }
                if (volleysLeft <= 0 && volleyTimer <= 0f) { s = S.Fly; stateTimer = 1.2f; }
                break;

            case S.Gust:
                FaceTowards(arenaCenter, 4f);
                if (frog) frog.Wind = windDir * (Angry ? 8.5f : 6.5f);
                streakTimer -= Time.deltaTime;
                if (streakTimer <= 0f) { SpawnStreak(); streakTimer = 0.08f; }
                if (stateTimer <= 0f) { s = S.Fly; stateTimer = 1.2f; }
                break;

            case S.DiveWindup:
                FaceTowards(PlayerPos, 8f);
                if (model) model.localPosition = Random.insideUnitSphere * 0.1f;
                if (stateTimer <= 0f)
                {
                    if (model) model.localPosition = Vector3.zero;
                    moveFrom = transform.position;
                    moveTo = ClampToArena(PlayerPos, 3f);
                    s = S.Dive;
                    stateTimer = 0.9f;
                }
                break;

            case S.Dive:
                {
                    float t = 1f - Mathf.Clamp01(stateTimer / 0.9f);
                    transform.position = Vector3.Lerp(moveFrom, moveTo, t * t);
                    TouchDamage(2.6f, topHeight + 0.5f);
                    if (stateTimer <= 0f)
                    {
                        s = S.Dizzy;
                        stateTimer = dizzyTime;
                        if (backPoint) backPoint.targetable = true;
                        Sfx.Play("stomp", 0.9f);
                        ThirdPersonCamera.Shake(0.5f);
                        Fx.Puff(transform.position, "Dust", 4f);
                        Shockwave.Spawn(transform.position + Vector3.up * 0.05f, 7f, 7f);
                        GameManager.ShowMessage("Tempestra is dizzy! Jump on her or grab her with your tongue!", 3f);
                    }
                }
                break;

            case S.Dizzy:
                if (model) model.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f) * 10f, Time.time * 120f, Mathf.Cos(Time.time * 5f) * 10f);
                if (IsStomped(2.6f, transform.position.y + topHeight))
                {
                    frog.Bounce(5f);
                    DoHit(false);
                }
                else if (stateTimer <= 0f) StartRise();
                break;

            case S.Rise:
                {
                    float t = 1f - Mathf.Clamp01(stateTimer / 1.2f);
                    transform.position = Vector3.Lerp(moveFrom, CirclePoint(angle), Mathf.SmoothStep(0, 1, t));
                    if (stateTimer <= 0f) { s = S.Fly; stateTimer = 1.5f; }
                }
                break;
        }
    }

    void MoveTo(Vector3 target, float speed)
    {
        Vector3 prev = transform.position;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
        Vector3 vel = transform.position - prev; vel.y = 0;
        if (vel.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vel), 5f * Time.deltaTime);
    }

    void ChooseAttack()
    {
        switch (nextAttack % 3)
        {
            case 0:
                s = S.Volley;
                volleysLeft = 3;
                volleyTimer = 0.4f;
                Attack(0.9f);
                break;
            case 1:
                s = S.Gust;
                stateTimer = Angry ? 4f : 3f;
                windDir = PlayerPos - transform.position; windDir.y = 0;
                windDir = windDir.sqrMagnitude > 0.01f ? windDir.normalized : Vector3.forward;
                Sfx.Play(gustSound, 1f);
                GameManager.ShowMessage("Storm wind! Run against it or you'll be blown off!", 2.5f);
                break;
            default:
                s = S.DiveWindup;
                stateTimer = 0.7f;
                Sfx.Play(roarSound, 0.9f);
                break;
        }
        nextAttack++;
    }

    void ShootFeathers(int count)
    {
        Attack(0.6f);
        Vector3 from = transform.position + Vector3.up * 1.2f;
        Vector3 to = PlayerPos + Vector3.up * 0.6f - from;
        float spread = 50f;
        for (int i = 0; i < count; i++)
        {
            float a = Mathf.Lerp(-spread / 2f, spread / 2f, count == 1 ? 0.5f : i / (float)(count - 1));
            Vector3 dir = Quaternion.AngleAxis(a, Vector3.up) * to.normalized;
            var p = Projectile.Spawn(from, dir * 11f, "Feather", 0.45f, PrimitiveType.Cube);
            p.transform.localScale = new Vector3(0.15f, 0.15f, 0.9f);
        }
    }

    void SpawnStreak()
    {
        if (!frog) return;
        Vector3 side = Vector3.Cross(Vector3.up, windDir);
        Vector3 p = PlayerPos + side * Random.Range(-6f, 6f) - windDir * 6f + Vector3.up * Random.Range(0.3f, 3f);
        var go = Fx.Prim(PrimitiveType.Cube, p, new Vector3(0.05f, 0.05f, 2.5f), "Cloud");
        go.transform.rotation = Quaternion.LookRotation(windDir);
        var d = go.AddComponent<Debris>();
        d.velocity = windDir * 25f;
        d.gravity = 0f;
        d.life = 0.5f;
    }

    void DoHit(bool byTongue)
    {
        Hit();
        if (byTongue && frog)
        {
            Vector3 away = PlayerPos - transform.position;
            away.y = 0;
            away = away.sqrMagnitude > 0.01f ? away.normalized : -transform.forward;
            frog.Velocity = away * 10f + Vector3.up * 8f;
        }
        StartRise();
    }

    void StartRise()
    {
        if (backPoint) backPoint.targetable = false;
        if (model) model.localRotation = Quaternion.identity;
        s = S.Rise;
        stateTimer = 1.2f;
        moveFrom = transform.position;
    }
}

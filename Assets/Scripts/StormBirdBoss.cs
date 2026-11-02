using UnityEngine;

// BOSS 4 – Oluja, olujna ptica.
// Kruži iznad arene i baca munje (bježi iz crvenih krugova). Zatim se spusti ošamućena – skoči na nju ili je uhvati jezikom.
public class StormBirdBoss : BossBase
{
    public float flyHeight = 9f;
    public float circleRadius = 10f;
    public float flySpeed = 0.55f;
    public GrapplePoint backPoint;
    public Transform model;
    public Transform wingL, wingR;
    public int strikesBeforeDive = 5;
    public float dizzyTime = 5f;
    public float topHeight = 2.4f;

    enum S { Fly, Dive, Dizzy, Rise }
    S s = S.Fly;
    float angle;
    int strikes;
    float strikeTimer;
    Vector3 moveFrom, moveTo;

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
        s = S.Rise;
        stateTimer = 1.2f;
        moveFrom = transform.position;
        strikeTimer = 1.5f;
    }

    Vector3 CirclePoint(float a) => arenaCenter + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * circleRadius + Vector3.up * flyHeight;

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        float flap = s == S.Dizzy ? 10f : Mathf.Sin(Time.time * 9f) * 35f;
        if (wingL) wingL.localRotation = Quaternion.Euler(0, 0, flap);
        if (wingR) wingR.localRotation = Quaternion.Euler(0, 0, -flap);

        switch (s)
        {
            case S.Fly:
                {
                    angle += flySpeed * Time.deltaTime * (1f + (maxHealth - health) * 0.15f);
                    Vector3 target = CirclePoint(angle);
                    Vector3 prev = transform.position;
                    transform.position = Vector3.Lerp(transform.position, target, 3f * Time.deltaTime);
                    Vector3 vel = transform.position - prev; vel.y = 0;
                    if (vel.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vel), 5f * Time.deltaTime);

                    strikeTimer -= Time.deltaTime;
                    if (strikeTimer <= 0f)
                    {
                        Vector3 ground = new Vector3(PlayerPos.x, arenaCenter.y, PlayerPos.z);
                        TimedStrike.Spawn(ground, 1.7f, 1.0f, "lightning");
                        if (health <= 2 && frog)
                        {
                            Vector3 lead = new Vector3(frog.Velocity.x, 0, frog.Velocity.z) * 0.9f;
                            TimedStrike.Spawn(ClampToArena(ground + lead + new Vector3(1.5f, 0, 1.5f), 1f), 1.7f, 1.2f, "lightning");
                        }
                        strikes++;
                        strikeTimer = health <= 2 ? 0.9f : 1.25f;
                        if (strikes >= strikesBeforeDive)
                        {
                            strikes = 0;
                            s = S.Dive;
                            stateTimer = 1.2f;
                            moveFrom = transform.position;
                            Vector2 r = Random.insideUnitCircle * 5f;
                            moveTo = ClampToArena(arenaCenter + new Vector3(r.x, 0, r.y), 4f);
                            Sfx.Play("roar", 0.5f);
                        }
                    }
                }
                break;

            case S.Dive:
                {
                    float t = 1f - Mathf.Clamp01(stateTimer / 1.2f);
                    transform.position = Vector3.Lerp(moveFrom, moveTo, t * t);
                    if (t > 0.6f) TouchDamage(2.4f, topHeight);
                    if (stateTimer <= 0f)
                    {
                        s = S.Dizzy;
                        stateTimer = dizzyTime;
                        if (backPoint) backPoint.targetable = true;
                        Sfx.Play("stomp", 0.9f);
                        ThirdPersonCamera.Shake(0.5f);
                        Fx.Puff(transform.position, "Dust", 4f);
                        GameManager.ShowMessage("Oluja je ošamućena! Skoči na nju ili je uhvati jezikom!", 3f);
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
                    Vector3 target = CirclePoint(angle);
                    transform.position = Vector3.Lerp(moveFrom, target, Mathf.SmoothStep(0, 1, t));
                    if (stateTimer <= 0f) { s = S.Fly; strikeTimer = 1.2f; }
                }
                break;
        }
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

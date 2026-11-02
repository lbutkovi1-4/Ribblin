using UnityEngine;

// BOSS 3 – Magmor, golem od lave.
// Baca kamenje lave (bježi iz crvenih krugova). Kad se pregrije, kristali na njemu zasvijetle – uhvati ih jezikom.
public class GolemBoss : BossBase
{
    public GrapplePoint[] crystals;
    public Transform model;
    public Transform throwHand;
    public float throwInterval = 1.6f;
    public int throwsBeforeOverheat = 4;
    public float overheatTime = 5.5f;

    enum S { Throwing, Overheat }
    S s = S.Throwing;
    int throws;
    float touchGrace;

    protected override void Start()
    {
        base.Start();
        if (crystals != null)
        {
            maxHealth = crystals.Length;
            health = maxHealth;
            foreach (var c in crystals)
            {
                if (!c) continue;
                c.targetable = false;
                c.Grappled += OnCrystal;
                SetCrystalLook(c, false);
            }
        }
    }

    protected override void OnActivate()
    {
        s = S.Throwing;
        stateTimer = 1.5f;
    }

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        touchGrace -= Time.deltaTime;

        switch (s)
        {
            case S.Throwing:
                FaceTowards(PlayerPos, 2.5f);
                if (touchGrace <= 0f) TouchDamage(2.6f, 6f);
                if (model) model.localPosition = Vector3.Lerp(model.localPosition, Vector3.zero, 5f * Time.deltaTime);
                if (stateTimer <= 0f)
                {
                    Throw();
                    throws++;
                    stateTimer = throwInterval * (0.6f + 0.4f * health / (float)maxHealth);
                    if (throws >= throwsBeforeOverheat)
                    {
                        throws = 0;
                        s = S.Overheat;
                        stateTimer = overheatTime;
                        SetCrystals(true);
                        Sfx.Play("roar", 0.6f);
                        GameManager.ShowMessage("Magmor is overheating! Grab a glowing crystal with your tongue!", 3f);
                    }
                }
                break;

            case S.Overheat:
                if (model) model.localPosition = Vector3.Lerp(model.localPosition, new Vector3(0, -0.9f, 0), 5f * Time.deltaTime)
                                                 + Random.insideUnitSphere * 0.02f;
                if (stateTimer <= 0f)
                {
                    SetCrystals(false);
                    s = S.Throwing;
                    stateTimer = 1.2f;
                }
                break;
        }
    }

    void Throw()
    {
        Vector3 from = throwHand ? throwHand.position : transform.position + Vector3.up * 6f;
        Vector3 lead = frog ? new Vector3(frog.Velocity.x, 0, frog.Velocity.z) * 0.5f : Vector3.zero;
        Vector3 target = ClampToArena(PlayerPos + lead, 1f);
        LavaRock.Spawn(from, target, 1.1f);
        if (health <= 1)
        {
            Vector2 r = Random.insideUnitCircle * 4f;
            LavaRock.Spawn(from, ClampToArena(PlayerPos + new Vector3(r.x, 0, r.y), 1f), 1.3f);
        }
    }

    void OnCrystal(GrapplePoint c)
    {
        if (s != S.Overheat || dead) return;
        c.gameObject.SetActive(false);
        Fx.Puff(c.transform.position, "CrystalGlow", 2.5f);
        Hit();
        if (frog)
        {
            Vector3 away = PlayerPos - transform.position;
            away.y = 0;
            away = away.sqrMagnitude > 0.01f ? away.normalized : -transform.forward;
            frog.Velocity = away * 11f + Vector3.up * 8f;
        }
        SetCrystals(false);
        touchGrace = 1.2f;
        s = S.Throwing;
        stateTimer = 1.5f;
    }

    void SetCrystals(bool on)
    {
        if (crystals == null) return;
        foreach (var c in crystals)
        {
            if (!c || !c.gameObject.activeSelf) continue;
            c.targetable = on;
            SetCrystalLook(c, on);
        }
    }

    static void SetCrystalLook(GrapplePoint c, bool on)
    {
        foreach (var r in c.GetComponentsInChildren<Renderer>(true))
            r.sharedMaterial = Fx.Mat(on ? "CrystalGlow" : "CrystalDim");
    }
}

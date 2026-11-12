using UnityEngine;

// BOSS 3 – Magmor the Molten, golem od lave.
// Baca kamenje lave (bježi iz crvenih krugova) – što je slabiji, baca više kamenja odjednom.
// Kad se pregrije, kristali na njemu nakratko zasvijetle – uhvati jedan jezikom. Ima 5 kristala.
public class GolemBoss : BossBase
{
    public GrapplePoint[] crystals;
    public Transform model;
    public Transform throwHand;
    public float throwInterval = 1.3f;
    public int throwsBeforeOverheat = 5;
    public float overheatTime = 3.5f;

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
        stateTimer = 1.2f;
    }

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        touchGrace -= Time.deltaTime;

        switch (s)
        {
            case S.Throwing:
                FaceTowards(PlayerPos, 3f);
                if (touchGrace <= 0f) TouchDamage(2.6f, 6f);
                if (model) model.localPosition = Vector3.Lerp(model.localPosition, Vector3.zero, 5f * Time.deltaTime);
                if (stateTimer <= 0f)
                {
                    Throw();
                    throws++;
                    stateTimer = throwInterval * (0.55f + 0.45f * health / (float)maxHealth);
                    if (throws >= throwsBeforeOverheat)
                    {
                        throws = 0;
                        s = S.Overheat;
                        stateTimer = overheatTime;
                        SetCrystals(true);
                        Sfx.Play(roarSound, 0.8f);
                        GameManager.ShowMessage("Magmor is overheating! Grab a glowing crystal with your tongue – quick!", 2.5f);
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
                    stateTimer = 0.8f;
                }
                break;
        }
    }

    void Throw()
    {
        Attack(0.7f);
        Vector3 from = throwHand ? throwHand.position : transform.position + Vector3.up * 6f;
        Vector3 lead = frog ? new Vector3(frog.Velocity.x, 0, frog.Velocity.z) * 0.5f : Vector3.zero;
        LavaRock.Spawn(from, ClampToArena(PlayerPos + lead, 1f), 1.0f);
        int extra = health <= 1 ? 3 : health <= 3 ? 2 : 0;    // slabiji Magmor = više kamenja
        for (int i = 0; i < extra; i++)
        {
            Vector2 r = Random.insideUnitCircle * 5f;
            LavaRock.Spawn(from, ClampToArena(PlayerPos + new Vector3(r.x, 0, r.y), 1f), Random.Range(1.0f, 1.5f));
        }
    }

    void OnCrystal(GrapplePoint c)
    {
        if (s != S.Overheat || dead) return;
        c.gameObject.SetActive(false);
        Fx.Burst(c.transform.position, "CrystalGlow", 14, 1.2f);
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
        stateTimer = 1.2f;
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

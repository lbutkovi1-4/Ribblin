using UnityEngine;

// BOSS 1 – Kapetan Klješto (rak).
// Zalijeće se prema žabici. Ako udari u kameni stup, ošamuti se – tad mu skoči na glavu.
public class CrabBoss : BossBase
{
    public Vector3[] pillars;
    public float pillarRadius = 1.3f;
    public float chargeSpeed = 14f;
    public float topHeight = 2.2f;
    public Transform model;
    public GameObject stunStars;

    enum S { Idle, Windup, Charge, Stunned, Recover }
    S s = S.Idle;
    Vector3 chargeDir;
    float hardShellHint;

    protected override void OnActivate()
    {
        s = S.Idle;
        stateTimer = 1.5f;
        if (stunStars) stunStars.SetActive(false);
    }

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        hardShellHint -= Time.deltaTime;
        Vector3 pos = transform.position;
        float top = pos.y + topHeight;

        if (s != S.Stunned && IsStomped(2.4f, top))
        {
            frog.Bounce(3.5f);
            Sfx.Play("stomp", 0.5f);
            if (hardShellHint <= 0f)
            {
                GameManager.ShowMessage("His shell is too hard! Make him crash into a stone pillar.", 3f);
                hardShellHint = 4f;
            }
            return;
        }

        switch (s)
        {
            case S.Idle:
                FaceTowards(PlayerPos, 6f);
                TouchDamage(2.4f, topHeight - 0.7f);
                if (stateTimer <= 0f) { s = S.Windup; stateTimer = 0.8f; Sfx.Play("roar", 0.3f); }
                break;

            case S.Windup:
                FaceTowards(PlayerPos, 8f);
                if (model) model.localPosition = Random.insideUnitSphere * 0.08f;
                TouchDamage(2.4f, topHeight - 0.7f);
                if (stateTimer <= 0f)
                {
                    if (model) model.localPosition = Vector3.zero;
                    chargeDir = PlayerPos - pos;
                    chargeDir.y = 0;
                    if (chargeDir.sqrMagnitude < 0.01f) chargeDir = transform.forward;
                    chargeDir.Normalize();
                    s = S.Charge;
                    stateTimer = 3f;
                }
                break;

            case S.Charge:
                transform.position += chargeDir * chargeSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(chargeDir);
                TouchDamage(2.4f, topHeight - 0.7f);
                if (pillars != null)
                {
                    foreach (var p in pillars)
                    {
                        if (Flat(transform.position, p) < pillarRadius + 2.0f)
                        {
                            s = S.Stunned;
                            stateTimer = 3.5f;
                            transform.position -= chargeDir * 0.8f;
                            Sfx.Play("bosshit", 0.6f);
                            ThirdPersonCamera.Shake(0.5f);
                            Fx.Puff(p + Vector3.up * 2f, "Dust", 3f);
                            if (stunStars) stunStars.SetActive(true);
                            GameManager.ShowMessage("He is dizzy! Jump on his head!", 2.5f);
                            return;
                        }
                    }
                }
                if (Flat(transform.position, arenaCenter) > arenaRadius - 2.2f || stateTimer <= 0f)
                {
                    transform.position = ClampToArena(transform.position, 2.2f);
                    s = S.Recover;
                    stateTimer = 0.8f;
                }
                break;

            case S.Stunned:
                if (stunStars) stunStars.transform.Rotate(0, 220f * Time.deltaTime, 0);
                if (IsStomped(2.6f, top))
                {
                    frog.Bounce(5f);
                    if (stunStars) stunStars.SetActive(false);
                    Hit();
                    s = S.Recover;
                    stateTimer = 1.2f;
                }
                else if (stateTimer <= 0f)
                {
                    if (stunStars) stunStars.SetActive(false);
                    s = S.Idle;
                    stateTimer = 0.6f;
                }
                break;

            case S.Recover:
                TouchDamage(2.4f, topHeight - 0.7f);
                if (stateTimer <= 0f)
                {
                    s = S.Idle;
                    stateTimer = Mathf.Lerp(0.5f, 1.4f, health / (float)maxHealth);
                }
                break;
        }
    }

    protected override void OnHit() { chargeSpeed += 3f; }
}

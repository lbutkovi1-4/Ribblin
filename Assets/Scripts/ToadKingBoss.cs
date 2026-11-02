using UnityEngine;

// BOSS 2 – Kralj Krastača.
// Skače prema žabici i pri doskoku šalje udarne valove (preskoči ih). Nakon nekoliko skokova se umori – skoči mu na glavu.
public class ToadKingBoss : BossBase
{
    public float topHeight = 3.4f;
    public Transform model;
    public GameObject tiredFx;
    public float jumpDuration = 1.1f;
    public float jumpHeight = 7f;
    public int jumpsBeforeTired = 3;
    public float waveSpeed = 8f;

    enum S { Idle, Jump, Land, Tired }
    S s = S.Idle;
    Vector3 jumpFrom, jumpTo;
    int jumpsDone;
    float hintCooldown;

    protected override void OnActivate()
    {
        s = S.Idle;
        stateTimer = 1.2f;
        if (tiredFx) tiredFx.SetActive(false);
    }

    protected override void Tick()
    {
        stateTimer -= Time.deltaTime;
        hintCooldown -= Time.deltaTime;
        Vector3 pos = transform.position;
        float top = pos.y + topHeight;

        if (s != S.Tired && s != S.Jump && IsStomped(2.8f, top))
        {
            frog.Bounce(3.5f);
            if (hintCooldown <= 0f) { GameManager.ShowMessage("Kralj je prejak dok skače! Pričekaj da se umori.", 3f); hintCooldown = 4f; }
            return;
        }

        switch (s)
        {
            case S.Idle:
                FaceTowards(PlayerPos, 5f);
                TouchDamage(2.6f, topHeight - 0.8f);
                if (model) model.localScale = new Vector3(1f + Mathf.Sin(Time.time * 6f) * 0.04f, 1f - Mathf.Sin(Time.time * 6f) * 0.04f, 1f);
                if (stateTimer <= 0f) StartJump();
                break;

            case S.Jump:
                {
                    float t = 1f - Mathf.Clamp01(stateTimer / jumpDuration);
                    transform.position = Vector3.Lerp(jumpFrom, jumpTo, t) + Vector3.up * Mathf.Sin(Mathf.PI * t) * jumpHeight;
                    FaceTowards(jumpTo, 6f);
                    if (stateTimer <= 0f) Land();
                }
                break;

            case S.Land:
                TouchDamage(2.6f, topHeight - 0.8f);
                if (model) model.localScale = Vector3.Lerp(model.localScale, Vector3.one, 8f * Time.deltaTime);
                if (stateTimer <= 0f)
                {
                    if (jumpsDone >= jumpsBeforeTired)
                    {
                        s = S.Tired;
                        stateTimer = 4.5f;
                        if (tiredFx) tiredFx.SetActive(true);
                        GameManager.ShowMessage("Kralj se umorio! Skoči mu na glavu!", 2.5f);
                    }
                    else { s = S.Idle; stateTimer = 0.4f + 0.2f * health; }
                }
                break;

            case S.Tired:
                if (model) model.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 3f) * 6f, 0, Mathf.Sin(Time.time * 2f) * 8f);
                if (IsStomped(3f, top))
                {
                    frog.Bounce(5.5f);
                    if (tiredFx) tiredFx.SetActive(false);
                    if (model) model.localRotation = Quaternion.identity;
                    jumpsDone = 0;
                    Hit();
                    s = S.Idle;
                    stateTimer = 0.8f;
                }
                else if (stateTimer <= 0f)
                {
                    if (tiredFx) tiredFx.SetActive(false);
                    if (model) model.localRotation = Quaternion.identity;
                    jumpsDone = 0;
                    s = S.Idle;
                    stateTimer = 0.3f;
                }
                break;
        }
    }

    void StartJump()
    {
        jumpFrom = transform.position;
        Vector2 r = Random.insideUnitCircle * 1.5f;
        jumpTo = ClampToArena(PlayerPos + new Vector3(r.x, 0, r.y), 3f);
        s = S.Jump;
        stateTimer = jumpDuration;
        if (model) model.localScale = new Vector3(0.85f, 1.2f, 0.85f);
        Sfx.Play("boing", 0.5f);
    }

    void Land()
    {
        transform.position = jumpTo;
        s = S.Land;
        stateTimer = 0.5f;
        jumpsDone++;
        if (model) model.localScale = new Vector3(1.25f, 0.75f, 1.25f);
        Sfx.Play("stomp", 0.9f);
        ThirdPersonCamera.Shake(0.6f);
        Fx.Puff(transform.position, "Dust", 4f);
        float spd = waveSpeed + (maxHealth - health) * 1.5f;
        Shockwave.Spawn(transform.position + Vector3.up * 0.05f, spd, arenaRadius + 2f);
        if (health <= 1) Invoke(nameof(SecondWave), 0.5f);
        if (frog && Flat(PlayerPos, transform.position) < 3f && PlayerPos.y < transform.position.y + 2f)
            PlayerHealth.Instance?.Damage(1, transform.position);
    }

    void SecondWave()
    {
        if (dead) return;
        Shockwave.Spawn(transform.position + Vector3.up * 0.05f, waveSpeed + 2f, arenaRadius + 2f);
    }
}

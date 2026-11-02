using UnityEngine;

// Zajednička logika svih bossova: zdravlje, aktivacija u areni, traka zdravlja, pobjeda i portal.
public abstract class BossBase : MonoBehaviour
{
    public static BossBase Current { get; private set; }

    public string bossName = "Boss";
    public int maxHealth = 3;
    public Vector3 arenaCenter;
    public float arenaRadius = 14f;
    public GameObject portal;
    [TextArea] public string hint = "";

    protected int health;
    protected FrogController frog;
    protected bool active;
    protected bool dead;
    protected float stateTimer;

    float flash;
    float invuln;
    float deathT;
    Renderer[] renderers;
    Vector3 baseScale;

    protected Vector3 PlayerPos => frog ? frog.transform.position : transform.position;

    protected virtual void Start()
    {
        Current = this;
        health = maxHealth;
        frog = FrogController.Instance;
        renderers = System.Array.FindAll(GetComponentsInChildren<Renderer>(true), r => !(r is LineRenderer));
        baseScale = transform.localScale;
        if (portal) portal.SetActive(false);
    }

    protected virtual void Update()
    {
        if (GameManager.Paused) return;
        if (!frog) frog = FrogController.Instance;

        if (dead)
        {
            deathT += Time.deltaTime;
            transform.localScale = baseScale * Mathf.Max(0.01f, 1f - deathT * 0.7f);
            transform.Rotate(0, 360f * Time.deltaTime, 0);
            if (deathT > 1.4f) gameObject.SetActive(false);
            return;
        }

        if (!active)
        {
            if (frog && Flat(frog.transform.position, arenaCenter) < arenaRadius - 1f && frog.IsGrounded &&
                Mathf.Abs(frog.transform.position.y - arenaCenter.y) < 3f)
            {
                active = true;
                Sfx.Play("roar", 0.7f);
                ThirdPersonCamera.Shake(0.4f);
                GameManager.ShowMessage(hint, 6f);
                OnActivate();
            }
            else return;
        }

        invuln -= Time.deltaTime;
        if (flash > 0f)
        {
            flash -= Time.deltaTime;
            bool visible = flash <= 0f || Mathf.Repeat(flash, 0.12f) > 0.06f;
            SetVisible(visible);
        }

        Tick();
    }

    protected abstract void Tick();
    protected virtual void OnActivate() { }
    protected virtual void OnHit() { }

    public void Hit()
    {
        if (dead || invuln > 0f || !active) return;
        health--;
        invuln = 0.6f;
        flash = 0.6f;
        Sfx.Play("bosshit", 0.8f);
        ThirdPersonCamera.Shake(0.5f);
        Fx.Puff(transform.position + Vector3.up * 2f, "Dust", 3f);
        if (health <= 0) Die(); else OnHit();
    }

    public void DebugHit()
    {
        active = true;
        invuln = 0f;
        Hit();
    }

    void Die()
    {
        dead = true;
        SetVisible(true);
        Sfx.Play("win", 0.7f);
        GameManager.ShowMessage(bossName + " is defeated! Enter the portal.", 6f);
        if (portal) portal.SetActive(true);
        GameStats.BossesDefeated++;
    }

    void SetVisible(bool v)
    {
        foreach (var r in renderers) if (r) r.enabled = v;
    }

    protected static float Flat(Vector3 a, Vector3 b) { a.y = 0; b.y = 0; return Vector3.Distance(a, b); }

    protected void TouchDamage(float radius, float height)
    {
        if (!frog) return;
        Vector3 p = frog.transform.position;
        if (Flat(p, transform.position) < radius && p.y < transform.position.y + height && p.y > transform.position.y - 1f)
            PlayerHealth.Instance?.Damage(1, transform.position);
    }

    protected bool IsStomped(float radius, float topY)
    {
        if (!frog) return false;
        Vector3 p = frog.transform.position;
        return Flat(p, transform.position) < radius && p.y > topY - 1.0f && p.y < topY + 1.5f && frog.Velocity.y < 0.5f && !frog.IsGrounded;
    }

    protected void FaceTowards(Vector3 p, float speed)
    {
        Vector3 d = p - transform.position;
        d.y = 0;
        if (d.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(d), speed * Time.deltaTime);
    }

    protected Vector3 ClampToArena(Vector3 p, float margin)
    {
        Vector3 off = p - arenaCenter;
        off.y = 0;
        float max = arenaRadius - margin;
        if (off.magnitude > max) off = off.normalized * max;
        return new Vector3(arenaCenter.x + off.x, arenaCenter.y, arenaCenter.z + off.z);
    }

    void OnGUI()
    {
        if (!active || dead) return;
        float s = Ui.S;
        float w = 700f * s, h = 30f * s;
        float x = Screen.width / 2f - w / 2f, y = 40f * s;
        Ui.Text(new Rect(0, y - 4 * s, Screen.width, 44 * s), bossName, Ui.Label(36, TextAnchor.UpperCenter));
        y += 46f * s;
        Ui.Box(new Rect(x - 4 * s, y - 4 * s, w + 8 * s, h + 8 * s), new Color(0, 0, 0, 0.6f));
        Ui.Box(new Rect(x, y, w * Mathf.Clamp01(health / (float)maxHealth), h), new Color(0.95f, 0.25f, 0.3f));
    }
}

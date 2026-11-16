using UnityEngine;

// Srca žabice. Udarac = -1 srce i odbacivanje. Bez srca = GAME OVER i razina kreće ispočetka.
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public int maxHearts = 3;
    public float invulnTime = 1.2f;
    public int Hearts { get; private set; }
    public static bool God;   // samo za testiranje u editoru (F6) – žabica ne gubi srca

    FrogController frog;
    Renderer[] renderers;
    float invuln;

    void Awake()
    {
        Instance = this;
        Hearts = maxHearts;
        frog = GetComponent<FrogController>();
    }

    void Start()
    {
        invuln = 2f;   // kratka zaštita na početku levela
        renderers = System.Array.FindAll(GetComponentsInChildren<Renderer>(), r => !(r is LineRenderer));
    }

    void Update()
    {
        if (invuln > 0f && !GameManager.IsGameOver)
        {
            invuln -= Time.deltaTime;
            bool visible = invuln <= 0f || Mathf.Repeat(invuln, 0.2f) > 0.1f;
            foreach (var r in renderers) if (r) r.enabled = visible;
        }
    }

    public bool Damage(int amount, Vector3 source)
    {
        if (invuln > 0f || GameManager.LevelDone || GameManager.IsGameOver || God) return false;
        Hearts -= amount;
        Sfx.Play("hurt", 0.6f);
        ThirdPersonCamera.Shake(0.3f);
        if (Hearts <= 0) { Die(); return true; }
        invuln = invulnTime;
        frog.Knockback(source, 9f);
        return true;
    }

    public void FellOff()
    {
        if (GameManager.IsGameOver) return;
        Sfx.Play("splash", 0.6f);
        Hearts--;
        if (Hearts <= 0) { Die(); return; }
        frog.Respawn();
        invuln = invulnTime;
    }

    public void Heal(int amount) => Hearts = Mathf.Min(maxHearts, Hearts + amount);

    void Die()
    {
        Hearts = 0;
        GameStats.Deaths++;
        // žabica "pukne" u oblak komadića i nestane, a GameManager prikaže GAME OVER
        Fx.Burst(transform.position + Vector3.up * 0.5f, "SlimeGreen", 16, 1.2f);
        Fx.Puff(transform.position + Vector3.up * 0.5f, "Dust", 2f);
        foreach (var r in renderers) if (r) r.enabled = false;
        invuln = 0f;
        GameManager.GameOver();
    }
}

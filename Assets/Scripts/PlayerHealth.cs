using UnityEngine;

// Srca žabice. Udarac = -1 srce i odbacivanje. Bez srca = povratak na checkpoint s punim srcima.
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    public int maxHearts = 3;
    public float invulnTime = 1.2f;
    public int Hearts { get; private set; }

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
        renderers = System.Array.FindAll(GetComponentsInChildren<Renderer>(), r => !(r is LineRenderer));
    }

    void Update()
    {
        if (invuln > 0f)
        {
            invuln -= Time.deltaTime;
            bool visible = invuln <= 0f || Mathf.Repeat(invuln, 0.2f) > 0.1f;
            foreach (var r in renderers) if (r) r.enabled = visible;
        }
    }

    public bool Damage(int amount, Vector3 source)
    {
        if (invuln > 0f || GameManager.LevelDone) return false;
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
        Sfx.Play("splash", 0.6f);
        Hearts--;
        if (Hearts <= 0) { Die(); return; }
        frog.Respawn();
        invuln = invulnTime;
    }

    public void Heal(int amount) => Hearts = Mathf.Min(maxHearts, Hearts + amount);

    void Die()
    {
        GameStats.Deaths++;
        GameManager.ShowMessage("Ouch! Back to the last checkpoint.", 2.5f);
        Hearts = maxHearts;
        frog.Respawn();
        invuln = invulnTime;
    }
}

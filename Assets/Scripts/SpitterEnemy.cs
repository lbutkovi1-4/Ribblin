using UnityEngine;

// Biljka/top koji stoji na mjestu i pljuje projektile prema žabici.
// Pobijedi ga skokom na glavu ili jezikom.
public class SpitterEnemy : Enemy
{
    public float range = 14f;
    public float interval = 2.2f;
    public float projectileSpeed = 9f;
    public float height = 1.6f;
    public string projectileMat = "SlimeGreen";
    public Transform head;
    float timer = 1.5f;
    Vector3 headScale = Vector3.one;

    public override Vector3 Center => transform.position + Vector3.up * height * 0.7f;

    void Start() { if (head) headScale = head.localScale; }

    void Update()
    {
        if (dead || GameManager.Paused) return;
        var frog = FrogController.Instance;
        if (!frog) return;
        Vector3 target = frog.transform.position + Vector3.up * 0.6f;
        Vector3 from = Center;
        Vector3 to = target - from;
        if (head) head.localScale = Vector3.Lerp(head.localScale, headScale, 8f * Time.deltaTime);

        if (to.magnitude < range)
        {
            Vector3 flat = to; flat.y = 0;
            if (flat.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flat), 6f * Time.deltaTime);
            timer -= Time.deltaTime;
            if (timer <= 0f && !Physics.Raycast(from, to.normalized, to.magnitude - 0.8f, ~0, QueryTriggerInteraction.Ignore))
            {
                Projectile.Spawn(from + to.normalized * 0.8f, to.normalized * projectileSpeed, projectileMat, 0.5f);
                Sfx.Play("spit", 0.5f);
                if (head) head.localScale = headScale * 1.3f;
                timer = interval;
            }
        }
        HandleContact(0.8f, transform.position.y, transform.position.y + height);
    }
}

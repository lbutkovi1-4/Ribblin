using System.Collections.Generic;
using UnityEngine;

// Sluzavi neprijatelj. Skoči mu na glavu ili ga pojedi jezikom. Dodir sa strane = -1 srce.
// Bodljikava verzija (spiky) se ne može zgaziti – nju pojedi jezikom.
public class Slime : Enemy
{
    public float radius = 0.7f;
    public float height = 1.0f;
    public float speed = 2.2f;
    public float wanderRadius = 4f;
    public float chaseRange = 7f;
    public bool chase = true;
    public bool lunge = false;          // od 3. levela: skoči prema žabici kad je blizu
    public float lungeRange = 4f;
    public float lungeSpeed = 8f;
    public Transform model;

    Vector3 home;
    Vector3 target;
    float retarget;
    float deadTimer;
    Vector3 baseScale = Vector3.one;
    float seed;
    float lungeCooldown = 1.5f;
    float lungeTime;
    Vector3 lungeDir;

    public override Vector3 Center => transform.position + Vector3.up * height * 0.5f;

    void Start()
    {
        home = transform.position;
        if (model) baseScale = model.localScale;
        seed = Random.value * 10f;
        PickTarget();
    }

    void PickTarget()
    {
        Vector2 r = Random.insideUnitCircle * wanderRadius;
        target = home + new Vector3(r.x, 0, r.y);
        retarget = Random.Range(1.5f, 3.5f);
    }

    void Update()
    {
        if (dead)
        {
            deadTimer += Time.deltaTime * 4f;
            if (model) model.localScale = new Vector3(baseScale.x * (1 + deadTimer), baseScale.y * Mathf.Max(0.05f, 1 - deadTimer), baseScale.z * (1 + deadTimer));
            if (deadTimer >= 1f) Destroy(gameObject);
            return;
        }
        if (GameManager.Paused) return;

        var frog = FrogController.Instance;

        // skok prema žabici
        lungeCooldown -= Time.deltaTime;
        if (lungeTime > 0f)
        {
            lungeTime -= Time.deltaTime;
            Vector3 np = transform.position + lungeDir * lungeSpeed * Time.deltaTime;
            Vector3 off2 = np - home; off2.y = 0;
            if (off2.magnitude <= wanderRadius + 1f) transform.position = np;
            if (model) model.localPosition = Vector3.up * Mathf.Sin((1f - lungeTime / 0.4f) * Mathf.PI) * 1.2f;
            CheckPlayer(frog);
            return;
        }
        if (lunge && frog && lungeCooldown <= 0f)
        {
            Vector3 d = frog.transform.position - transform.position;
            if (Mathf.Abs(d.y) < 1.5f && new Vector2(d.x, d.z).magnitude < lungeRange)
            {
                d.y = 0;
                lungeDir = d.normalized;
                lungeTime = 0.4f;
                lungeCooldown = Random.Range(1.6f, 2.6f);
                transform.rotation = Quaternion.LookRotation(lungeDir);
                return;
            }
        }

        Vector3 goal = target;
        float spd = speed;
        if (chase && frog)
        {
            Vector3 fp = frog.transform.position;
            if (Flat(fp, transform.position) < chaseRange && Mathf.Abs(fp.y - home.y) < 2.5f && Flat(fp, home) < wanderRadius + 2f)
            {
                goal = fp;
                spd = speed * 1.4f;
            }
        }
        Vector3 off = goal - home; off.y = 0;
        if (off.magnitude > wanderRadius) off = off.normalized * wanderRadius;
        goal = home + off;

        Vector3 to = goal - transform.position; to.y = 0;
        if (to.magnitude > 0.2f)
        {
            Vector3 step = to.normalized * spd * Time.deltaTime;
            transform.position += step;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), 8f * Time.deltaTime);
        }
        retarget -= Time.deltaTime;
        if (retarget <= 0f) PickTarget();

        // poskakivanje
        if (model)
        {
            float hop = Mathf.Abs(Mathf.Sin(Time.time * 6f + seed));
            model.localPosition = Vector3.up * hop * 0.25f;
            model.localScale = new Vector3(baseScale.x * (1.1f - hop * 0.15f), baseScale.y * (0.85f + hop * 0.25f), baseScale.z * (1.1f - hop * 0.15f));
        }

        CheckPlayer(frog);
    }

    void CheckPlayer(FrogController frog)
    {
        HandleContact(radius, transform.position.y, transform.position.y + height);
    }

    protected override void OnKilled() { }   // umjesto brisanja: animacija spljoštavanja u Update

}

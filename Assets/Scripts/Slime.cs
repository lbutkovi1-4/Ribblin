using System.Collections.Generic;
using UnityEngine;

// Sluzavi neprijatelj. Skoči mu na glavu ili ga pojedi jezikom. Dodir sa strane = -1 srce.
public class Slime : MonoBehaviour
{
    public static readonly List<Slime> All = new List<Slime>();

    public float radius = 0.7f;
    public float height = 1.0f;
    public float speed = 2.2f;
    public float wanderRadius = 4f;
    public float chaseRange = 7f;
    public bool chase = true;
    public Transform model;

    Vector3 home;
    Vector3 target;
    float retarget;
    bool dead;
    float deadTimer;
    Vector3 baseScale = Vector3.one;
    float seed;

    public bool Alive => !dead;
    public Vector3 Center => transform.position + Vector3.up * height * 0.5f;

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }

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

        if (frog)
        {
            Vector3 fp = frog.transform.position;
            float fd = Flat(fp, transform.position);
            float top = transform.position.y + height;
            if (fd < radius + 0.5f && fp.y > top - 0.5f && fp.y < top + 1.0f && frog.Velocity.y < 0.5f && !frog.IsGrounded)
            {
                frog.Bounce(2.8f);
                Sfx.Play("stomp", 0.6f);
                Die();
            }
            else if (fd < radius + 0.4f && fp.y < top - 0.4f && fp.y > transform.position.y - 1f)
            {
                PlayerHealth.Instance?.Damage(1, transform.position);
            }
        }
    }

    public void Eat()
    {
        Sfx.Play("gulp", 0.6f);
        Die();
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        All.Remove(this);
        Fx.Puff(Center, "Dust", 1.4f);
    }

    static float Flat(Vector3 a, Vector3 b) { a.y = 0; b.y = 0; return Vector3.Distance(a, b); }
}

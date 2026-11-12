using UnityEngine;

// Projektil (sjemenka, kuglica lave, pero...). Leti ravno; pogodak = -1 srce.
public class Projectile : MonoBehaviour
{
    public Vector3 velocity;
    public float radius = 0.35f;
    public float life = 4f;
    float t;

    public static Projectile Spawn(Vector3 pos, Vector3 velocity, string mat, float size = 0.6f, PrimitiveType shape = PrimitiveType.Sphere)
    {
        var go = Fx.Prim(shape, pos, Vector3.one * size, mat);
        var p = go.AddComponent<Projectile>();
        p.velocity = velocity;
        p.radius = size * 0.5f;
        if (velocity.sqrMagnitude > 0.01f) go.transform.rotation = Quaternion.LookRotation(velocity);
        return p;
    }

    void Update()
    {
        if (GameManager.Paused) return;
        t += Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        if (t > life) { Destroy(gameObject); return; }

        var frog = FrogController.Instance;
        if (frog && Vector3.Distance(frog.transform.position + Vector3.up * 0.6f, transform.position) < radius + 0.55f)
        {
            PlayerHealth.Instance?.Damage(1, transform.position - velocity.normalized);
            Fx.Puff(transform.position, "Dust", 1f);
            Destroy(gameObject);
            return;
        }
        if (t > 0.15f && Physics.CheckSphere(transform.position, radius * 0.6f, ~0, QueryTriggerInteraction.Ignore))
        {
            Fx.Puff(transform.position, "Dust", 0.8f);
            Destroy(gameObject);
        }
    }
}

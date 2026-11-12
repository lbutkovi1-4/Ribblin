using UnityEngine;

// Komadić koji odleti, vrti se, pada i nestane (efekt uništenja).
public class Debris : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 spin;
    public float gravity = 18f;
    public float life = 0.9f;
    float t;
    Vector3 startScale;

    void Start() { startScale = transform.localScale; }

    void Update()
    {
        t += Time.deltaTime;
        velocity += Vector3.down * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.Rotate(spin * Time.deltaTime);
        transform.localScale = startScale * Mathf.Clamp01(1f - t / life);
        if (t >= life) Destroy(gameObject);
    }
}

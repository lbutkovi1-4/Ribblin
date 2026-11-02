using UnityEngine;

// Ptica koja kruži po nebu i maše krilima (ukras).
public class FlyingBird : MonoBehaviour
{
    public Vector3 center;
    public float radius = 20f;
    public float height = 15f;
    public float speed = 0.3f;
    public Transform wingL, wingR;
    float angle;
    float seed;

    void Start()
    {
        seed = Random.value * 10f;
        angle = Random.value * Mathf.PI * 2f;
    }

    void Update()
    {
        angle += speed * Time.deltaTime;
        Vector3 p = center + new Vector3(Mathf.Cos(angle) * radius, height + Mathf.Sin(Time.time * 0.7f + seed) * 1.5f, Mathf.Sin(angle) * radius);
        Vector3 vel = p - transform.position;
        transform.position = p;
        if (vel.sqrMagnitude > 0.00001f) transform.rotation = Quaternion.LookRotation(vel);
        float flap = Mathf.Sin(Time.time * 8f + seed) * 35f;
        if (wingL) wingL.localRotation = Quaternion.Euler(0, 0, flap);
        if (wingR) wingR.localRotation = Quaternion.Euler(0, 0, -flap);
    }
}

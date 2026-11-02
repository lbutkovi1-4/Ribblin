using UnityEngine;

// Ukras koji lagano lebdi gore-dolje.
public class Bobber : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float speed = 1f;
    Vector3 start;
    float seed;

    void Start() { start = transform.localPosition; seed = Random.value * 10f; }
    void Update() { transform.localPosition = start + Vector3.up * Mathf.Sin(Time.time * speed + seed) * amplitude; }
}

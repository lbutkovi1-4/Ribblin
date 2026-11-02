using UnityEngine;

// Platforma koja se miče naprijed-natrag i nosi žabicu.
[DefaultExecutionOrder(-50)]
public class MovingPlatform : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 6);
    public float period = 4f;
    public float phase = 0f;
    public Vector3 Delta { get; private set; }

    Vector3 start;

    void Start()
    {
        start = transform.position;
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        if (GameManager.Paused) { Delta = Vector3.zero; return; }
        float t = (Mathf.Sin((Time.time / period + phase) * Mathf.PI * 2f) + 1f) * 0.5f;
        Vector3 p = start + offset * t;
        Delta = p - transform.position;
        transform.position = p;
    }
}

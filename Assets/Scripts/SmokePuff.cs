using UnityEngine;

// Oblak dima koji se diže iz kratera vulkana, raste i nestaje, pa ponovno.
public class SmokePuff : MonoBehaviour
{
    public float riseSpeed = 3f;
    public float maxHeight = 30f;
    public float startSize = 4f;
    public float endSize = 14f;
    Vector3 start;
    float h;

    void Start()
    {
        start = transform.position;
        h = Random.value * maxHeight;
    }

    void Update()
    {
        h += riseSpeed * Time.deltaTime;
        if (h > maxHeight) h = 0f;
        float k = h / maxHeight;
        transform.position = start + new Vector3(k * 6f, h, k * 3f);
        transform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, k) * (k < 0.1f ? k * 10f : 1f) * (k > 0.85f ? (1f - k) / 0.15f : 1f);
    }
}

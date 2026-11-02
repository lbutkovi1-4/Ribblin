using UnityEngine;

// Plamen koji treperi (ukras za vulkan).
public class Flame : MonoBehaviour
{
    public Transform[] parts;
    Vector3[] baseScale;
    Vector3[] basePos;
    float seed;

    void Start()
    {
        seed = Random.value * 100f;
        baseScale = new Vector3[parts.Length];
        basePos = new Vector3[parts.Length];
        for (int i = 0; i < parts.Length; i++) { baseScale[i] = parts[i].localScale; basePos[i] = parts[i].localPosition; }
    }

    void Update()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            float n = Mathf.PerlinNoise(seed + i * 3.1f, Time.time * 4f);
            float k = 0.75f + n * 0.5f;
            parts[i].localScale = new Vector3(baseScale[i].x * (1.1f - n * 0.2f), baseScale[i].y * k, baseScale[i].z * (1.1f - n * 0.2f));
            parts[i].localPosition = basePos[i] + new Vector3(Mathf.Sin(Time.time * 7f + i + seed) * 0.05f, (k - 1f) * baseScale[i].y * 0.4f, 0);
        }
    }
}

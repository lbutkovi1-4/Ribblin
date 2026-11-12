using UnityEngine;

// Iskra koja se polako diže iz lave, nestane i krene ispočetka (ukras).
public class Ember : MonoBehaviour
{
    public float riseSpeed = 1.2f;
    public float maxHeight = 10f;
    Vector3 start;
    float h;
    float seed;

    void Start()
    {
        start = transform.position;
        seed = Random.value * 10f;
        h = Random.value * maxHeight;
    }

    void Update()
    {
        h += riseSpeed * Time.deltaTime;
        if (h > maxHeight) h = 0f;
        transform.position = start + new Vector3(Mathf.Sin(Time.time + seed) * 0.6f, h, Mathf.Cos(Time.time * 0.8f + seed) * 0.6f);
        float k = 1f - h / maxHeight;
        transform.localScale = Vector3.one * 0.18f * k;
    }
}

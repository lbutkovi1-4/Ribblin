using UnityEngine;

// Kratki "puf" oblačić koji naraste i nestane.
public class Puff : MonoBehaviour
{
    public float size = 1f;
    float t;

    void Update()
    {
        t += Time.deltaTime * 3f;
        float s = size * Mathf.Lerp(0.3f, 1.2f, t) * (1f - t * 0.7f);
        transform.localScale = Vector3.one * Mathf.Max(0.01f, s);
        if (t >= 1f) Destroy(gameObject);
    }
}

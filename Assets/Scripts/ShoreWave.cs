using UnityEngine;

// Bijela pjena vala: dolazi s mora prema obali, uspori se, razlije i nestane – pa opet.
public class ShoreWave : MonoBehaviour
{
    public Vector3 shorePoint;     // gdje val udari u obalu
    public Vector3 outward;        // smjer prema moru
    public float distance = 9f;    // odakle val kreće
    public float period = 5f;
    public float phase;
    Vector3 baseScale;

    void Start() { baseScale = transform.localScale; }

    void Update()
    {
        float k = Mathf.Repeat(Time.time / period + phase, 1f);
        float travel = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(k / 0.75f));   // 1 = daleko, 0 = na obali
        transform.position = shorePoint + outward * (distance * travel);
        // val raste dok dolazi, a na obali se razlije i nestane
        float grow = Mathf.Clamp01(k / 0.3f);
        float fade = k < 0.75f ? 1f : 1f - (k - 0.75f) / 0.25f;
        transform.localScale = new Vector3(baseScale.x * (0.7f + 0.3f * grow), baseScale.y * fade, baseScale.z * (0.6f + (k > 0.75f ? (k - 0.75f) * 6f : 0f)) * Mathf.Max(0.05f, fade));
    }
}

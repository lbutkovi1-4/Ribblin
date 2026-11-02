using UnityEngine;

// Opasno područje (trnje, vrtuljak...). Oblik = kocka ovog objekta (pozicija, rotacija, veličina).
public class DamageZone : MonoBehaviour
{
    public float margin = 0.35f;
    public bool zoneActive = true;

    void Update()
    {
        if (!zoneActive || GameManager.Paused) return;
        var frog = FrogController.Instance;
        if (!frog) return;
        Vector3 p = frog.transform.position + Vector3.up * 0.6f;
        Vector3 s = transform.lossyScale;
        Vector3 w = Vector3.Scale(transform.InverseTransformPoint(p), s);
        Vector3 half = s * 0.5f;
        if (Mathf.Abs(w.x) < half.x + margin && Mathf.Abs(w.y) < half.y + margin + 0.3f && Mathf.Abs(w.z) < half.z + margin)
            PlayerHealth.Instance?.Damage(1, transform.position);
    }
}

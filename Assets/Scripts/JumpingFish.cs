using UnityEngine;

// Riba koja povremeno iskoči iz vode u luku i vrati se natrag (ukras).
public class JumpingFish : MonoBehaviour
{
    public float waterY = -0.3f;
    public float jumpLength = 4.5f;
    public float jumpHeight = 3.2f;
    public float jumpTime = 1.3f;
    public Transform model;

    Vector3 home;
    float wait;
    float t = -1f;
    Vector3 from, to;

    void Start()
    {
        home = transform.position;
        wait = Random.Range(0.3f, 3f);
        Hide();
    }

    void Hide()
    {
        transform.position = new Vector3(home.x, waterY - 2f, home.z);
    }

    void Update()
    {
        if (t < 0f)
        {
            wait -= Time.deltaTime;
            if (wait > 0f) return;
            float a = Random.Range(0f, Mathf.PI * 2f);
            Vector3 dir = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
            from = home - dir * jumpLength * 0.5f; from.y = waterY - 0.3f;
            to = home + dir * jumpLength * 0.5f; to.y = waterY - 0.3f;
            t = 0f;
        }

        t += Time.deltaTime / jumpTime;
        float k = Mathf.Clamp01(t);
        Vector3 p = Vector3.Lerp(from, to, k) + Vector3.up * Mathf.Sin(k * Mathf.PI) * jumpHeight;
        Vector3 vel = (to - from) / jumpTime + Vector3.up * Mathf.Cos(k * Mathf.PI) * jumpHeight * Mathf.PI / jumpTime;
        transform.position = p;
        if (vel.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(vel);
        if (model) model.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 20f) * 15f, 0);

        if (t >= 1f)
        {
            t = -1f;
            wait = Random.Range(1.2f, 3.5f);
            Hide();
        }
    }
}

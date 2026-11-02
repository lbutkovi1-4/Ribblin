using UnityEngine;

// Udarni val koji se širi od bossa. Preskoči ga!
public class Shockwave : MonoBehaviour
{
    public float speed = 9f;
    public float maxRadius = 16f;
    public float band = 0.7f;
    float radius = 0.5f;
    float groundY;
    LineRenderer lr;
    bool hit;
    const int Segments = 56;

    public static void Spawn(Vector3 pos, float speed, float maxRadius)
    {
        var go = new GameObject("Shockwave");
        go.transform.position = pos;
        var s = go.AddComponent<Shockwave>();
        s.speed = speed;
        s.maxRadius = maxRadius;
        s.groundY = pos.y;
    }

    void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = Segments;
        lr.startWidth = lr.endWidth = 0.55f;
        lr.material = Fx.Mat("Shock");
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    void Update()
    {
        if (GameManager.Paused) return;
        radius += speed * Time.deltaTime;
        for (int i = 0; i < Segments; i++)
        {
            float a = i / (float)Segments * Mathf.PI * 2f;
            lr.SetPosition(i, transform.position + new Vector3(Mathf.Cos(a) * radius, 0.3f, Mathf.Sin(a) * radius));
        }
        var frog = FrogController.Instance;
        if (!hit && frog)
        {
            Vector3 fp = frog.transform.position;
            Vector3 a = fp; a.y = 0;
            Vector3 b = transform.position; b.y = 0;
            if (Mathf.Abs(Vector3.Distance(a, b) - radius) < band && fp.y - groundY < 0.7f && fp.y - groundY > -1f)
                hit = PlayerHealth.Instance && PlayerHealth.Instance.Damage(1, transform.position);
        }
        if (radius > maxRadius) Destroy(gameObject);
    }
}

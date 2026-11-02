using UnityEngine;

// Mala životinja koja šeta oko svog mjesta (rak na plaži, kornjača...). Ukras, ne napada.
public class Critter : MonoBehaviour
{
    public float radius = 3f;
    public float speed = 1.5f;
    public bool sideways = false;     // rakovi hodaju bočno
    public Transform model;

    Vector3 home, target;
    float pause;
    float seed;

    void Start()
    {
        home = transform.position;
        seed = Random.value * 10f;
        Pick();
    }

    void Pick()
    {
        Vector2 r = Random.insideUnitCircle * radius;
        target = home + new Vector3(r.x, 0, r.y);
    }

    void Update()
    {
        if (pause > 0f) { pause -= Time.deltaTime; return; }
        Vector3 to = target - transform.position; to.y = 0;
        if (to.magnitude < 0.2f)
        {
            Pick();
            pause = Random.Range(0.5f, 2.5f);
            return;
        }
        Vector3 dir = to.normalized;
        transform.position += dir * speed * Time.deltaTime;
        Vector3 face = sideways ? Vector3.Cross(Vector3.up, dir) : dir;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(face), 8f * Time.deltaTime);
        if (model) model.localPosition = Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 14f + seed)) * 0.06f;
    }
}

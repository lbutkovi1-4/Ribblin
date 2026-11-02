using System.Collections.Generic;
using UnityEngine;

// Zmija koja vijuga po tlu oko svog mjesta (ukras, ne napada).
public class Snake : MonoBehaviour
{
    public Transform[] segments;   // [0] = glava
    public float radius = 3f;
    public float speed = 1.6f;
    public float spacing = 0.32f;

    Vector3 home;
    Vector3 target;
    readonly List<Vector3> trail = new List<Vector3>();
    float wiggle;

    void Start()
    {
        home = transform.position;
        Pick();
        for (int i = 0; i < 200; i++) trail.Add(home);
    }

    void Pick()
    {
        Vector2 r = Random.insideUnitCircle * radius;
        target = home + new Vector3(r.x, 0, r.y);
    }

    void Update()
    {
        if (segments == null || segments.Length == 0) return;
        Transform head = segments[0];
        Vector3 to = target - head.position; to.y = 0;
        if (to.magnitude < 0.4f) Pick();
        wiggle += Time.deltaTime * 6f;
        Vector3 dir = to.normalized;
        Vector3 side = Vector3.Cross(Vector3.up, dir);
        Vector3 move = (dir + side * Mathf.Sin(wiggle) * 0.6f).normalized * speed * Time.deltaTime;
        head.position += move;
        if (move.sqrMagnitude > 0.000001f) head.rotation = Quaternion.LookRotation(move);

        if (Vector3.Distance(trail[0], head.position) > 0.05f)
        {
            trail.Insert(0, head.position);
            if (trail.Count > 400) trail.RemoveAt(trail.Count - 1);
        }

        // ostali dijelovi prate trag glave
        for (int i = 1; i < segments.Length; i++)
        {
            float want = spacing * i;
            float acc = 0f;
            Vector3 p = trail[trail.Count - 1];
            for (int j = 1; j < trail.Count; j++)
            {
                float d = Vector3.Distance(trail[j - 1], trail[j]);
                if (acc + d >= want)
                {
                    p = Vector3.Lerp(trail[j - 1], trail[j], (want - acc) / Mathf.Max(d, 0.0001f));
                    break;
                }
                acc += d;
            }
            segments[i].position = p;
        }
    }
}

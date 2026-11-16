using System.Collections.Generic;
using UnityEngine;

// Dio levela iznad kojeg kruže ptice i s neba padaju ptičji "darovi".
// Prije svakog pada na tlu se pojavi tamna sjena koja raste – makni se s nje!
public class BirdDropZone : MonoBehaviour
{
    public Vector3 size = new Vector3(8f, 0f, 40f);   // širina i duljina područja (y se ne koristi)
    public float groundY;                             // visina tla (mosta)
    public float interval = 0.8f;                     // koliko često pada
    public float warnTime = 1.1f;                     // koliko dugo se vidi sjena prije udarca
    public float hitRadius = 0.9f;

    class Drop { public Transform shadow, blob; public Vector3 target; public float t; public bool landed; }
    readonly List<Drop> drops = new List<Drop>();
    float timer;
    int count;
    bool hintShown;

    bool FrogInside(out Vector3 fp)
    {
        fp = Vector3.zero;
        var frog = FrogController.Instance;
        if (!frog) return false;
        fp = frog.transform.position;
        Vector3 local = transform.InverseTransformPoint(fp);
        return Mathf.Abs(local.x) < size.x * 0.5f + 2f && Mathf.Abs(local.z) < size.z * 0.5f + 3f && Mathf.Abs(fp.y - groundY) < 6f;
    }

    void Update()
    {
        if (GameManager.Paused) return;
        bool inside = FrogInside(out var fp);

        if (inside && !GameManager.LevelDone && !GameManager.IsGameOver)
        {
            if (!hintShown) { GameManager.ShowMessage("Birds overhead! Watch the shadows and dodge the droppings!", 3.5f); hintShown = true; }
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = interval * Random.Range(0.7f, 1.3f);
                count++;
                Vector3 target;
                if (count % 4 == 0)
                {
                    // svaki četvrti pada točno na žabicu (malo ispred nje)
                    var frog = FrogController.Instance;
                    Vector3 v = frog.Velocity; v.y = 0;
                    target = fp + v * 0.6f;
                }
                else
                {
                    Vector3 local = new Vector3(Random.Range(-0.5f, 0.5f) * size.x, 0, Random.Range(-0.5f, 0.5f) * size.z);
                    target = transform.TransformPoint(local);
                }
                Vector3 l2 = transform.InverseTransformPoint(target);
                l2.x = Mathf.Clamp(l2.x, -size.x * 0.5f, size.x * 0.5f);
                l2.z = Mathf.Clamp(l2.z, -size.z * 0.5f, size.z * 0.5f);
                target = transform.TransformPoint(l2);
                target.y = groundY;
                Spawn(target);
            }
        }

        for (int i = drops.Count - 1; i >= 0; i--)
        {
            var d = drops[i];
            d.t += Time.deltaTime;
            if (!d.landed)
            {
                float k = Mathf.Clamp01(d.t / warnTime);
                d.shadow.localScale = new Vector3(0.4f + k * 1.6f, 0.02f, 0.4f + k * 1.6f);
                // kapljica pada zadnjih 0.35 s
                float fall = Mathf.Clamp01((d.t - (warnTime - 0.35f)) / 0.35f);
                d.blob.position = d.target + Vector3.up * Mathf.Lerp(18f, 0.2f, fall * fall);
                d.blob.gameObject.SetActive(fall > 0f);
                if (d.t >= warnTime) Land(d);
            }
            else
            {
                // mrlja ostane malo i nestane
                float k = (d.t - warnTime) / 1.5f;
                if (k >= 1f)
                {
                    Destroy(d.shadow.gameObject);
                    Destroy(d.blob.gameObject);
                    drops.RemoveAt(i);
                    continue;
                }
                d.blob.localScale = new Vector3(1.4f, 0.08f, 1.4f) * (1f - k * 0.6f);
            }
        }
    }

    void Spawn(Vector3 target)
    {
        var d = new Drop { target = target };
        d.shadow = Fx.Prim(PrimitiveType.Cylinder, target + Vector3.up * 0.05f, new Vector3(0.4f, 0.02f, 0.4f), "PoopDark").transform;
        d.blob = Fx.Prim(PrimitiveType.Sphere, target + Vector3.up * 18f, new Vector3(0.45f, 0.6f, 0.45f), "Poop").transform;
        d.blob.gameObject.SetActive(false);
        d.shadow.SetParent(transform, true);
        d.blob.SetParent(transform, true);
        drops.Add(d);
    }

    void Land(Drop d)
    {
        d.landed = true;
        d.blob.gameObject.SetActive(true);
        d.blob.position = d.target + Vector3.up * 0.08f;
        d.blob.localScale = new Vector3(1.4f, 0.08f, 1.4f);
        d.shadow.localScale = Vector3.zero;
        Fx.Burst(d.target + Vector3.up * 0.2f, "Poop", 5, 0.6f);
        Sfx.Play("splash", 0.25f);
        var frog = FrogController.Instance;
        if (frog)
        {
            Vector3 p = frog.transform.position;
            Vector3 a = p; a.y = 0; Vector3 b = d.target; b.y = 0;
            if (Vector3.Distance(a, b) < hitRadius && p.y - groundY < 2.5f && p.y - groundY > -1f)
                if (PlayerHealth.Instance && PlayerHealth.Instance.Damage(1, d.target - transform.forward * 0.6f))
                    GameManager.ShowMessage("Splat! Right on the head!", 1.5f);
        }
    }
}

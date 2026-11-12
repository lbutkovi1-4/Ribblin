using UnityEngine;

// Zamka: šiljci koji povremeno iskoče iz tla. Prije toga se zatresu (upozorenje).
public class PopSpikes : MonoBehaviour
{
    public float period = 3f;
    public float upTime = 1.2f;
    public float offset = 0f;
    public Transform spikes;
    public DamageZone zone;
    float t;
    bool wasUp;

    void Start() { t = offset; }

    void Update()
    {
        if (GameManager.Paused || !spikes) return;
        t += Time.deltaTime;
        float c = Mathf.Repeat(t, period);
        bool up = c > period - upTime;
        bool warn = !up && c > period - upTime - 0.6f;
        float y = up ? 0f : warn ? -0.45f + Mathf.Sin(Time.time * 50f) * 0.05f : -0.75f;
        spikes.localPosition = new Vector3(0, Mathf.Lerp(spikes.localPosition.y, y, 25f * Time.deltaTime), 0);
        if (zone) zone.zoneActive = up && spikes.localPosition.y > -0.25f;

        var frog = FrogController.Instance;
        if (up && !wasUp && frog && Vector3.Distance(frog.transform.position, transform.position) < 12f)
            Sfx.Play("spikes", 0.4f);
        wasUp = up;
    }
}

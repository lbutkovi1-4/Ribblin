using System.Collections.Generic;
using UnityEngine;

// Mjesto za koje se jezik može uhvatiti. Može biti zaključano dok ne skupiš dovoljno voća.
public class GrapplePoint : MonoBehaviour
{
    public static readonly List<GrapplePoint> All = new List<GrapplePoint>();

    public Transform visual;
    public GameObject lockedVisual;
    public bool bob = true;
    public bool targetable = true;
    public int requiredFruit = 0;

    public event System.Action<GrapplePoint> Grappled;

    bool highlighted;
    bool wasLocked;
    Vector3 baseScale = Vector3.one;
    Vector3 baseLocal;
    float seed;
    float hintCooldown;

    public bool IsLocked => requiredFruit > 0 && GameManager.Collected < requiredFruit;
    public bool CanTarget => targetable && !IsLocked && isActiveAndEnabled;

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }

    void Start()
    {
        if (visual) baseScale = visual.localScale;
        baseLocal = transform.localPosition;
        seed = Random.value * 10f;
        wasLocked = IsLocked;
        RefreshLockVisual();
    }

    void Update()
    {
        if (bob) transform.localPosition = baseLocal + Vector3.up * Mathf.Sin(Time.time * 1.5f + seed) * 0.15f;

        bool locked = IsLocked;
        if (locked != wasLocked)
        {
            wasLocked = locked;
            RefreshLockVisual();
            if (!locked) { Fx.Puff(transform.position, "Dust", 2f); }
        }

        if (visual && !locked)
        {
            float s = !targetable ? 0.75f : highlighted ? 1.18f + Mathf.Sin(Time.time * 10f) * 0.05f : 1f;
            visual.localScale = Vector3.Lerp(visual.localScale, baseScale * s, 15f * Time.deltaTime);
        }

        if (locked)
        {
            hintCooldown -= Time.deltaTime;
            var frog = FrogController.Instance;
            if (frog && hintCooldown <= 0f && Vector3.Distance(frog.transform.position, transform.position) < 12f)
            {
                int missing = requiredFruit - GameManager.Collected;
                GameManager.ShowMessage($"Collect {missing} more fruit to unlock the path to the boss!", 2.5f);
                hintCooldown = 3f;
            }
        }
    }

    void RefreshLockVisual()
    {
        bool locked = IsLocked;
        if (visual) visual.gameObject.SetActive(!locked);
        if (lockedVisual) lockedVisual.SetActive(locked);
    }

    public void SetHighlighted(bool value) => highlighted = value;
    public void NotifyGrappled() => Grappled?.Invoke(this);
}

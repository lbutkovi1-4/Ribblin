using UnityEngine;

// Portal na kraju levela – pojavi se kad pobijediš bossa.
public class Portal : MonoBehaviour
{
    public string nextScene;
    public Transform ring;
    bool used;

    void OnEnable()
    {
        Fx.Puff(transform.position + Vector3.up * 2f, "PortalCore", 4f);
    }

    void Update()
    {
        if (ring) ring.Rotate(0, 0, 90f * Time.deltaTime, Space.Self);
        if (used) return;
        var frog = FrogController.Instance;
        if (frog && Vector3.Distance(frog.transform.position + Vector3.up, transform.position + Vector3.up * 2f) < 2.4f)
        {
            used = true;
            GameManager.CompleteLevel(nextScene);
        }
    }
}

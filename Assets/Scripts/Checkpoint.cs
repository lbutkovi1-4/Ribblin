using UnityEngine;

// Zastavica: nakon pada ili gubitka srca žabica se vraća ovdje.
public class Checkpoint : MonoBehaviour
{
    public float radius = 3f;
    public Transform flag;
    bool reached;

    void Update()
    {
        if (flag && reached) flag.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 4f) * 15f, 0);
        if (reached) return;
        var frog = FrogController.Instance;
        if (frog && Vector3.Distance(frog.transform.position, transform.position) < radius)
        {
            reached = true;
            frog.SetCheckpoint(transform.position + Vector3.up * 0.5f);
            PlayerHealth.Instance?.Heal(3);
            GameManager.ShowMessage("Checkpoint! Hearts restored.", 2f);
            Sfx.Play("checkpoint", 0.5f);
            if (flag)
            {
                var r = flag.GetComponentInChildren<Renderer>();
                if (r) r.sharedMaterial = Fx.Mat("FlagOn");
            }
        }
    }
}

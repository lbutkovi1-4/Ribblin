using UnityEngine;

// Zamka: njihalo (balvan, čekić...) koje se njiše preko staze.
public class Pendulum : MonoBehaviour
{
    public float angle = 60f;
    public float period = 2.6f;
    public float offset = 0f;

    void Update()
    {
        if (GameManager.Paused) return;
        float a = Mathf.Sin((Time.time / period + offset) * Mathf.PI * 2f) * angle;
        transform.localRotation = Quaternion.Euler(0, 0, a);
    }
}

using UnityEngine;

// Gljiva-trampolin: kad žabica stane na nju, odbaci je visoko.
public class BouncePad : MonoBehaviour
{
    public float bounceHeight = 9f;
    public Transform cap;
    Vector3 capScale;
    float squash;

    void Start() { if (cap) capScale = cap.localScale; }

    void Update()
    {
        var frog = FrogController.Instance;
        if (frog && frog.IsGrounded && frog.GroundTransform && frog.GroundTransform.IsChildOf(transform))
        {
            frog.Bounce(bounceHeight);
            squash = 1f;
            Sfx.Play("boing", 0.6f);
        }
        if (cap)
        {
            squash = Mathf.MoveTowards(squash, 0f, Time.deltaTime * 4f);
            float k = Mathf.Sin(squash * Mathf.PI * 3f) * squash * 0.25f;
            cap.localScale = new Vector3(capScale.x * (1 + k), capScale.y * (1 - k), capScale.z * (1 + k));
        }
    }
}

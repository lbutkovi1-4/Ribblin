using UnityEngine;
using UnityEngine.InputSystem;

// Kamera iza žabice: miš okreće, kotačić zumira, ne prolazi kroz zidove.
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 focusOffset = new Vector3(0, 1.2f, 0);
    public float distance = 8f;
    public float minZoom = 4f;
    public float maxZoom = 14f;
    public float minDistance = 1.5f;
    public float sensitivity = 0.12f;
    public float minPitch = -10f;
    public float maxPitch = 70f;
    public float followSmooth = 15f;

    static float shake;
    public static void Shake(float amount) => shake = Mathf.Max(shake, amount);

    float yaw;
    float pitch = 20f;
    Vector3 smoothFocus;

    void Start()
    {
        yaw = target ? target.eulerAngles.y : transform.eulerAngles.y;
        if (target) smoothFocus = target.position + focusOffset;
        shake = 0f;
    }

    void LateUpdate()
    {
        if (!target) return;

        var mouse = Mouse.current;
        if (mouse != null && !GameManager.Paused && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 d = mouse.delta.ReadValue();
            yaw += d.x * sensitivity;
            pitch -= d.y * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f) distance = Mathf.Clamp(distance - Mathf.Sign(scroll) * 0.8f, minZoom, maxZoom);
        }

        smoothFocus = Vector3.Lerp(smoothFocus, target.position + focusOffset, followSmooth * Time.unscaledDeltaTime);
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 back = rot * Vector3.back;

        float dist = distance;
        var hits = Physics.SphereCastAll(smoothFocus, 0.3f, back, distance, ~0, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.transform == target || hit.transform.IsChildOf(target)) continue;
            if (hit.distance <= 0f) continue;
            dist = Mathf.Min(dist, Mathf.Max(minDistance, hit.distance - 0.1f));
        }

        Vector3 shakeOffset = shake > 0f ? Random.insideUnitSphere * shake * 0.5f : Vector3.zero;
        shake = Mathf.MoveTowards(shake, 0f, Time.deltaTime * 1.5f);

        transform.position = smoothFocus + back * dist + shakeOffset;
        transform.rotation = rot;
    }
}

using UnityEngine;

// Kamera koja polako kruži oko otoka u izborniku.
public class MenuCameraOrbit : MonoBehaviour
{
    public Vector3 center;
    public float radius = 18f;
    public float height = 8f;
    public float speed = 8f;
    float angle;

    void Update()
    {
        angle += speed * Time.deltaTime;
        float a = angle * Mathf.Deg2Rad;
        transform.position = center + new Vector3(Mathf.Cos(a) * radius, height, Mathf.Sin(a) * radius);
        transform.LookAt(center + Vector3.up * 1.5f);
    }
}

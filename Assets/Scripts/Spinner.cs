using UnityEngine;

// Okreće objekt (npr. vrtuljak-prepreku ili ukras).
public class Spinner : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed = 90f;

    void Update()
    {
        if (GameManager.Paused) return;
        transform.Rotate(axis, speed * Time.deltaTime, Space.Self);
    }
}

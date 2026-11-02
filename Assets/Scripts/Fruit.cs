using UnityEngine;

// Voće koje žabica skuplja. Vrti se i lebdi; kad se žabica približi, pokupi se.
public class Fruit : MonoBehaviour
{
    public float pickupRadius = 1.3f;
    public float spinSpeed = 90f;

    Vector3 basePos;
    float seed;

    void Start()
    {
        basePos = transform.position;
        seed = Random.value * 10f;
        GameManager.RegisterFruit();
    }

    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0, Space.World);
        transform.position = basePos + Vector3.up * (Mathf.Sin(Time.time * 2f + seed) * 0.2f);

        var frog = FrogController.Instance;
        if (frog && Vector3.Distance(frog.transform.position + Vector3.up * 0.5f, transform.position) < pickupRadius)
        {
            GameManager.CollectFruit();
            Fx.Puff(transform.position, "Dust", 0.8f);
            Destroy(gameObject);
        }
    }
}

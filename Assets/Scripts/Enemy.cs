using System.Collections.Generic;
using UnityEngine;

// Zajednička logika svih običnih neprijatelja:
// popis (za jezik), smrt s efektom raspadanja, skok na glavu i dodir sa strane.
public abstract class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> All = new List<Enemy>();

    public string debrisMat = "SlimeGreen";   // boja komadića kad se uništi
    public bool spiky = false;                 // bodljikav: skok na glavu boli, pobijedi ga jezikom

    protected bool dead;
    public bool Alive => !dead;
    public abstract Vector3 Center { get; }

    protected virtual void OnEnable() { All.Add(this); }
    protected virtual void OnDisable() { All.Remove(this); }

    public virtual void Eat()
    {
        Sfx.Play("gulp", 0.6f);
        Kill();
    }

    public void Kill()
    {
        if (dead) return;
        dead = true;
        All.Remove(this);
        Fx.Burst(Center, debrisMat, 12, 1f);
        Fx.Puff(Center, "Dust", 1.6f);
        Sfx.Play("enemy_die", 0.7f);
        ThirdPersonCamera.Shake(0.12f);
        OnKilled();
    }

    protected virtual void OnKilled() { Destroy(gameObject); }

    // Provjera dodira sa žabicom. Vraća true ako je neprijatelj uništen.
    protected bool HandleContact(float radius, float bottomY, float topY)
    {
        var frog = FrogController.Instance;
        if (!frog || dead) return false;
        Vector3 fp = frog.transform.position;
        float fd = Flat(fp, transform.position);

        bool fromAbove = fd < radius + 0.5f && fp.y > topY - 0.5f && fp.y < topY + 1.0f && frog.Velocity.y < 0.5f && !frog.IsGrounded;
        if (fromAbove)
        {
            if (spiky)
            {
                frog.Bounce(3f);
                PlayerHealth.Instance?.Damage(1, transform.position);
                return false;
            }
            frog.Bounce(2.8f);
            Sfx.Play("stomp", 0.6f);
            Kill();
            return true;
        }
        if (fd < radius + 0.4f && fp.y < topY - 0.4f && fp.y > bottomY - 1f)
            PlayerHealth.Instance?.Damage(1, transform.position);
        return false;
    }

    protected static float Flat(Vector3 a, Vector3 b) { a.y = 0; b.y = 0; return Vector3.Distance(a, b); }
}

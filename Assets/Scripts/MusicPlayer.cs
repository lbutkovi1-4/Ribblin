using UnityEngine;

// Pušta glazbu za scenu. Glazbene datoteke idu u Assets/Resources/Music/
// (npr. Level1.mp3). Ako datoteka ne postoji, igra radi i bez glazbe.
public class MusicPlayer : MonoBehaviour
{
    public string trackName = "Menu";
    public float volume = 0.35f;
    AudioSource source;

    void Start()
    {
        var clip = Resources.Load<AudioClip>("Music/" + trackName);
        if (clip == null) return;
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = 0f;
        source.playOnAwake = false;
        source.Play();
    }

    void Update()
    {
        if (!source) return;
        float target = GameManager.Paused ? volume * 0.4f : volume;
        source.volume = Mathf.MoveTowards(source.volume, target, Time.unscaledDeltaTime * 0.3f);
    }
}

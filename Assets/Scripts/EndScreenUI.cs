using UnityEngine;
using UnityEngine.SceneManagement;

// Završni ekran nakon zadnjeg bossa.
public class EndScreenUI : MonoBehaviour
{
    public string menuScene = "Ribblin_Menu";

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Sfx.Init();
        Sfx.Play("win", 0.7f);
    }

    void OnGUI()
    {
        float s = Ui.S;
        Ui.Text(new Rect(0, 100 * s, Screen.width, 160 * s), "POBJEDA!", Ui.Label(140, TextAnchor.MiddleCenter), new Color(1f, 0.85f, 0.3f));
        Ui.Text(new Rect(0, 260 * s, Screen.width, 60 * s), "Ribblin je spasio sve otoke!", Ui.Label(46, TextAnchor.MiddleCenter));

        int m = (int)(GameStats.TotalTime / 60), sec = (int)(GameStats.TotalTime % 60);
        var r = Ui.Centered(370, 820, 300);
        Ui.Box(r, new Color(0, 0, 0, 0.45f));
        Ui.Text(r,
            $"Ukupno vrijeme: {m:00}:{sec:00}\n" +
            $"Voće: {GameStats.FruitCollected} / {GameStats.FruitTotal}\n" +
            $"Pobijeđeni bossovi: {GameStats.BossesDefeated}\n" +
            $"Padovi: {GameStats.Deaths}",
            Ui.Label(40, TextAnchor.MiddleCenter));

        if (Ui.Button(Ui.Centered(720, 460, 85), "Glavni izbornik")) SceneManager.LoadScene(menuScene);
        if (Ui.Button(Ui.Centered(825, 460, 85), "Izlaz")) Application.Quit();
    }
}

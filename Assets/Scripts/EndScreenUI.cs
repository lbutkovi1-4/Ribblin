using UnityEngine;
using UnityEngine.SceneManagement;

// Završni ekran nakon zadnjeg bossa.
public class EndScreenUI : MonoBehaviour
{
    public string menuScene = "Ribblin_Menu";
    bool newBest;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Sfx.Init();
        Sfx.Play("win", 0.7f);
        newBest = GameStats.FinishRun();
    }

    void OnGUI()
    {
        float s = Ui.S;
        Ui.Text(new Rect(0, 100 * s, Screen.width, 160 * s), "VICTORY!", Ui.Label(140, TextAnchor.MiddleCenter), new Color(1f, 0.85f, 0.3f));
        Ui.Text(new Rect(0, 260 * s, Screen.width, 60 * s), "Ribblin saved all the islands!", Ui.Label(46, TextAnchor.MiddleCenter));

        int m = (int)(GameStats.TotalTime / 60), sec = (int)(GameStats.TotalTime % 60);
        var r = Ui.Centered(370, 820, 300);
        Ui.Box(r, new Color(0, 0, 0, 0.45f));
        Ui.Text(r,
            $"Total time: {m:00}:{sec:00}\n" +
            $"Fruit: {GameStats.FruitCollected} / {GameStats.FruitTotal}\n" +
            $"Bosses defeated: {GameStats.BossesDefeated}\n" +
            $"Falls: {GameStats.Deaths}",
            Ui.Label(40, TextAnchor.MiddleCenter));

        if (newBest)
            Ui.Text(new Rect(0, 675 * s, Screen.width, 40 * s), "New best time!", Ui.Label(40, TextAnchor.MiddleCenter), new Color(1f, 0.85f, 0.3f));
        else if (GameStats.HasBestTime)
            Ui.Text(new Rect(0, 675 * s, Screen.width, 40 * s), "Best time: " + GameStats.Format(GameStats.BestTime), Ui.Label(32, TextAnchor.MiddleCenter));

        if (Ui.Button(Ui.Centered(720, 460, 85), "Main menu")) SceneManager.LoadScene(menuScene);
        if (Ui.Button(Ui.Centered(825, 460, 85), "Quit")) Application.Quit();
    }
}

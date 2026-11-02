using UnityEngine;
using UnityEngine.SceneManagement;

// Glavni izbornik.
public class MainMenuUI : MonoBehaviour
{
    public string[] levelScenes = { "Level1_Otok", "Level2_Mocvara", "Level3_Vulkan", "Level4_Nebo" };
    public string[] levelNames = { "1 · Tropski otok", "2 · Močvara", "3 · Vulkan", "4 · Nebo" };
    bool showLevels;
    bool showHelp;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Sfx.Init();
    }

    void OnGUI()
    {
        float s = Ui.S;
        Ui.Text(new Rect(0, 90 * s, Screen.width, 200 * s), "RIBBLIN", Ui.Label(170, TextAnchor.MiddleCenter), new Color(0.6f, 1f, 0.35f));
        Ui.Text(new Rect(0, 270 * s, Screen.width, 60 * s), "Pustolovina male žabice", Ui.Label(44, TextAnchor.MiddleCenter), new Color(1f, 0.92f, 0.6f));

        if (showLevels)
        {
            for (int i = 0; i < levelScenes.Length; i++)
                if (Ui.Button(Ui.Centered(400 + i * 100, 560, 80), levelNames[i]))
                {
                    GameStats.Reset();
                    SceneManager.LoadScene(levelScenes[i]);
                }
            if (Ui.Button(Ui.Centered(820, 560, 80), "Natrag")) showLevels = false;
            return;
        }

        if (showHelp)
        {
            var r = Ui.Centered(380, 1200, 420);
            Ui.Box(r, new Color(0, 0, 0, 0.5f));
            Ui.Text(new Rect(r.x + 30 * s, r.y + 20 * s, r.width - 60 * s, r.height - 40 * s),
                "WASD – kretanje      Space – skok (u zraku još jedan)      Shift – trčanje\n" +
                "Miš – kamera      Kotačić – zum      Esc – pauza\n" +
                "Lijevi klik na crvenu rotkvicu – jezik te povuče do nje\n" +
                "Lijevi klik na sluzavca u blizini – pojedeš ga (ili mu skoči na glavu)\n\n" +
                "Skupi dovoljno voća da otključaš put do bossa.\nPobijedi bossa i uđi u portal!",
                Ui.Label(32, TextAnchor.MiddleCenter, false));
            if (Ui.Button(Ui.Centered(840, 460, 80), "Natrag")) showHelp = false;
            return;
        }

        if (Ui.Button(Ui.Centered(420, 460, 85), "Nova igra"))
        {
            GameStats.Reset();
            SceneManager.LoadScene(levelScenes[0]);
        }
        if (Ui.Button(Ui.Centered(525, 460, 85), "Odaberi level")) showLevels = true;
        if (Ui.Button(Ui.Centered(630, 460, 85), "Kontrole")) showHelp = true;
        if (Ui.Button(Ui.Centered(735, 460, 85), "Izlaz")) Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

// Glavni izbornik.
public class MainMenuUI : MonoBehaviour
{
    public string[] levelScenes = { "Level1_Otok", "Level2_Mocvara", "Level3_Vulkan", "Level4_Nebo" };
    public string[] levelNames = { "1 · Tropical Island", "2 · Murky Swamp", "3 · Volcano", "4 · Sky Islands" };
    bool showLevels;
    bool showHelp;
    bool showCredits;

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
        Ui.Text(new Rect(0, 270 * s, Screen.width, 60 * s), "A little frog's big adventure", Ui.Label(44, TextAnchor.MiddleCenter), new Color(1f, 0.92f, 0.6f));

        if (showLevels)
        {
            for (int i = 0; i < levelScenes.Length; i++)
                if (Ui.Button(Ui.Centered(400 + i * 100, 560, 80), levelNames[i]))
                {
                    GameStats.Reset();
                    SceneManager.LoadScene(levelScenes[i]);
                }
            if (Ui.Button(Ui.Centered(820, 560, 80), "Back")) showLevels = false;
            return;
        }

        if (showCredits)
        {
            var cr = Ui.Centered(330, 1300, 470);
            Ui.Box(cr, new Color(0, 0, 0, 0.5f));
            var txt = Resources.Load<TextAsset>("Music/credits");
            string credits = "Game design & code: Lidia Butković\n\nMusic:\n" + (txt ? txt.text : "(no music added yet)");
            Ui.Text(new Rect(cr.x + 30 * s, cr.y + 20 * s, cr.width - 60 * s, cr.height - 40 * s), credits, Ui.Label(28, TextAnchor.MiddleCenter, false));
            if (Ui.Button(Ui.Centered(840, 460, 80), "Back")) showCredits = false;
            return;
        }

        if (showHelp)
        {
            var r = Ui.Centered(380, 1200, 420);
            Ui.Box(r, new Color(0, 0, 0, 0.5f));
            Ui.Text(new Rect(r.x + 30 * s, r.y + 20 * s, r.width - 60 * s, r.height - 40 * s),
                "WASD – move      Space – jump (again in the air)      Shift – run\n" +
                "Mouse – camera      Wheel – zoom      Esc – pause\n" +
                "Left click on a red radish – your tongue pulls you to it\n" +
                "Left click on a nearby slime – eat it (or jump on its head)\n\n" +
                "Collect enough fruit to unlock the path to the boss.\nDefeat the boss and enter the portal!",
                Ui.Label(32, TextAnchor.MiddleCenter, false));
            if (Ui.Button(Ui.Centered(840, 460, 80), "Back")) showHelp = false;
            return;
        }

        if (Ui.Button(Ui.Centered(420, 460, 85), "New game"))
        {
            GameStats.Reset();
            SceneManager.LoadScene(levelScenes[0]);
        }
        if (Ui.Button(Ui.Centered(525, 460, 85), "Select level")) showLevels = true;
        if (Ui.Button(Ui.Centered(630, 460, 85), "Controls")) showHelp = true;
        if (Ui.Button(Ui.Centered(735, 460, 85), "Credits")) showCredits = true;
        if (Ui.Button(Ui.Centered(840, 460, 85), "Quit")) Application.Quit();
    }
}

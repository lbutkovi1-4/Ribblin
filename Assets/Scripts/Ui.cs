using UnityEngine;

// Pomoćne funkcije za crtanje sučelja (OnGUI).
public static class Ui
{
    static Texture2D white;
    public static float S => Screen.height / 1080f;

    public static Texture2D White
    {
        get
        {
            if (white == null)
            {
                white = new Texture2D(1, 1);
                white.SetPixel(0, 0, Color.white);
                white.Apply();
            }
            return white;
        }
    }

    public static GUIStyle Label(int size, TextAnchor anchor = TextAnchor.UpperLeft, bool bold = true)
    {
        var s = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Max(8, Mathf.RoundToInt(size * S)),
            alignment = anchor,
            fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
            wordWrap = true
        };
        s.normal.textColor = Color.white;
        return s;
    }

    public static void Text(Rect r, string text, GUIStyle style, Color? color = null)
    {
        var old = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.65f * old.a);
        GUI.Label(new Rect(r.x + 2 * S + 1, r.y + 2 * S + 1, r.width, r.height), text, style);
        GUI.color = color ?? Color.white;
        GUI.Label(r, text, style);
        GUI.color = old;
    }

    public static void Box(Rect r, Color c)
    {
        var old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, White);
        GUI.color = old;
    }

    public static bool Button(Rect r, string text)
    {
        bool hover = r.Contains(Event.current.mousePosition);
        Box(new Rect(r.x + 4 * S, r.y + 5 * S, r.width, r.height), new Color(0, 0, 0, 0.35f));
        Box(r, hover ? new Color(1f, 0.6f, 0.3f, 0.97f) : new Color(0.22f, 0.6f, 0.32f, 0.95f));
        Text(r, text, Label(34, TextAnchor.MiddleCenter));
        bool clicked = GUI.Button(r, GUIContent.none, GUIStyle.none);
        if (clicked) Sfx.Play("click", 0.5f);
        return clicked;
    }

    public static Rect Centered(float y, float w, float h)
    {
        return new Rect(Screen.width / 2f - w * S / 2f, y * S, w * S, h * S);
    }
}

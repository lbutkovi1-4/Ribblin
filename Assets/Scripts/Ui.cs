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

    // Klizač: naziv lijevo, traka desno. Vraća novu vrijednost (min – max).
    public static float Slider(Rect r, string label, float value, float min, float max)
    {
        float t = Mathf.InverseLerp(min, max, value);
        string shown = max <= 1f ? $"{Mathf.RoundToInt(t * 100)} %" : $"{value:0.0}x";   // glasnoća u %, miš kao množitelj
        Text(new Rect(r.x, r.y, r.width * 0.45f, r.height), $"{label}: {shown}", Label(30, TextAnchor.MiddleLeft));

        var track = new Rect(r.x + r.width * 0.48f, r.y + r.height * 0.5f - 6 * S, r.width * 0.5f, 12 * S);
        var hit = new Rect(track.x - 10 * S, r.y, track.width + 20 * S, r.height);
        int id = GUIUtility.GetControlID(FocusType.Passive);
        var e = Event.current;
        switch (e.GetTypeForControl(id))
        {
            case EventType.MouseDown:
                if (hit.Contains(e.mousePosition)) { GUIUtility.hotControl = id; t = Mathf.InverseLerp(track.x, track.xMax, e.mousePosition.x); e.Use(); }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == id) { t = Mathf.InverseLerp(track.x, track.xMax, e.mousePosition.x); e.Use(); }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id) { GUIUtility.hotControl = 0; e.Use(); }
                break;
        }

        Box(track, new Color(0, 0, 0, 0.5f));
        Box(new Rect(track.x, track.y, track.width * t, track.height), new Color(0.35f, 0.85f, 0.4f));
        float k = 30 * S;
        Box(new Rect(track.x + track.width * t - k / 2f, track.center.y - k / 2f, k, k), Color.white);
        return Mathf.Lerp(min, max, t);
    }

    public static Rect Centered(float y, float w, float h)
    {
        return new Rect(Screen.width / 2f - w * S / 2f, y * S, w * S, h * S);
    }
}

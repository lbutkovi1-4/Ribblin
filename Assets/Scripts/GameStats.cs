using UnityEngine;

// Statistika cijele igre (preko svih levela) i najbolje vrijeme.
public static class GameStats
{
    const string BestKey = "BestTime";
    const int LevelCount = 4;

    public static bool FullRun;   // igra pokrenuta s "New game" (a ne iz "Select level")
    public static float TotalTime;
    public static int FruitCollected;
    public static int FruitTotal;
    public static int Deaths;
    public static int BossesDefeated;
    public static int LevelsCompleted;

    public static void Reset()
    {
        TotalTime = 0; FruitCollected = 0; FruitTotal = 0;
        Deaths = 0; BossesDefeated = 0; LevelsCompleted = 0;
        FullRun = false;
    }

    public static bool HasBestTime => PlayerPrefs.HasKey(BestKey);
    public static float BestTime => PlayerPrefs.GetFloat(BestKey, 0f);

    // Poziva se na završnom ekranu. Vraća true ako je ovo novo najbolje vrijeme.
    // Računa se samo cijela igra od 1. razine, bez preskakanja.
    public static bool FinishRun()
    {
        if (!FullRun || LevelsCompleted < LevelCount) return false;
        FullRun = false;
        if (HasBestTime && TotalTime >= BestTime) return false;
        PlayerPrefs.SetFloat(BestKey, TotalTime);
        PlayerPrefs.Save();
        return true;
    }

    public static string Format(float seconds)
    {
        int m = (int)(seconds / 60), s = (int)(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    public static void AddLevel(float time, int collected, int total)
    {
        TotalTime += time;
        FruitCollected += collected;
        FruitTotal += total;
        LevelsCompleted++;
    }
}

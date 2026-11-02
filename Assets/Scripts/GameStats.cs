// Statistika cijele igre (preko svih levela).
public static class GameStats
{
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
    }

    public static void AddLevel(float time, int collected, int total)
    {
        TotalTime += time;
        FruitCollected += collected;
        FruitTotal += total;
        LevelsCompleted++;
    }
}

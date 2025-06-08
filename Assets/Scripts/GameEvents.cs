using System;

public static class GameEvents
{
    public static event Action<int> OnCubeMerged;

    public static event Action<bool> OnGameOver;

    public static void CubeMerged(int score)
    {
        OnCubeMerged?.Invoke(score);
    }

    public static void GameOver(bool won)
    {
        OnGameOver?.Invoke(won);
    }
}
using System;

public static class GameEvents
{
    public static event Action<int> OnCubeMerged;

    public static void CubeMerged(int score)
    {
        OnCubeMerged?.Invoke(score);
    }
}
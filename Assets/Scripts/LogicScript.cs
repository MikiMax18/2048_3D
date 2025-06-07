using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public static LogicScript Instance { get; private set; }

    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public int winCubeValue = 2048;

    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    private void OnEnable()
    {
        GameEvents.OnCubeMerged += AddScore;
    }

    [ContextMenu("restartGame")]
    public void RestartGame()
    {
        IsGameOver = false;
        playerScore = 0;
        scoreText.text = "0";
        gameOverScreen.SetActive(false);

        foreach (var cube in GameObject.FindGameObjectsWithTag("GameCube"))
        {
            Destroy(cube);
        }

        CubeSpawner spawner = FindAnyObjectByType<CubeSpawner>();
        spawner.OnCubeLaunched();
    }

    public void ShowGameOver()
    {
        IsGameOver = true;
        gameOverScreen.SetActive(true);
    }
}

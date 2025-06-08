using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public static LogicScript Instance { get; private set; }

    public int playerScore;
    public Text scoreText;
    public Text resultText;
    public GameObject gameOverScreen; 
    public int winCubeValue = 2048; 

    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnCubeMerged += AddScore;
        GameEvents.OnGameOver += ShowGameOver;
    }

    public void AddScore(int scoreToAdd)
    {
        // Increase score and update UI
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    [ContextMenu("restartGame")]
    public void RestartGame()
    {
        // Reset game state and score
        IsGameOver = false;
        playerScore = 0;
        scoreText.text = "0";
        gameOverScreen.SetActive(false);

        // Destroy all cubes in the scene
        foreach (var cube in GameObject.FindGameObjectsWithTag("GameCube"))
        {
            Destroy(cube);
        }

        // Spawn a new cube after restart
        CubeSpawner spawner = FindAnyObjectByType<CubeSpawner>();
        spawner.OnCubeLaunched();
    }

    public void ShowGameOver(bool won)
    {
        // Trigger game over screen and stop controls
        IsGameOver = true;
        resultText.text = won ? "You win" : "You lose";
        gameOverScreen.SetActive(true);
    }
}

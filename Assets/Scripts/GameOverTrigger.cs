using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (LogicScript.Instance.IsGameOver)
            return;

        if (other.CompareTag("GameCube") && !other.TryGetComponent(out CubeController component))
        {
            GameEvents.GameOver(false);
        }
    }
}

using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameCube") && !other.TryGetComponent(out CubeController component))
        {
            GameEvents.GameOver(false);
        }
    }
}

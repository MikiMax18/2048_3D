using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; 
    [SerializeField] private Transform spawnPoint;  

    private CubeController currentCube;

    void Start()
    {
        // Spawn the first cube when the game starts
        SpawnNewCube();
    }

    public void SpawnNewCube()
    {
        // If there is still an active cube, don't spawn a new one
        if (currentCube != null) return;

        // Create a new cube at the spawn position
        GameObject cubeObj = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);

        currentCube = cubeObj.GetComponent<CubeController>();
        currentCube.isActiveCube = true;

        // Set a random value (2 or 4) to the cube
        if (cubeObj.TryGetComponent(out NumberCube numberCube))
        {
            int value = Random.value < 0.75f ? 2 : 4;
            numberCube.SetValue(value);
        }
    }

    public void OnCubeLaunched()
    {
        currentCube = null;

        Invoke(nameof(SpawnNewCube), 1f);
    }
}

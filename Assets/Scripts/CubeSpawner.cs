using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform spawnPoint;

    private CubeController currentCube;

    void Start()
    {
        SpawnNewCube();
    }

    public void SpawnNewCube()
    {
        if (currentCube != null) return; // Забороняємо спавн нового, поки є активний

        GameObject cubeObj = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);

        currentCube = cubeObj.GetComponent<CubeController>();
        currentCube.isActiveCube = true;

        // Присвоюємо значення 2 або 4 з ймовірністю 75% / 25%
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

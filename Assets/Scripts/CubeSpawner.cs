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
        if (currentCube != null) return; // ����������� ����� ������, ���� � ��������

        GameObject cubeObj = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);

        currentCube = cubeObj.GetComponent<CubeController>();
        currentCube.isActiveCube = true;

        // ���������� �������� 2 ��� 4 � ��������� 75% / 25%
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

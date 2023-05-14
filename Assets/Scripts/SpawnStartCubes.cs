using UnityEngine;

public class SpawnStartCubes : MonoBehaviour
{
    [SerializeField] private GameObject[] startCubesPrefabs;

    private void Awake()
    {
        Instantiate(startCubesPrefabs[Random.Range(0, startCubesPrefabs.Length)]);
    }
}

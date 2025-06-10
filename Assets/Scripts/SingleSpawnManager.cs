using UnityEngine;

public class SingleSpawnManager : MonoBehaviour
{
    private GameObject _currentSpawnedObject;
    private const string SpawnableTag = "Spawnable";

    void Update()
    {
        GameObject[] spawnableObjects = GameObject.FindGameObjectsWithTag(SpawnableTag);

        if (spawnableObjects.Length > 1)
        {
            for (int i = 0; i < spawnableObjects.Length - 1; i++)
            {
                Destroy(spawnableObjects[i]);
            }
            _currentSpawnedObject = spawnableObjects[spawnableObjects.Length - 1];
        }
        else if (spawnableObjects.Length == 1)
        {
            _currentSpawnedObject = spawnableObjects[0];
        }
        else
        {
            _currentSpawnedObject = null;
        }
    }

    public void RegisterNewSpawnedObject(GameObject newObject)
    {
        if (_currentSpawnedObject != null && _currentSpawnedObject != newObject)
        {
            Destroy(_currentSpawnedObject);
        }
        _currentSpawnedObject = newObject;
    }
}
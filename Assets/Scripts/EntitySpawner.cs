using UnityEngine;

public interface IEntitySpawner
{
    GameObject Spawn(GameObject prefab, Transform spawnPoint);
    GameObject Replace(GameObject prefab, GameObject currentInstance, Transform spawnPoint);
}

public sealed class EntitySpawner : IEntitySpawner
{
    public GameObject Spawn(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null)
        {
            Debug.LogError("EntitySpawner: Prefab is not assigned.");
            return null;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        return Object.Instantiate(prefab, position, rotation);
    }

    public GameObject Replace(GameObject prefab, GameObject currentInstance, Transform spawnPoint)
    {
        if (currentInstance != null)
        {
            Object.Destroy(currentInstance);
        }

        return Spawn(prefab, spawnPoint);
    }
}

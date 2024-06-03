using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemPrefabs; // List of item prefabs
    [SerializeField] private Vector3 spawnOffsetRange; // Range for random offset (x, y, z) from parent
    [SerializeField] private Transform parentTransform; // Parent transform for the spawned items

    void Start()
    {
        if (parentTransform == null)
        {
            parentTransform = transform; // Default to this object's transform if not set
        }
        SpawnRandomItem();
    }

    private void SpawnRandomItem()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0)
        {
            Debug.LogWarning("Item prefabs list is empty!");
            return;
        }

        // Select a random prefab from the list
        int randomIndex = Random.Range(0, itemPrefabs.Count);
        GameObject randomPrefab = itemPrefabs[randomIndex];

        // Generate a random position within the specified offset range
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y),
            Random.Range(-spawnOffsetRange.z, spawnOffsetRange.z)
        );

        Vector3 spawnPosition = parentTransform.position + randomOffset;

        // Instantiate the selected prefab at the random position
        GameObject spawnedItem = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, parentTransform);
    }
}

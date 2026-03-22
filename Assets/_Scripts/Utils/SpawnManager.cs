using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] objectsToSpawn;

    void Start()
    {
        List<int> availableSpawnIndexes = new List<int>();
        int alreadyCollectedFromThisSpawner = 0;

        // Check every spawn point against the save file to see if it was already looted
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Use the GameObject name in the Unity Inspector as the unique ID
            string pointID = spawnPoints[i].name;

            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.collectedItemIDs.Contains(pointID))
            {
                alreadyCollectedFromThisSpawner++;
            }
            else
            {
                availableSpawnIndexes.Add(i);
            }
        }

        // Deduct the already collected items so we don't spawn extras
        int amountToSpawn = objectsToSpawn.Length - alreadyCollectedFromThisSpawner;

        if (amountToSpawn <= 0)
        {
            return; // Everything from this spawner has been collected
        }

        // Spawn the remaining items in random, available locations
        for (int i = 0; i < amountToSpawn; i++)
        {
            if (availableSpawnIndexes.Count == 0) break;

            int randomListIndex = Random.Range(0, availableSpawnIndexes.Count);
            int actualSpawnPointIndex = availableSpawnIndexes[randomListIndex];

            // Remove the location so two items don't spawn in the exact same spot
            availableSpawnIndexes.RemoveAt(randomListIndex);

            GameObject spawnedItem = Instantiate(
                objectsToSpawn[i],
                spawnPoints[actualSpawnPointIndex].position,
                Quaternion.identity
            );

            // Pass the location's ID into the newly spawned item so it knows its identity
            PickupItem pickupScript = spawnedItem.GetComponent<PickupItem>();
            if (pickupScript != null)
            {
                pickupScript.uniqueLocationID = spawnPoints[actualSpawnPointIndex].name;
            }
        }
    }
}
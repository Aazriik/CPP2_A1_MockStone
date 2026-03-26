using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The size of the Spawn Points array MUST match the Objects To Spawn array!")]
    public Transform[] spawnPoints;
    public GameObject[] objectsToSpawn;

    private void Start()
    {
        // 1. Safety check to prevent the shifting array bug
        if (spawnPoints.Length != objectsToSpawn.Length)
        {
            Debug.LogError($"[SpawnManager] {gameObject.name} has a mismatched number of points and objects. They must be equal!");
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // 2. Create a truly unique ID using the Scene Name + Spawner Name + Point Name
            string uniqueID = $"{gameObject.scene.name}_{gameObject.name}_{spawnPoints[i].name}";

            // 3. Check the save file. If this specific spot was looted, skip it and move to the next item!
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.collectedItemIDs.Contains(uniqueID))
            {
                continue;
            }

            // 4. If not collected, spawn the specific object assigned to this specific point
            GameObject spawnedItem = Instantiate(objectsToSpawn[i], spawnPoints[i].position, Quaternion.identity);

            // 5. Inject the unique ID into the pickup script so it knows exactly what to save when grabbed
            PickupItem pickupScript = spawnedItem.GetComponent<PickupItem>();
            if (pickupScript != null)
            {
                pickupScript.uniqueLocationID = uniqueID;
            }
        }
    }
}
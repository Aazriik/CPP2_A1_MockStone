using UnityEngine;
using System;

public class PickupItem : MonoBehaviour
{
    public int value = 1;

    // Hidden because the SpawnManager will fill this in automatically now!
    [HideInInspector] public string uniqueLocationID;

    public static event Action<int, int> OnCollectiblePickedUp;

    private const int MaxCollectibles = 5;

    private void Start()
    {
        // Just update the UI when the item spawns, don't modify the save data here
        if (SaveManager.Instance != null)
        {
            OnCollectiblePickedUp?.Invoke(SaveManager.Instance.CurrentData.totalCollectedItems, MaxCollectibles);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int currentTotal = 0;

            if (SaveManager.Instance != null)
            {
                // 1. Add to the global save data count
                SaveManager.Instance.CurrentData.totalCollectedItems += value;
                currentTotal = SaveManager.Instance.CurrentData.totalCollectedItems;

                // 2. Save this specific item's unique ID so it never spawns again
                if (!string.IsNullOrEmpty(uniqueLocationID) && !SaveManager.Instance.CurrentData.collectedItemIDs.Contains(uniqueLocationID))
                {
                    SaveManager.Instance.CurrentData.collectedItemIDs.Add(uniqueLocationID);
                }

                // 3. Write to disk
                SaveManager.Instance.SaveGame();
            }

            // Notify the UI
            OnCollectiblePickedUp?.Invoke(currentTotal, MaxCollectibles);

            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using System;

public class PickupItem : MonoBehaviour
{
    public int value = 1;
    public string uniqueLocationID;

    public static event Action<int, int> OnCollectiblePickedUp;

    private static int collectedCount = 0;
    private const int MaxCollectibles = 35;

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            collectedCount = SaveManager.Instance.CurrentData.totalCollectedItems;
        }

        OnCollectiblePickedUp?.Invoke(collectedCount, MaxCollectibles);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectedCount += value;
            Debug.Log("Picked up item! Collected: " + collectedCount);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentData.totalCollectedItems = collectedCount;

                if (!string.IsNullOrEmpty(uniqueLocationID) && !SaveManager.Instance.CurrentData.collectedItemIDs.Contains(uniqueLocationID))
                {
                    SaveManager.Instance.CurrentData.collectedItemIDs.Add(uniqueLocationID);
                }

                SaveManager.Instance.SaveGame();
            }

            OnCollectiblePickedUp?.Invoke(collectedCount, MaxCollectibles);

            Destroy(gameObject);
        }
    }
}
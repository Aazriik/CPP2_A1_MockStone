using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [HideInInspector] public string uniqueLocationID;

    protected virtual void Start()
    {
        // Safety check: If this was already looted in a past save, destroy it before the player sees it
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.collectedItemIDs.Contains(uniqueLocationID))
        {
            Destroy(gameObject);
        }
    }

    protected void MarkAsCollectedAndDestroy()
    {
        if (SaveManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(uniqueLocationID) && !SaveManager.Instance.CurrentData.collectedItemIDs.Contains(uniqueLocationID))
            {
                SaveManager.Instance.CurrentData.collectedItemIDs.Add(uniqueLocationID);
            }
            SaveManager.Instance.SaveGame();
        }

        Destroy(gameObject);
    }
}
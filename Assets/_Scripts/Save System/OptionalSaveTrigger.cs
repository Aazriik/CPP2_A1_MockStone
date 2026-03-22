using UnityEngine;

public class OptionalSaveTrigger : MonoBehaviour
{
    public bool saveOnDestroy = false;

    public void TriggerManualSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("Optional manual save triggered by " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("SaveManager instance not found. Cannot save.");
        }
    }

    private void OnDestroy()
    {
        if (saveOnDestroy && SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
    }
}
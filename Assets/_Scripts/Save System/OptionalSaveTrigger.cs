using UnityEngine;

public class OptionalSaveTrigger : MonoBehaviour
{
    [Header("Save Settings")]
    [Tooltip("Check this to save when this object is destroyed (e.g., enemy killed).")]
    public bool saveOnDestroy = false;

    [Tooltip("Check this to save when the player walks into this object's trigger collider.")]
    public bool saveOnTriggerEnter = false;

    [Tooltip("If true, the trigger will only save once and then disable itself so it doesn't spam saves.")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

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

    private void OnTriggerEnter(Collider other)
    {
        // Check if trigger saving is enabled, it hasn't fired yet, and the colliding object is the player
        if (saveOnTriggerEnter && !hasTriggered && other.CompareTag("Player"))
        {
            TriggerManualSave();

            if (triggerOnlyOnce)
            {
                hasTriggered = true;
            }
        }
    }

    private void OnDestroy()
    {
        // gameObject.scene.isLoaded ensures it only saves during actual gameplay, 
        // not when Unity is unloading the scene or quitting the game.
        if (saveOnDestroy && SaveManager.Instance != null && gameObject.scene.isLoaded)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("Optional save triggered by the destruction of " + gameObject.name);
        }
    }
}
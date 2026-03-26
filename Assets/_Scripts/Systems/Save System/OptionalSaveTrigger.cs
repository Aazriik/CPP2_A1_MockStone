using UnityEngine;

public class OptionalSaveTrigger : MonoBehaviour
{
    [Header("Save Settings")]
    public bool saveOnDestroy = false;
    public bool saveOnTriggerEnter = false;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    public void TriggerManualSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        // gameObject.scene.isLoaded prevents saving during application quit or scene teardown
        if (saveOnDestroy && SaveManager.Instance != null && gameObject.scene.isLoaded)
        {
            SaveManager.Instance.SaveGame();
        }
    }
}
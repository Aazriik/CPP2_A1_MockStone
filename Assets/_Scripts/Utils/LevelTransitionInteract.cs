//Mockstone
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelTransitionInteract : MonoBehaviour, IInteract
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;   // Drag scene here
#endif

    [SerializeField] private string sceneName;        // Used at runtime

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif

    public void Interact(PlayerController player)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (SaveManager.Instance != null)
            {
                // 1. Record the new level name
                SaveManager.Instance.CurrentData.currentLevelName = sceneName;

                // 2. Save the game, but tell it NOT to record our current coordinates
                SaveManager.Instance.SaveGame(false);
            }

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"{name}: No scene assigned.");
        }
    }
}
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
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"{name}: No scene assigned.");
        }
    }
}

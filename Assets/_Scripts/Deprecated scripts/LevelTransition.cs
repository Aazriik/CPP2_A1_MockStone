//Mockstone
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour, IInteract
{
    [SerializeField] public string sceneName;

    public void Interact(PlayerController interactor)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene((sceneName));
        }
        else
        {
            Debug.LogError($"(name): name is empty. Please set the scene name in the inspector.");
        }
    }
}

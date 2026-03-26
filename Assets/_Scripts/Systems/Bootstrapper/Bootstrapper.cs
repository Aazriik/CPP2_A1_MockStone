using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The Build Index of the scene to load after bootstrapping.")]
    [SerializeField] private int firstSceneIndex = 1;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(firstSceneIndex);
    }
}
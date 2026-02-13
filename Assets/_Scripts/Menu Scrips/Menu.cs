using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    // Called by the UI Button OnClick to load the Hub scene
    public void OnStartButton()
    {
        SceneManager.LoadScene("Hub");
    }

    // Called by the UI Button OnClick to quit the application.
    // In the Editor this will stop play mode; in a built player it will quit the application.
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
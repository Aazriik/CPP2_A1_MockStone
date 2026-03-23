using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro; // Required to alter TextMeshPro UI elements

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    public TMP_Text startButtonText;
    private string levelToLoad = "Hub";

    private void Start()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.sav");

        // Dynamically change the button text based on whether a save file exists
        if (File.Exists(saveFilePath))
        {
            if (startButtonText != null)
            {
                startButtonText.text = "Continue";
            }

            // Look up the last saved level so the Continue button goes to the right place
            if (SaveManager.Instance != null && !string.IsNullOrEmpty(SaveManager.Instance.CurrentData.currentLevelName))
            {
                levelToLoad = SaveManager.Instance.CurrentData.currentLevelName;
            }
        }
        else
        {
            if (startButtonText != null)
            {
                startButtonText.text = "Start";
            }
        }
    }

    // Called by the UI Button OnClick to load the scene
    public void OnStartButton()
    {
        SceneManager.LoadScene(levelToLoad);
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
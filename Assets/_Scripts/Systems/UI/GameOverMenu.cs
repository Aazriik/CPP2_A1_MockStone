using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void LoadLastSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.LoadGame();
            SceneManager.LoadScene(SaveManager.Instance.CurrentData.currentLevelName);
        }
        else
        {
            SceneManager.LoadScene("Hub");
        }
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
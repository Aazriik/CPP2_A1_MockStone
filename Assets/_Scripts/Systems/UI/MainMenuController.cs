using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text playButtonText;

    [Header("Scene Settings")]
    [SerializeField] private string newGameSceneName = "Hub";

    private bool hasSaveData = false;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CheckForSaveData();
        }
    }

    private void CheckForSaveData()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.currentHealth != -1)
        {
            hasSaveData = true;
            if (playButtonText != null) playButtonText.text = "Continue";
        }
        else
        {
            hasSaveData = false;
            if (playButtonText != null) playButtonText.text = "Play";
        }
    }

    public void OnPlayButtonClicked()
    {
        if (UIManager.Instance != null) UIManager.Instance.HideAllMenus();

        if (hasSaveData && SaveManager.Instance != null)
        {
            SceneManager.LoadScene(SaveManager.Instance.CurrentData.currentLevelName);
        }
        else
        {
            SceneManager.LoadScene(newGameSceneName);
        }
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
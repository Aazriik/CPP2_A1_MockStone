using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menu Panels")]
    [SerializeField] private UIPanel mainMenuPanel;
    [SerializeField] private UIPanel pauseMenuPanel;
    [SerializeField] private UIPanel gameOverPanel;
    [SerializeField] private UIPanel warningPanel;
    [SerializeField] private GameObject hudLayer;
    [SerializeField] private GameObject interactionContainer;

    private UIPanel currentActivePanel;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator Start()
    {
        yield return null;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPauseEvent += HandlePauseInput;

            InputManager.Instance.SetPlayerControlsActive(true);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        try
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnPauseEvent -= HandlePauseInput;
            }
        }
        catch { }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (scene.buildIndex == 1)
        {
            HideAllMenus();
            ShowPanel(mainMenuPanel);

            if (hudLayer != null) hudLayer.SetActive(false);

            if (InputManager.Instance != null) InputManager.Instance.SetPlayerControlsActive(false);
        }
        else if (scene.buildIndex > 1)
        {
            HideAllMenus();

            if (hudLayer != null) hudLayer.SetActive(true);

            if (InputManager.Instance != null) InputManager.Instance.SetPlayerControlsActive(true);
        }
    }

    private void HandlePauseInput(bool pressed)
    {
        if (pressed && currentActivePanel != mainMenuPanel && currentActivePanel != gameOverPanel)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        InputManager.Instance.SetPlayerControlsActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowPanel(pauseMenuPanel);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        InputManager.Instance.SetPlayerControlsActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HideAllMenus();
    }

    public void SetInteractionPromptVisible(bool isVisible)
    {
        if (interactionContainer != null && interactionContainer.activeSelf != isVisible)
        {
            interactionContainer.SetActive(isVisible);
        }
    }

    public void ShowPanel(UIPanel panelToShow)
    {
        if (currentActivePanel != null) currentActivePanel.Hide();

        panelToShow.Show();
        currentActivePanel = panelToShow;
    }

    public void HideAllMenus()
    {
        if (mainMenuPanel != null) mainMenuPanel.Hide();
        if (pauseMenuPanel != null) pauseMenuPanel.Hide();
        if (gameOverPanel != null) gameOverPanel.Hide();
        if (warningPanel != null) warningPanel.Hide();
        currentActivePanel = null;
    }

    public void LoadLastSave()
    {
        Time.timeScale = 1f;

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

    public void ReturnToHub()
    {
        Time.timeScale = 1f;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentData.currentHealth = -1;
            SaveManager.Instance.CurrentData.currentStamina = -1f;
            SaveManager.Instance.CurrentData.hasSavedPosition = false;
            SaveManager.Instance.CurrentData.currentLevelName = "Hub";
        }

        SceneManager.LoadScene("Hub");
    }

    public void ConfirmQuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void ShowGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowPanel(gameOverPanel);
    }

    public void ShowWarningPanel() => ShowPanel(warningPanel);
    public void ShowPauseMenu() => ShowPanel(pauseMenuPanel);
}
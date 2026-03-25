using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Pause : MonoBehaviour
{
    [Tooltip("Drag the Pause menu root GameObject here (panel containing the pause UI).")]
    public GameObject pauseMenu;

    [Tooltip("Name of the Hub scene to load")]
    public string hubSceneName = "Hub";

    [Tooltip("Name of the Main Menu scene to load")]
    public string mainMenuSceneName = "Main Menu";

    [Tooltip("Enable extra debug logging for pause/resume actions")]
    public bool debugMode = false;

    bool isPaused;
    // default fixed delta time (Unity default is 0.02)
    float defaultFixedDelta = 0.02f;

    void Awake()
    {
        // Defensive: ensure the game starts running
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log("[Pause] Awake: Time.timeScale set to 1, fixedDeltaTime reset");
    }

    void Start()
    {
        // Ensure pause UI is hidden at start
        ResumeGame();
    }

    void Update()
    {
        // Toggle pause with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (debugMode) Debug.Log($"[Pause][Debug] Toggle requested. isPaused={isPaused}, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // Called by the "Back to game" button
    public void OnBackToGameButton()
    {
        ResumeGame();
    }

    // Called by the "Back to Hub" button
    public void OnBackToHubButton()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log($"[Pause] OnBackToHubButton: loading {hubSceneName}, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        SceneManager.LoadScene(hubSceneName);
    }

    // Called by the "Main Menu" button
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log($"[Pause] OnMainMenuButton: loading {mainMenuSceneName}, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Show pause UI and stop time
    public void PauseGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        // scale fixedDeltaTime with timeScale (will become 0)
        Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // When running inside the Unity Editor, also pause the Editor play mode so
        // editor-driven updates are stopped (useful while testing in Editor).
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#endif

        Debug.Log($"[Pause] Paused: timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        if (debugMode)
        {
            Debug.Log($"[Pause][Debug] pauseMenuActive={(pauseMenu!=null?pauseMenu.activeSelf.ToString():"null")}, Cursor.visible={Cursor.visible}, Cursor.lockState={Cursor.lockState}");
        }
    }

    // Hide pause UI and resume time
    public void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // When running inside the Unity Editor, unpause the Editor play mode.
#if UNITY_EDITOR
        EditorApplication.isPaused = false;
#endif

        Debug.Log($"[Pause] Resumed: timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        if (debugMode)
        {
            Debug.Log($"[Pause][Debug] pauseMenuActive={(pauseMenu!=null?pauseMenu.activeSelf.ToString():"null")}, Cursor.visible={Cursor.visible}, Cursor.lockState={Cursor.lockState}");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Tooltip("Drag the Game Over menu root GameObject here (panel containing the game over UI).")]
    public GameObject gameOverMenu;

    [Tooltip("Name of the Hub scene to load")]
    public string hubSceneName = "Hub";

    [Tooltip("Name of the Main Menu scene to load")]
    public string mainMenuSceneName = "Main Menu";

    // default fixed delta time (Unity default is 0.02)
    float defaultFixedDelta = 0.02f;
    bool isGameOver;

    void Start()
    {
        // Ensure game over UI is hidden at start
        HideGameOver();
    }

    // Call this from your player/health system when the player dies
    public void ShowGameOver()
    {
        if (gameOverMenu != null) gameOverMenu.SetActive(true);

        Time.timeScale = 0f;
        Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
        isGameOver = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log($"[GameOver] Shown: timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
    }

    // Hide the GameOver UI (not typically used in a true game-over flow, but useful for testing)
    public void HideGameOver()
    {
        if (gameOverMenu != null) gameOverMenu.SetActive(false);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        isGameOver = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Called by the "Restart Level" button
    public void OnRestartLevelButton()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log($"[GameOver] OnRestartLevelButton: reloading active scene, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called by the "Back to Hub" button
    public void OnBackToHubButton()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log($"[GameOver] OnBackToHubButton: loading {hubSceneName}, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        SceneManager.LoadScene(hubSceneName);
    }

    // Called by the "Main Menu" button
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        Debug.Log($"[GameOver] OnMainMenuButton: loading {mainMenuSceneName}, timescale={Time.timeScale}, fixedDelta={Time.fixedDeltaTime}");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public string currentLevelName = "Hub";
    public int totalCollectedItems;
    public List<string> collectedItemIDs = new List<string>();

    // -1 indicates a fresh save, prompting the game to use default max values
    public int currentHealth = -1;
    public float currentStamina = -1f;

    // --- Position Data ---
    public bool hasSavedPosition = false;
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
}
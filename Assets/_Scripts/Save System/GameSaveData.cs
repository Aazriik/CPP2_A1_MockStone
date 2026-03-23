using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // Tracks the exact scene name the player was last in
    public string currentLevelName = "Hub";

    // The running total of collectibles found across the entire game
    public int totalCollectedItems;

    // A list of the specific spawn points that have already been looted
    public List<string> collectedItemIDs = new List<string>();

    // -1 tells the game this is a fresh save file and to use the default MaxHealth
    public int currentHealth = -1;
}
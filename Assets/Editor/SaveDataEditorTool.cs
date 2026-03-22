using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveDataEditorTool
{
    // Adds a clickable button to the Unity Editor toolbar at the top
    [MenuItem("Tools/Delete Save Data")]
    public static void DeleteSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamedata.sav");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted successfully from: " + path);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + path);
        }
    }
}
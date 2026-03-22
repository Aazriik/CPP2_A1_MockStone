using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class SaveManager : MonoBehaviour
{
    // The Singleton instance allows any script to access the save data easily
    public static SaveManager Instance { get; private set; }

    // The active save data that your game will read from and write to in memory
    public GameSaveData CurrentData;

    private string saveFilePath;

    // 16-character keys for 128-bit AES encryption. 
    private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("1234567890123456");
    private readonly byte[] encryptionIV = Encoding.UTF8.GetBytes("1234567890123456");

    private void Awake()
    {
        // Singleton pattern enforcement to ensure only one manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keeps the manager and its data alive when loading new scenes
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.sav");

        // Prints the save location to the console for easy debugging
        Debug.Log("Save path: " + Application.persistentDataPath);

        CurrentData = new GameSaveData();
        LoadGame();
    }

    public void SaveGame()
    {
        // Convert the data object to a JSON string, encrypt it, and write to disk
        string json = JsonUtility.ToJson(CurrentData);
        string encryptedData = EncryptAES(json);
        File.WriteAllText(saveFilePath, encryptedData);
        Debug.Log("Game Saved Securely.");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                // Read the file, decrypt it, and overwrite CurrentData
                string encryptedData = File.ReadAllText(saveFilePath);
                string decryptedJson = DecryptAES(encryptedData);
                CurrentData = JsonUtility.FromJson<GameSaveData>(decryptedJson);
                Debug.Log("Game Loaded Successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Save file corrupted or decryption failed: " + e.Message);
                CurrentData = new GameSaveData();
            }
        }
        else
        {
            Debug.Log("No save file found. Starting fresh.");
        }
    }

    // Standard AES encryption logic
    private string EncryptAES(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = encryptionKey;
            aesAlg.IV = encryptionIV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    // Standard AES decryption logic
    private string DecryptAES(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = encryptionKey;
            aesAlg.IV = encryptionIV;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
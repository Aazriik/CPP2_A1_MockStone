using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class SaveManager : Singleton<SaveManager>
{
    public GameSaveData CurrentData;
    private string saveFilePath;

    // 16-character keys for 128-bit AES encryption
    private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("1234567890123456");
    private readonly byte[] encryptionIV = Encoding.UTF8.GetBytes("1234567890123456");

    protected override void Awake()
    {
        base.Awake(); // Executes the Singleton hierarchy protection

        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.sav");
        CurrentData = new GameSaveData();
        LoadGame();
    }

    public void SaveGame(bool savePosition = true)
    {
        if (savePosition)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                CurrentData.playerPosX = player.transform.position.x;
                CurrentData.playerPosY = player.transform.position.y;
                CurrentData.playerPosZ = player.transform.position.z;
                CurrentData.hasSavedPosition = true;
            }
        }
        else
        {
            // Wipe the position flag so the player loads at the new scene's default spawn
            CurrentData.hasSavedPosition = false;
        }

        string json = JsonUtility.ToJson(CurrentData);
        string encryptedData = EncryptAES(json);
        File.WriteAllText(saveFilePath, encryptedData);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string encryptedData = File.ReadAllText(saveFilePath);
                string decryptedJson = DecryptAES(encryptedData);
                CurrentData = JsonUtility.FromJson<GameSaveData>(decryptedJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save file corrupted or decryption failed: {e.Message}");
                CurrentData = new GameSaveData();
            }
        }
    }

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
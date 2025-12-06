using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int rows;
    public int columns;
    public List<CardData> cards = new List<CardData>();
    public ScoreData scoreData;
    public float gameTime;
    public int moveCount;
    public int matchesFound;
}

[System.Serializable]
public class CardData
{
    public int cardId;
    public bool isFlipped;
    public bool isMatched;
    public int siblingIndex; // Position in grid
}

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager instance;
    private string saveFilePath;
    private const string SAVE_FILE_NAME = "cardgame_save.json";
    
    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveLoadManager>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        Debug.Log($"Save file path: {saveFilePath}");
    }
    
    public void SaveGame(GameData gameData)
    {
        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    
    public GameData LoadGame()
    {
        if (!HasSaveFile())
        {
            Debug.Log("No save file found.");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded successfully!");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }
    
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }
    
    public void DeleteSaveFile()
    {
        if (HasSaveFile())
        {
            try
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save file: {e.Message}");
            }
        }
    }
    
    // Auto-save with PlayerPrefs as backup
    public void SaveGameToPlayerPrefs(GameData gameData)
    {
        try
        {
            string json = JsonUtility.ToJson(gameData);
            PlayerPrefs.SetString("CardGameSave", json);
            PlayerPrefs.Save();
            Debug.Log("Game saved to PlayerPrefs!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save to PlayerPrefs: {e.Message}");
        }
    }
    
    public GameData LoadGameFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("CardGameSave"))
        {
            return null;
        }
        
        try
        {
            string json = PlayerPrefs.GetString("CardGameSave");
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load from PlayerPrefs: {e.Message}");
            return null;
        }
    }
    
    public void ClearPlayerPrefsSave()
    {
        if (PlayerPrefs.HasKey("CardGameSave"))
        {
            PlayerPrefs.DeleteKey("CardGameSave");
            PlayerPrefs.Save();
        }
    }
}

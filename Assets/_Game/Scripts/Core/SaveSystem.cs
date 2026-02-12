using UnityEngine;
using System.IO; // Required for file handling

public static class SaveSystem
{
    private static string _fileName = "savefile.json";

    public static void Save(PlayerData data)
    {
        // 1. Convert Data to Text (JSON)
        string json = JsonUtility.ToJson(data, true);

        // 2. Write to file
        string path = Path.Combine(Application.persistentDataPath, _fileName);
        File.WriteAllText(path, json);

        Debug.Log($"Game Saved to: {path}");
    }

    public static PlayerData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, _fileName);

        if (File.Exists(path))
        {
            // 1. Read Text
            string json = File.ReadAllText(path);

            // 2. Convert Text back to Data
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            Debug.Log("No save file found. Creating new one.");
            return new PlayerData(); // Return fresh data
        }
    }
}
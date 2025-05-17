using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveSystem : MonoBehaviour
{
    private static readonly string SAVE_FILENAME = "userData.dat";
    private static string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILENAME);

    public static void SaveUserData(UserData userData)
    {
        string data = JsonConvert.SerializeObject(userData, Formatting.Indented);
        File.WriteAllText(SavePath, data);
    }

    public static UserData LoadUserData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"Save file not found. Creating a new one at {SavePath}.");
            UserData newUserData = new UserData();
            newUserData.InitializeDefaultValues();
            return newUserData;
        }
        
        Debug.Log($"Loading user data from {SavePath}.");
        string data = File.ReadAllText(SavePath);
        UserData userData = JsonConvert.DeserializeObject<UserData>(data);

        if (userData.levelProgress == null)
        {
            userData.levelProgress = new Dictionary<int, LevelProgress>();
        }

        return userData;
    }
}
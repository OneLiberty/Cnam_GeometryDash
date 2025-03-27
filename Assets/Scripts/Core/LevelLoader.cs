using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour {
    private Dictionary<string, GameObject> prefabsDictionnary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        LoadAllPrefabs();
    }

    // test 
    void Start()
    {
        // Load level 1 automatically for testing
        LoadLevel(1);
    }

    private void LoadAllPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/"); 

        foreach (GameObject prefab in prefabs)
        {
            prefabsDictionnary[prefab.name] = prefab;
            Debug.Log("Loaded prefab: " + prefab.name);
        }

        Debug.Log("All prefabs loaded: " + prefabsDictionnary.Count);
    }

    public void LoadLevel(int levelNumber)
    {
        string LevelPath = Path.Combine(Application.dataPath, "Levels", $"level{levelNumber}.json");
        if (!File.Exists(LevelPath))
        {
            Debug.LogError($"Level file not found: {LevelPath}");
            return;
        }

        Debug.Log($"Loading level from: {LevelPath}");

        string json = File.ReadAllText(LevelPath);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        // don't forger the AudioManager here

        foreach (var gameObj in levelData.levelObjects) 
        {
            if (prefabsDictionnary.TryGetValue(gameObj.type, out GameObject prefab))
            {
                Instantiate(
                    prefab,
                    gameObj.position,
                    Quaternion.Euler(0, 0, gameObj.rotation)
                );

            }      
        }
    }
}
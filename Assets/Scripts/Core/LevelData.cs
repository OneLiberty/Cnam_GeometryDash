using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData {
    public string name;
    public int levelNumber;
    public int difficulty; // 1 to 10
    public string musicFile;
    public List<LevelObjectData> levelObjects = new List<LevelObjectData>();
}

[Serializable]
public class LevelObjectData {
    public string type;
    public Vector2 position;
    public float rotation; 
    public string anchor = "bottom"; // "bottom", "top", "center"
    // maybe other properties, we'll see. 
}
using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnchorType 
{
    Bottom, 
    Center, 
    Top
}

[Serializable]
public class LevelData
{
    public string name;
    public int levelNumber;
    public int difficulty; // 1 to 10
    public string musicFile;
    public string groundColor;
    public string backgroundColor;
    public List<LevelObjectData> levelObjects = new List<LevelObjectData>();
}

[Serializable]
public class LevelObjectData
{
    public string type;
    public Vector2 position;
    public float rotation;
    public AnchorType anchor = AnchorType.Bottom; // "bottom", "top", "center"
    // maybe other properties, we'll see. 
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold level data for the levels. This includes the level name, number, difficulty, music file, colors, and objects in the level.
/// Each level will have a LevelData that will itself contain a list of LevelObjectData representing each object in the level.
/// LevelObjectData will contain the type of object, its position, rotation, and anchor point.
/// </summary>

/// A level is a JSON file structured like this: 
/// file : level+ID.json                                 // MANDATORY, ID is the level number, and should not the same as another level.
/// /// {
/// ///     "name": "Level 1",
/// ///     "levelNumber": 1,
/// ///     "difficulty": 5,
/// ///     "musicFile": "level1.mp3",
/// ///     "groundColor": "#FF0000",
/// ///     "backgroundColor": "#00FF00",
/// ///     "levelObjects": [
/// ///         {
/// ///             "type": "tile",
/// ///             "position": { "x": 0, "y": 0 },
/// ///             "rotation": 0,
/// ///             "anchor": "bottom"                  // optional, default is "bottom"
/// ///         }
/// ///     ]
/// /// }
/// 

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
    public string anchor = "bottom"; // "bottom", "top", "center"
    // maybe other properties, we'll see. 
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that holds user data. This doesn't account for multiple user, so it will be a single user data file.
/// It contains informations about the input binding, and small data such as total jumps/death. 
/// More importantly, it contains a dictionary of level progress containing level specific data.
/// </summary>
[Serializable]
public class UserData
{
    public int totalJumps = 0;
    public int totalDeath = 0;

    public Dictionary<int, LevelProgress> levelProgress = new Dictionary<int, LevelProgress>();

    // No default values - just storage
    public KeyCode jumpButton_0;
    public KeyCode jumpButton_1;
    public KeyCode pauseButton;
    public KeyCode restartButton;

    public KeyCode editorUpButton;
    public KeyCode editorDownButton;
    public KeyCode editorLeftButton;
    public KeyCode editorRightButton;
    public KeyCode editorRotationButton;
    public KeyCode editorAnchorButton;
    public KeyCode editorRemoveButton;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}

[Serializable]
public class LevelProgress
{
    public bool isCompleted = false;
    public int jumps = 0;
    public int deaths = 0;
    public float bestScore = 0f; // best completion percentage
}

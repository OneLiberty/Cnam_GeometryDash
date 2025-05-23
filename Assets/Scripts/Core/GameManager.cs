using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory, LevelEditor }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    public int CurrentLevel { get; private set; }
    public string CurrentLevelPath { get; private set; }
    public float completionPercentage = 0f;
    public float endPosition = 0f;

    private float lastActionTime = 0f;
    private const float actionCooldown = 0.1f;  

    private UnityAction<Scene, LoadSceneMode> onSceneLoaded;
    public InputSettings inputSettings;
    public UserData userData { get ; private set ;}

    public event Action<int> OnJump;
    public event Action<int> OnDeath;
    public event Action<int> OnLevelComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            userData = SaveSystem.LoadUserData();
            
            QualitySettings.vSyncCount = userData.vSyncEnabled ? 1 : 0;
            Application.targetFrameRate = userData.vSyncEnabled ? -1 : userData.targetFrameRate;
            
            if (inputSettings != null)
            {
                inputSettings.LoadInputSettings(userData);
            }
            
            // increments the total and level progress for jumps and deaths
            OnJump += (level) => { 
                userData.totalJumps++;
                if (!userData.levelProgress.ContainsKey(level))
                {
                    userData.levelProgress[level] = new LevelProgress();
                }
                userData.levelProgress[level].jumps++;
                SaveData();
            };

            OnDeath += (level) => { 
                userData.totalDeath++;
                if (!userData.levelProgress.ContainsKey(level))
                {
                    userData.levelProgress[level] = new LevelProgress();
                }
                userData.levelProgress[level].deaths++;
                SaveData();
             };

             OnLevelComplete += (level) => {
                if (!userData.levelProgress.ContainsKey(level))
                {
                    userData.levelProgress[level] = new LevelProgress();
                }
                LevelProgress progress = userData.levelProgress[level];
                progress.isCompleted = true;
                progress.bestScore = 100; // we can safely assume that if the player completed the level, they got 100%
                SaveData();
            };
            SaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLevel(string levelPath)
    {
        if (!File.Exists(levelPath))
        {
        Debug.LogError($"Level file not found: {levelPath}");
        return;
        }

        CurrentLevelPath = levelPath;
        CurrentLevel = JsonUtility.FromJson<LevelData>(File.ReadAllText(levelPath)).levelNumber;
        CurrentGameState = GameState.Playing;

        if (onSceneLoaded != null)
        {
            SceneManager.sceneLoaded -= onSceneLoaded;
        }

        onSceneLoaded = (scene, mode) =>
        {
            if (scene.name == "Level")
            {
                
                LevelLoader levelLoader = FindFirstObjectByType<LevelLoader>();
                if (levelLoader != null)
                {
                    levelLoader.LoadLevel(levelPath);
                }

                endPosition = levelLoader.endPosition;
            }

            SceneManager.sceneLoaded -= onSceneLoaded;
        };

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.LoadScene("Level");
    } 

    public void PauseGame()
    {
        // since we can't pause outside of a level, just get the levelUiManger here
        LevelUIManager levelUi = FindFirstObjectByType<LevelUIManager>();

        if (CurrentGameState == GameState.Paused) {
            levelUi.ShowPausePanel(false);
            Time.timeScale = 1f; // Resume the game
            CurrentGameState = GameState.Playing;
            AudioManager.Instance.musicSource.UnPause();
        } else {
            levelUi.ShowPausePanel(true);
            Time.timeScale = 0f; // Pause the game
            CurrentGameState = GameState.Paused;
            AudioManager.Instance.musicSource.Pause();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Resume the game
        StartLevel(CurrentLevelPath);
    }
    

    public void ReturnToMainMenu()
    {
        CurrentGameState = GameState.MainMenu;
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("Main Menu");
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            AudioManager.Instance.SetMusicClip("menuLoop");
        };
    }

    public void LoadLevelEditor() 
    {
        CurrentGameState = GameState.LevelEditor;
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("Level Editor");
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            AudioManager.Instance.SetMusicClip("StayInsideMe");
        };
    }

    public void RecordJump()
    {
        if (Time.time - lastActionTime >= actionCooldown) {
            lastActionTime = Time.time;
            OnJump?.Invoke(CurrentLevel);
        }
    }

    public void RecordDeath()
    {
        OnDeath?.Invoke(CurrentLevel);
    }

    public void RecordLevelCompleted()
    {
        OnLevelComplete?.Invoke(CurrentLevel);
        CurrentGameState = GameState.Victory;
    }

    public void UpdateCompletion(float completion)
    {
        if (completion > userData.levelProgress[CurrentLevel].bestScore)
        {
            userData.levelProgress[CurrentLevel].bestScore = completion;
        }
        completionPercentage = completion;
        if (!userData.levelProgress.ContainsKey(CurrentLevel))
        {
            userData.levelProgress[CurrentLevel] = new LevelProgress();
        }
        LevelProgress progress = userData.levelProgress[CurrentLevel];
        if (completion > progress.bestScore)
        {
            progress.bestScore = completion;
            SaveData();
        }
    }

    public void SaveData()
    {
        inputSettings.ExportToUserData(userData);
        
        if (AudioManager.Instance != null)
        {
            userData.musicVolume = AudioManager.Instance.musicSource.volume;
            userData.sfxVolume = AudioManager.Instance.sfxSource.volume;
        }
        
        SaveSystem.SaveUserData(userData);
    }
}
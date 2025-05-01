using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, LevelSelection, Playing, Paused, GameOver, Victory }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    private UnityAction<Scene, LoadSceneMode> onSceneLoaded;

    [Header("Input Settings")]
    public InputSettings inputSettings;

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartLevel(int levelNumber)
    {
        Debug.Log($"Starting level {levelNumber}");
        CurrentLevel = levelNumber;
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
                    levelLoader.LoadLevel(levelNumber);
                }
            
            }

            SceneManager.sceneLoaded -= onSceneLoaded;
        };

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.LoadScene("Level");
    } 

    public void PauseGame()
    {
        if (CurrentGameState == GameState.Paused) {
            Time.timeScale = 1f; // Resume the game
            CurrentGameState = GameState.Playing;
            AudioManager.Instance.musicSource.UnPause();
        } else {
            Time.timeScale = 0f; // Pause the game
            CurrentGameState = GameState.Paused;
            AudioManager.Instance.musicSource.Pause();
        }
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

    public void GoToLevelSelection()
    {
        CurrentGameState = GameState.LevelSelection;
        Time.timeScale = 1f; // Resume the game
        // scene management
    }

}
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    public int CurrentLevel { get; private set; }
    public float completionPercentage = 0f;
    public float endPosition = 0f;

    private UnityAction<Scene, LoadSceneMode> onSceneLoaded;

    [Header("Input Settings")]
    public InputSettings inputSettings;

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
        StartLevel(CurrentLevel);
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

}
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, LevelSelection, Playing, Paused, GameOver, Victory }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
private UnityAction<Scene, LoadSceneMode> onSceneLoaded;

[Header("Managers")]
    public AudioManager AudioManager { get { return AudioManager.Instance; } }
    private MainMenuManager mainMenuManager;
    private LevelUIManager levelUIManager;

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
    
    private void Start()
    {
        // we should always start on the main menu but just in case... 
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            mainMenuManager = gameObject.AddComponent<MainMenuManager>();
        } 
        else if (SceneManager.GetActiveScene().name == "Level")
        {
            levelUIManager = gameObject.AddComponent<LevelUIManager>();
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
                if (levelUIManager == null) 
                {
                    levelUIManager = gameObject.AddComponent<LevelUIManager>();
                }
                
            LevelLoader levelLoader = FindFirstObjectByType<LevelLoader>();
            if (levelLoader != null)
            {
                levelLoader.LoadLevel(levelNumber);
            }

                levelUIManager.InitializePanels();
            }

            SceneManager.sceneLoaded -= onSceneLoaded;
        };

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.LoadScene("Level");
    } 

    public void PauseGame()
    {
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f; // Pause the game
        // scene management
    }

    public void ReturnToMainMenu()
    {
        // detroy so we can create a new one when loading another level
        if (levelUIManager != null)
        {
            Destroy(levelUIManager);
            levelUIManager = null;
        }

        CurrentGameState = GameState.MainMenu;
        Time.timeScale = 1f; // Resume the game
        
        SceneManager.LoadScene("Main Menu");
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            UIManager.ReinitializePanels();
            UIManager.ShowMainMenu();
            AudioManager.SetMusicClip("menuLoop");
            mainMenuManager.InitializePanels();
        };
    }

    public void GoToLevelSelection()
    {
        CurrentGameState = GameState.LevelSelection;
        Time.timeScale = 1f; // Resume the game
        // scene management
    }

}
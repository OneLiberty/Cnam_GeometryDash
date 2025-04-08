using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, LevelSelection, Playing, Paused, GameOver, Victory }
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    public AudioManager AudioManager { get { return AudioManager.Instance; } }
    public UIManager UIManager { get { return UIManager.Instance; } }

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
        CurrentLevel = levelNumber;
        CurrentGameState = GameState.Playing;

        SceneManager.LoadScene("Level");
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            LevelLoader levelLoader = FindFirstObjectByType<LevelLoader>();
            if (levelLoader != null)
            {
                levelLoader.LoadLevel(levelNumber);
            }
        };
    } 

    public void PauseGame()
    {
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f; // Pause the game
        // scene management
    }

    public void ReturnToMainMenu()
    {
        CurrentGameState = GameState.MainMenu;
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("Main Menu");
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            AudioManager.SetMusicClip("menuLoop");
        };
    }

    public void GoToLevelSelection()
    {
        CurrentGameState = GameState.LevelSelection;
        Time.timeScale = 1f; // Resume the game
        // scene management
    }

}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region UIManager
public abstract class UIManager : MonoBehaviour 
{
    protected Canvas uiCanvas;

    protected virtual void Awake() {
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        if (uiCanvas == null) {
            Debug.LogError("UICanvas not found in the scene. [Awake]");
        }
    }

    protected virtual void Start() {
        Debug.Log("UIManager started.");
        InitializePanels();
    }

    public abstract void InitializePanels();

}
#endregion

#region MainMenuManager
public class MainMenuManager : UIManager
{
    [Header("Panels")]
    private GameObject mainMenuPanel;
    private GameObject levelSelectionPanel;
    private GameObject settingsPanel;

    public override void InitializePanels()
    {
        if (uiCanvas == null) {
            uiCanvas = GameObject.Find("UICanvas")?.GetComponent<Canvas>();
            if (uiCanvas == null) {
                Debug.LogError("UICanvas not found in the scene. [Init]");
                return; // Exit early if canvas isn't found
            }
        }
        
        mainMenuPanel = uiCanvas.transform.Find("MainMenu")?.gameObject;
        levelSelectionPanel = uiCanvas.transform.Find("LevelSelection")?.gameObject;
        settingsPanel = uiCanvas.transform.Find("Settings")?.gameObject;
    
        if (mainMenuPanel != null) InitializeMainMenu();
        if (levelSelectionPanel != null) InitializeLevelSelection(); 
        if (settingsPanel != null) InitializeSettings();

        if (mainMenuPanel != null) {
            ShowMainMenu();
        }   
    }

    public void ShowMainMenu() {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowLevelSelection() {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings() {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void InitializeSettings() {
        Button backButton = settingsPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });

        // Jump button 0
        Button button0 = settingsPanel.transform.Find("JumpInput1").GetComponent<Button>();
        TextMeshProUGUI button0text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button0text.text = GameManager.Instance.inputSettings.jumpButton_0.ToString();
        button0.onClick.AddListener(() => {
            // TODO : 
            // Open key binding menu for jump button 0
            // Set the key binding to GameManager.Instance.inputSettings.jumpButton_0
        });


        // Jump button 1
        Button button1 = settingsPanel.transform.Find("JumpInput2").GetComponent<Button>();
        TextMeshProUGUI button1text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button1text.text = GameManager.Instance.inputSettings.jumpButton_1.ToString();

        button1.onClick.AddListener(() => {
            // TODO :
            // Open key binding menu for jump button 1
            // Set the key binding to GameManager.Instance.inputSettings.jumpButton_1
        });

        Slider musicSlider = settingsPanel.transform.Find("MusicSlider").GetComponent<Slider>();
        Slider sfxSlider = settingsPanel.transform.Find("SfxSlider").GetComponent<Slider>();
        
        musicSlider.onValueChanged.AddListener((value) => {
            GameManager.Instance.AudioManager.SetMusicVolume(value);
        });
        sfxSlider.onValueChanged.AddListener((value) => {
            GameManager.Instance.AudioManager.SetSFXVolume(value);
        });
    }

    private void InitializeMainMenu() {
        Button playButton = mainMenuPanel.transform.Find("Start").GetComponent<Button>();
        Button settingsButton = mainMenuPanel.transform.Find("Settings").GetComponent<Button>();
        Button quitButton = mainMenuPanel.transform.Find("Quit").GetComponent<Button>();

        playButton.onClick.AddListener(() => {
            ShowLevelSelection();
        });

        settingsButton.onClick.AddListener(() => {
            ShowSettings();
        });

        quitButton.onClick.AddListener(() => {
            QuitGame();
        });
    }
    
    private void InitializeLevelSelection() {
        Button backButton = levelSelectionPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });

        Button level1Button = levelSelectionPanel.transform.Find("Level1").GetComponent<Button>();
        level1Button.onClick.AddListener(() => {
            GameManager.Instance.StartLevel(1);
        });
        Button level2Button = levelSelectionPanel.transform.Find("Level2").GetComponent<Button>();
        level2Button.onClick.AddListener(() => {
            GameManager.Instance.StartLevel(2);
        });
    }

    private void QuitGame() {
        Application.Quit();
    }
}
#endregion

#region LevelUIManager
public class LevelUIManager : UIManager
{
    private GameObject pausePanel;
    private GameObject gameOverPanel;
    private GameObject levelCompletePanel;
    private GameObject levelUI;
    
    public override void InitializePanels()
    {
        pausePanel = uiCanvas.transform.Find("PausePanel")?.gameObject;
        gameOverPanel = uiCanvas.transform.Find("GameOverPanel")?.gameObject;
        levelCompletePanel = uiCanvas.transform.Find("LevelCompletePanel")?.gameObject;
        levelUI = uiCanvas.transform.Find("LevelUI")?.gameObject;

        if (pausePanel != null) InitializePausePanel();
        // if (gameOverPanel != null) InitializeGameOverPanel();
        // if (levelCompletePanel != null) InitializeLevelCompletePanel();
    }

    private void InitializePausePanel() {
        pausePanel.SetActive(false);
    }

    public void ShowPausePanel(bool gamePaused) {
        pausePanel.SetActive(gamePaused);
        // gameOverPanel.SetActive(false);
        // levelCompletePanel.SetActive(false);
        // levelUI.SetActive(false);
    }

}
#endregion
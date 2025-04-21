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
    private GameObject askInputPanel;
    
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
        askInputPanel = uiCanvas.transform.Find("AskInput")?.gameObject;
    
        // it's easier to edit them in the editor when they are not stacked on top of each other
        // so we just reset their position to 0,0,0 here
        if (mainMenuPanel != null) { InitializeMainMenu();} 
        if (levelSelectionPanel != null) { InitializeLevelSelection();} 
        if (settingsPanel != null) { InitializeSettings();}

        if (mainMenuPanel != null) {
            ShowMainMenu();
        }   
    }

    private void InitializeMainMenu() 
    {
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
    
    private void InitializeLevelSelection() 
    {
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

    private void InitializeSettings() 
    {
        //Back to main menu button
        Button backButton = settingsPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });

        // Jump button 0
        Button button0 = settingsPanel.transform.Find("Jump/JumpInput0").GetComponent<Button>();
        TextMeshProUGUI button0text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button0text.text = GameManager.Instance.inputSettings.jumpButton_0.ToString();
        button0.onClick.AddListener(() => {
            askInputPanel.SetActive(true);
            GameManager.Instance.inputSettings.ListenForInput(key => {
                GameManager.Instance.inputSettings.SetJumpBtn(0, key);
                button0text.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });


        // Jump button 1
        Button button1 = settingsPanel.transform.Find("Jump/JumpInput1").GetComponent<Button>();
        TextMeshProUGUI button1text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button1text.text = GameManager.Instance.inputSettings.jumpButton_1.ToString();
        button1.onClick.AddListener(() => {
            askInputPanel.SetActive(true);
            GameManager.Instance.inputSettings.ListenForInput(key => {
                GameManager.Instance.inputSettings.SetJumpBtn(1, key);
                button1text.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });

        // Restart button
        Button restartButton = settingsPanel.transform.Find("Restart/RestartInput").GetComponent<Button>();
        TextMeshProUGUI restartButtonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartButtonText.text = GameManager.Instance.inputSettings.restartButton.ToString();
        restartButton.onClick.AddListener(() => {
            askInputPanel.SetActive(true);
            GameManager.Instance.inputSettings.ListenForInput(key => {
                GameManager.Instance.inputSettings.SetRestartBtn(key);
                restartButtonText.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });

        // Pause button
        Button pauseButton = settingsPanel.transform.Find("Pause/PauseInput").GetComponent<Button>();
        TextMeshProUGUI pauseButtonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
        pauseButtonText.text = GameManager.Instance.inputSettings.pauseButton.ToString();
        pauseButton.onClick.AddListener(() => {
            askInputPanel.SetActive(true);
            GameManager.Instance.inputSettings.ListenForInput(key => {
                GameManager.Instance.inputSettings.SetPauseBtn(key);
                pauseButtonText.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });

        Slider musicSlider = settingsPanel.transform.Find("Music&Sfx/MusicSlider").GetComponent<Slider>();
        Slider sfxSlider = settingsPanel.transform.Find("Music&Sfx/SfxSlider").GetComponent<Slider>();
            
        musicSlider.onValueChanged.AddListener((value) => {
            GameManager.Instance.AudioManager.SetMusicVolume(value);
        });
        sfxSlider.onValueChanged.AddListener((value) => {
            GameManager.Instance.AudioManager.SetSFXVolume(value);
        });
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
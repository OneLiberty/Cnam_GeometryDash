using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    
        if (mainMenuPanel != null) { InitializeMainMenu();} 
        if (levelSelectionPanel != null) { InitializeLevelSelection();} 
        if (settingsPanel != null) { InitializeSettings();}

        if (mainMenuPanel != null) {
            ShowMainMenu();
        }   
    }

#region Main Menu
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
#endregion
#region Level Selection
    private void InitializeLevelSelection() 
    {
        Button backButton = levelSelectionPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });
    }

#endregion
#region Settings
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
                GameManager.Instance.inputSettings.jumpButton_0 = key;
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
                GameManager.Instance.inputSettings.jumpButton_1 = key;
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
                GameManager.Instance.inputSettings.restartButton = key;
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
                GameManager.Instance.inputSettings.pauseButton = key;
                pauseButtonText.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });

        Slider musicSlider = settingsPanel.transform.Find("Music&Sfx/MusicSlider").GetComponent<Slider>();
        Slider sfxSlider = settingsPanel.transform.Find("Music&Sfx/SfxSlider").GetComponent<Slider>();
            
        musicSlider.onValueChanged.AddListener((value) => {
            AudioManager.Instance.musicSource.volume = value;
        });
        sfxSlider.onValueChanged.AddListener((value) => {
            AudioManager.Instance.sfxSource.volume = value;
        });
    }
#endregion
#region View Management

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

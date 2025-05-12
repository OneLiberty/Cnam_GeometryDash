using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class MainMenuManager : UIManager
{
    [Header("Panels")]
    private GameObject mainMenuPanel;
    private GameObject levelSelectionPanel;
    private GameObject settingsPanel;
    private GameObject statsPanel;
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
        statsPanel = uiCanvas.transform.Find("Stats")?.gameObject;
        askInputPanel = uiCanvas.transform.Find("AskInput")?.gameObject;
    
        if (mainMenuPanel != null) { InitializeMainMenu();} 
        if (levelSelectionPanel != null) { InitializeLevelSelection();} 
        if (statsPanel != null) { InitializeStats();}
        if (settingsPanel != null) { InitializeSettings();}

        if (mainMenuPanel != null) {
            ShowMainMenu();
        }   
    }

#region Main Menu
    private void InitializeMainMenu() 
    {
        Button playButton = mainMenuPanel.transform.Find("Start").GetComponent<Button>();
        Button statsButton = mainMenuPanel.transform.Find("Stats").GetComponent<Button>();
        Button settingsButton = mainMenuPanel.transform.Find("Settings").GetComponent<Button>();
        Button quitButton = mainMenuPanel.transform.Find("Quit").GetComponent<Button>();

        playButton.onClick.AddListener(() => {
            ShowLevelSelection();
        });

        statsButton.onClick.AddListener(() => {
            ShowStats();
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
#region Stats
    private void InitializeStats()
    {
        Button backButton = statsPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });

        TMP_Dropdown levelDropDown = statsPanel.transform.Find("LevelStats/LevelSelector").GetComponent<TMP_Dropdown>();
        levelDropDown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>(); 
        foreach (var levelData in GameManager.Instance.userData.levelProgress)
        {
            options.Add(new TMP_Dropdown.OptionData("Level " + levelData.Key.ToString()));
        }
        
        levelDropDown.AddOptions(options);
        levelDropDown.onValueChanged.AddListener((index) => {
            if (index >= 0 && index < levelDropDown.options.Count) {
                string text = levelDropDown.options[index].text;
                string digits = new string(text.Where(char.IsDigit).ToArray());
                int levelID = int.Parse(digits);
                UpdateLevelStats(levelID);
            }
        });

        UpdateGlobalStats();
        
        if (options.Count > 0) {
            string text = levelDropDown.options[0].text;
            string digits = new string(text.Where(char.IsDigit).ToArray());
            int levelID = int.Parse(digits);
            UpdateLevelStats(levelID);
        }
    }

    private void UpdateGlobalStats() 
    {
        Transform globalStats = statsPanel.transform.Find("GlobalStats");
        TextMeshProUGUI totalJumpsText = globalStats.transform.Find("TotalJumps").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI totalDeathsText = globalStats.transform.Find("TotalDeaths").GetComponent<TextMeshProUGUI>();

        totalJumpsText.text = "Total Jumps: " + GameManager.Instance.userData.totalJumps.ToString();
        totalDeathsText.text = "Total Deaths: " + GameManager.Instance.userData.totalDeath.ToString();
    }

    private void UpdateLevelStats(int levelID) 
    {
        Transform levelStats = statsPanel.transform.Find("LevelStats");
        TextMeshProUGUI levelJumpsText = levelStats.transform.Find("LevelJumps").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI levelDeathsText = levelStats.transform.Find("LevelDeaths").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bestScoreText = levelStats.transform.Find("BestScore").GetComponent<TextMeshProUGUI>();

        if (GameManager.Instance.userData.levelProgress.ContainsKey(levelID)) 
        {
            LevelProgress progress = GameManager.Instance.userData.levelProgress[levelID];
            levelJumpsText.text = "Level Jumps: " + progress.jumps.ToString();
            levelDeathsText.text = "Level Deaths: " + progress.deaths.ToString();
            bestScoreText.text = "Best Score: " + Math.Round(progress.bestScore).ToString() + "%";
        }
        else // nothing found, values are set to 0
        {
            levelJumpsText.text = "Level Jumps: 0";
            levelDeathsText.text = "Level Deaths: 0";
            bestScoreText.text = "Best Score: 0%";
        }
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
                GameManager.Instance.inputSettings.SetBinding(ref GameManager.Instance.inputSettings.jumpButton_0, key);
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
                GameManager.Instance.inputSettings.SetBinding(ref GameManager.Instance.inputSettings.jumpButton_1, key);
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
                GameManager.Instance.inputSettings.SetBinding(ref GameManager.Instance.inputSettings.restartButton, key);
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
                GameManager.Instance.inputSettings.SetBinding(ref GameManager.Instance.inputSettings.pauseButton, key);
                pauseButtonText.text = key.ToString();
                askInputPanel.SetActive(false);
            });
        });

        Slider musicSlider = settingsPanel.transform.Find("Music&Sfx/MusicSlider").GetComponent<Slider>();
        musicSlider.value = GameManager.Instance.userData.musicVolume;
        Slider sfxSlider = settingsPanel.transform.Find("Music&Sfx/SfxSlider").GetComponent<Slider>();
        sfxSlider.value = GameManager.Instance.userData.sfxVolume;
            
        musicSlider.onValueChanged.AddListener((value) => {
            AudioManager.Instance.SetMusicVolume(value);
        });
        sfxSlider.onValueChanged.AddListener((value) => {
            AudioManager.Instance.SetSFXVolume(value);
        });
    }
#endregion

#region View Management
    public void ShowMainMenu() {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
        statsPanel.SetActive(false);
    }

    public void ShowLevelSelection() {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }

    public void ShowStats() {
        mainMenuPanel.SetActive(false);
        statsPanel.SetActive(true);

        UpdateGlobalStats();

        TMP_Dropdown levelDropDown = statsPanel.transform.Find("LevelStats/LevelSelector").GetComponent<TMP_Dropdown>();
        levelDropDown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>(); 
        foreach (var levelData in GameManager.Instance.userData.levelProgress)
        {
            options.Add(new TMP_Dropdown.OptionData("Level " + levelData.Key.ToString()));
        }
        levelDropDown.AddOptions(options);
    }

    public void ShowSettings() {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void QuitGame() {
        Application.Quit();
    }
}
#endregion
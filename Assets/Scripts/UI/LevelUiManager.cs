using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUIManager : UIManager
{
    private GameObject pausePanel;
    private GameObject gameOverPanel;
    private GameObject winPanel;
    private GameObject levelUI;

    private Slider completionBar;
    private TextMeshProUGUI completionText;
    
    public override void InitializePanels()
    {
        pausePanel = uiCanvas.transform.Find("PausePanel")?.gameObject;
        gameOverPanel = uiCanvas.transform.Find("GameOverPanel")?.gameObject;
        winPanel = uiCanvas.transform.Find("WinPanel")?.gameObject;
        levelUI = uiCanvas.transform.Find("LevelUI")?.gameObject;

        if (pausePanel != null) InitializePausePanel();
        if (levelUI != null) InitializeUiPanel();
        if (gameOverPanel != null) InitializeGameOverPanel();
        if (winPanel != null) InitializeWinPanel();
    }

    protected override void Start()
    {
        base.Start();
        FindAnyObjectByType<PlayerController>().OnPlayerPositionChanged += UpdateCompletionBar;
    }

    private void InitializePausePanel() {
        pausePanel.SetActive(false);

        Button resumeBtn = pausePanel.transform.Find("ResumeBtn").GetComponent<Button>();
        Button restartBtn = pausePanel.transform.Find("RestartBtn").GetComponent<Button>();
        Button quitBtn = pausePanel.transform.Find("QuitBtn").GetComponent<Button>();
        
        resumeBtn.onClick.AddListener(() => {
            GameManager.Instance.PauseGame();
        });

        restartBtn.onClick.AddListener(() => {
            GameManager.Instance.RestartLevel();    
        });

        quitBtn.onClick.AddListener(() => {
            GameManager.Instance.ReturnToMainMenu();
        });
    }

    private void InitializeUiPanel() {
        levelUI.SetActive(true);
        completionBar = levelUI.transform.Find("CompletionBar").GetComponent<Slider>();
        completionText = levelUI.transform.Find("CompletionText").GetComponent<TextMeshProUGUI>();
    }

    private void InitializeGameOverPanel() {
        gameOverPanel.SetActive(false);
    }

    private void InitializeWinPanel() {
        winPanel.SetActive(false);
    }

    public void ShowPausePanel(bool gamePaused) {
        pausePanel.SetActive(gamePaused);
    }

    public void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);
    }

    public void ShowWinPanel() {
        winPanel.SetActive(true);
    }

    private void UpdateCompletionBar(Vector2 playerPosition) {
        float completionPercentage = Mathf.Clamp(playerPosition.x / GameManager.Instance.endPosition, 0, 100);
        completionBar.value = completionPercentage * 100;
        completionText.text = $"{(completionPercentage * 100).ToString("F0")}%";
        GameManager.Instance.UpdateCompletion(completionPercentage * 100);
    }
}
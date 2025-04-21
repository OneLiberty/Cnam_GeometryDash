using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour 
{
    [SerializeField] private Transform levelInfoPanel;
    [SerializeField] private Transform parentPanel;
    private bool isFirstLevel = true;
    private string[] levels;
    private List<Transform> levelPanels = new List<Transform>();

    private void Start()
    {
        LoadAllLevels();
    }

    public void LoadAllLevels()
    {
        // load all levels from the Levels Folder
        // Create a panel for each level.

        // Reset when reloading
        foreach (Transform child in parentPanel) {
            if (child.name == "Back") continue;
            Destroy(child.gameObject);
        }
        levelPanels.Clear();
        isFirstLevel = true;
        
        string levelsPath = Application.dataPath + "/Levels";
        levels = Directory.GetFiles(levelsPath, "*.json");

        int panelIndex = 0;
        foreach (string levelPath in levels) {
            string json = File.ReadAllText(levelPath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            CreatePanel(levelData.name, levelData.levelNumber, levelPath, panelIndex);
            panelIndex++;
        }
    }

    private void CreatePanel(string levelName, int levelID, string levelPath, int panelIndex) 
    {
        Transform levelPanel = Instantiate(levelInfoPanel, parentPanel);
        levelPanels.Add(levelPanel);
        
        if (isFirstLevel) {
            levelPanel.gameObject.SetActive(true);
            isFirstLevel = false;
        }
        else 
        {
            levelPanel.gameObject.SetActive(false);
        }
        levelPanel.name = "LevelPanel_" + panelIndex;

        TextMeshProUGUI levelNameText = levelPanel.transform.Find("LevelNameText").GetComponent<TextMeshProUGUI>();
        // Text difficultyText = levelPanel.GetComponentInChildren<Text>();

        levelNameText.text = levelName;
        // difficultyText.text = "Difficulty: " + difficulty.ToString();

        Button levelBtn = levelPanel.GetComponentInChildren<Button>();
        levelBtn.onClick.AddListener(() => {
            GameManager.Instance.StartLevel(levelPath);
        });

        Button nextBtn = levelPanel.transform.Find("NextBtn").GetComponent<Button>();
        nextBtn.onClick.AddListener(() => {
            int nextIndex = (panelIndex + 1) % levelPanels.Count;
            StartCoroutine(LevelSelectionSlide(levelPanel, levelPanels[nextIndex], 1));
        });

        Button prevBtn = levelPanel.transform.Find("PrevBtn").GetComponent<Button>();
        prevBtn.onClick.AddListener(() => {
            int prevIndex = (panelIndex - 1 + levelPanels.Count) % levelPanels.Count;
            StartCoroutine(LevelSelectionSlide(levelPanel, levelPanels[prevIndex], -1));
        });

        Slider completionSlider = levelPanel.transform.Find("CompletionSlider").GetComponent<Slider>();
        TextMeshProUGUI completionText = completionSlider.transform.Find("CompletionText").GetComponent<TextMeshProUGUI>();
        float bestScore = 0; 
        if (GameManager.Instance.userData.levelProgress.ContainsKey(levelID)) {
            bestScore = GameManager.Instance.userData.levelProgress[levelID].bestScore;
        }
        bestScore = Mathf.Round(Mathf.Clamp(bestScore, 0, 100));
        completionText.text = "Best Score : " + bestScore + "%";
        completionSlider.value = bestScore / 100f;

        if (bestScore == 100f) 
        {
            GameObject completionStar = levelPanel.transform.Find("BigStar").gameObject;
            completionStar.SetActive(true);
        }
    }

    IEnumerator LevelSelectionSlide(Transform levelPanel, Transform nextLevelPanel, int direction = 1) {
        float duration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPos = levelPanel.localPosition;
        float panelWidth = levelPanel.GetComponent<RectTransform>().rect.width;

        // direction = 1 for next, -1 for previous
        Vector3 nextPanelPos = new Vector3(startPos.x + direction * panelWidth, startPos.y, startPos.z);
        Vector3 endPos = new Vector3(startPos.x - direction * panelWidth, startPos.y, startPos.z);

        nextLevelPanel.localPosition = nextPanelPos;
        nextLevelPanel.gameObject.SetActive(true);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            levelPanel.localPosition = Vector3.Lerp(startPos, endPos, t);
            nextLevelPanel.localPosition = Vector3.Lerp(nextPanelPos, startPos, t);
            yield return null;
        }

        levelPanel.gameObject.SetActive(false);
        nextLevelPanel.localPosition = startPos;
    }
}
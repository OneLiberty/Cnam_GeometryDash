using System.Collections;
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

    private void Start()
    {
        LoadAllLevels();
    }

    public void LoadAllLevels()
    {
        // load all levels from the Levels Folder
        // Create a panel for each level.
        string levelsPath = Application.dataPath + "/Levels";
        levels = Directory.GetFiles(levelsPath, "*.json");

        foreach (string level in levels) {
            string json = File.ReadAllText(level);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            CreatePanel(levelData.name, levelData.levelNumber, levelData.difficulty); // maybe add more infos if needed. 
        }
    }

    private void CreatePanel(string levelName, int levelID, int difficulty) 
    {
        Transform levelPanel = Instantiate(levelInfoPanel, parentPanel);
        if (isFirstLevel) {
            levelPanel.gameObject.SetActive(true);
            isFirstLevel = false;
        }
        else {
            levelPanel.gameObject.SetActive(false);
        }

        levelPanel.name = "LevelPanel_" + levelID;

        TextMeshProUGUI levelNameText = levelPanel.transform.Find("LevelNameText").GetComponent<TextMeshProUGUI>();
        // Text difficultyText = levelPanel.GetComponentInChildren<Text>();

        levelNameText.text = levelName;
        // difficultyText.text = "Difficulty: " + difficulty.ToString();

        Button levelBtn = levelPanel.GetComponentInChildren<Button>();
        levelBtn.onClick.AddListener(() => {
            GameManager.Instance.StartLevel(levelID);
        });

        Button nextBtn = levelPanel.transform.Find("NextBtn").GetComponent<Button>();
        nextBtn.onClick.AddListener(() => {
            // show next level card
            int nextID = (levelID + 1 > levels.Length) ? 1 : levelID + 1;
            
            Transform nextLevelPanel = levelPanel.transform.parent.Find("LevelPanel_" + nextID);
            if (nextLevelPanel != null) {
                StartCoroutine(LevelSelectionSlide(levelPanel, nextLevelPanel, 1));
            }
        });

        Button prevBtn = levelPanel.transform.Find("PrevBtn").GetComponent<Button>();
        prevBtn.onClick.AddListener(() => {
            // show previous level card
            int prevID = (levelID - 1 < 1) ? levels.Length : levelID - 1;

            Transform prevLevelPanel = levelPanel.transform.parent.Find("LevelPanel_" + prevID);
            if (prevLevelPanel != null) {
                StartCoroutine(LevelSelectionSlide(levelPanel, prevLevelPanel, -1));
            }
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
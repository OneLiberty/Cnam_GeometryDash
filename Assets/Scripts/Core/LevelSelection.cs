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
                levelPanel.gameObject.SetActive(false);
                nextLevelPanel.gameObject.SetActive(true);
            }
        });

        Button prevBtn = levelPanel.transform.Find("PrevBtn").GetComponent<Button>();
        prevBtn.onClick.AddListener(() => {
            // show previous level card
            int prevID = (levelID - 1 < 1) ? levels.Length : levelID - 1;

            Transform prevLevelPanel = levelPanel.transform.parent.Find("LevelPanel_" + prevID);
            if (prevLevelPanel != null) {
                levelPanel.gameObject.SetActive(false);
                prevLevelPanel.gameObject.SetActive(true);
            }
        });
    }

    private void UpdateLevelDisplay()
    {
        
    }
}
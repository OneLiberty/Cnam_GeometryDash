using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private LevelEditor levelEditor;

    [Header("Panels")]
    [SerializeField] private GameObject verticalView;
    [SerializeField] private GameObject propertiesPanel;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button quitButton;

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField levelNameInput;
    [SerializeField] private TMP_InputField levelNumberInput;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TMP_Dropdown musicDropdown;
    [SerializeField] private TMP_Dropdown levelsDropdown;

    private void Start()
    {
        InitializeUI();

        difficultySlider.onValueChanged.AddListener(value =>
        {
            difficultyText.text = $"Difficulty: {value:0} / 10";
        });

        saveButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(levelNameInput.text))
            {
                Debug.LogWarning("Level name cannot be empty");
                return;
            }

            int levelNum = 1;
            if (!int.TryParse(levelNumberInput.text, out levelNum))
            {
                Debug.LogWarning("Invalid level number");
                return;
            }

            int difficulty = Mathf.RoundToInt(difficultySlider.value);
            string music = musicDropdown.options[musicDropdown.value].text;

            levelEditor.SaveLevel(levelNameInput.text, levelNum, difficulty, music);
            RefreshLevelDropdown();
        });

        loadButton.onClick.AddListener(() =>
        {
            if (levelsDropdown.options.Count > 0)
            {
                string levelName = levelsDropdown.options[levelsDropdown.value].text;
                string levelPath = Path.Combine(Application.streamingAssetsPath, "Levels", levelName + ".json");
                levelEditor.LoadLevel(levelPath);

                levelNameInput.text = levelName;
                string music = levelEditor.currentLevel.musicFile;
                for (int i = 0; i < musicDropdown.options.Count; i++)
                {
                    if (musicDropdown.options[i].text == music)
                    {
                        musicDropdown.value = i;
                        break;
                    }
                }
            }
        });
        
        quitButton.onClick.AddListener(() => {
            GameManager.Instance.ReturnToMainMenu();
        });
    }

    private void InitializeUI()
    {
        levelNameInput.text = "New Level";
        levelNumberInput.text = "1";

        difficultySlider.minValue = 1;
        difficultySlider.maxValue = 10;
        difficultySlider.value = 1;
        difficultyText.text = $"Difficulty: {difficultySlider.value:0} / 10";

        // Load music
        musicDropdown.ClearOptions();

        List<string> musicOptions = new List<string>();
        AudioClip[] musicFiles = Resources.LoadAll<AudioClip>("Audio");
        if (musicFiles.Length > 0)
        {
            foreach (AudioClip musicFile in musicFiles)
            {
                if (musicFile.name != "menuLoop" && musicFile.name != "deathSfx")
                {
                    musicOptions.Add(musicFile.name);
                }
            }
        }
        musicDropdown.AddOptions(musicOptions);
        RefreshLevelDropdown();
    }

    private void RefreshLevelDropdown()
    {
        levelsDropdown.ClearOptions();
        
        string levelsPath = Path.Combine(Application.streamingAssetsPath, "Levels");
        if (Directory.Exists(levelsPath))
        {
            string[] levels = Directory.GetFiles(levelsPath, "*.json");
            List<string> levelNames = new List<string>();
            
            foreach (string levelPath in levels)
            {
                string levelName = Path.GetFileNameWithoutExtension(levelPath);
                levelNames.Add(levelName);
            }
            
            if (levelNames.Count > 0)
            {
                levelsDropdown.AddOptions(levelNames);
            }
            else
            {
                List<string> placeholder = new List<string>() { "No Levels" };
                levelsDropdown.AddOptions(placeholder);
                levelsDropdown.interactable = false;
            }
        }
    }
}
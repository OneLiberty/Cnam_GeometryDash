using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject settingsPanel;
    // credits panel
    // secret panel ?? 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu() {
        InitializeMainMenu();
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
        InitializeSettingsPanel();
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ReinitializePanels()
    {
        Canvas menuCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        mainMenuPanel = GameObject.Find("MainMenu");
        levelSelectionPanel = GameObject.Find("LevelSelection");
        settingsPanel = GameObject.Find("Settings");
    }

    private void InitializeSettingsPanel() {
        Button backButton = settingsPanel.transform.Find("Back").GetComponent<Button>();
        backButton.onClick.AddListener(() => {
            ShowMainMenu();
        });

        // Jump button 0
        Button button0 = settingsPanel.transform.Find("JumpInput1").GetComponent<Button>();
        TextMeshProUGUI button0text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button0text.text = GameManager.Instance.inputSettings.jumpButton_0.ToString();
        button0.onClick.AddListener(() => {
            // Open key binding menu for jump button 0
            // Set the key binding to GameManager.Instance.inputSettings.jumpButton_0
        });


        // Jump button 1
        Button button1 = settingsPanel.transform.Find("JumpInput2").GetComponent<Button>();
        TextMeshProUGUI button1text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button1text.text = GameManager.Instance.inputSettings.jumpButton_1.ToString();

        // button1.onClick.AddListener(() => {
        //     // Open key binding menu for jump button 0
        //     // Set the key binding to GameManager.Instance.inputSettings.jumpButton_0
        // });
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

    public void QuitGame() {
        Application.Quit();
    }
}
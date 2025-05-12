using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Grid grid; 
    [SerializeField] private Camera editorCamera;
    [SerializeField] private GameObject objectsPanel;
    [SerializeField] private GameObject propertiesPanel;

    [Header("Prefabs")]
    [SerializeField] private GameObject previewPrefab;
    [SerializeField] private Button buttonPrefab;

    [Header("Ground Settings")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private float groundWidth = 200f;
    [SerializeField] private float offsetX = 20f;

    private GameObject groundObject;

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private GameObject currentPreview;
    private string selectedObjectName;
    private int selectedRotationIndex = 0;
    private string selectedAnchor = "bottom";

    private LevelData currentLevel = new LevelData();
    private List<GameObject> placedObjects = new List<GameObject>();

    private LevelObjectData selectedObjectData;
    private GameObject selectedObject;

    private float cameraSpeed = 20f;

    private void Start()
    {
        LoadAllPrefabs();
        InitializeNewLevel();
        InitializeUI();
        CreateGround(groundWidth);
    } 

    private void LoadAllPrefabs()
    {
        prefabDictionary.Clear();
        foreach (var prefab in Resources.LoadAll<GameObject>("Prefabs/Items"))
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
        foreach (var prefab in Resources.LoadAll<GameObject>("Prefabs/Obstacles"))
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
    }

    private void InitializeNewLevel()
    {
        currentLevel = new LevelData{
            name = "New Level",
            levelNumber = 100, // This should be set to the next available level number
            difficulty = 1,
            musicFile = "EditorMusic",
            groundColor = "#FFFFFF",
            backgroundColor = "#0000FF",
            levelObjects = new List<LevelObjectData>()
        };
    }

    private void InitializeUI() 
    {
        foreach (var prefab in prefabDictionary)
        {
            Button button = Instantiate(buttonPrefab, objectsPanel.transform);
            button.GetComponentInChildren<TMP_Text>().text = prefab.Key;
            button.onClick.AddListener(() => SelectObjectType(prefab.Key));
        }

    }

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
        HandleEscapeKey();
        
        if (!IsPointerOverUI())
        {
            if (selectedObjectName != null) 
            {
                HandleObjectPlacement();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                HandleObjectSelection();
            }
        }

        if (Input.GetKeyDown(GameManager.Instance.inputSettings.editorRemoveButton))
        {
            HandleObjectRemoval();
        }
    }

    private void HandleCameraMovement()
    {
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(GameManager.Instance.inputSettings.editorUpButton) || Input.GetKey(KeyCode.UpArrow))
            movement += Vector3.up;
        if (Input.GetKey(GameManager.Instance.inputSettings.editorDownButton) || Input.GetKey(KeyCode.DownArrow))
            movement += Vector3.down;
        if (Input.GetKey(GameManager.Instance.inputSettings.editorLeftButton) || Input.GetKey(KeyCode.LeftArrow))
            movement += Vector3.left;
        if (Input.GetKey(GameManager.Instance.inputSettings.editorRightButton) || Input.GetKey(KeyCode.RightArrow))
            movement += Vector3.right;
            
        cameraSpeed = Input.GetKey(KeyCode.LeftShift) ? 100f : 20f;
        
        if (movement != Vector3.zero)
            editorCamera.transform.position += movement * Time.deltaTime * cameraSpeed;
    }

    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
            editorCamera.orthographicSize = Mathf.Clamp(editorCamera.orthographicSize - Input.mouseScrollDelta.y, 4f, 20f);
    }

    private void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        selectedObject = null;
        selectedObjectData = null;
        
        if (selectedObjectName != null)
        {
            selectedObjectName = null;
            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
            }
        }
    }

    private void HandleObjectPlacement()
    {
        Vector3 mousePosition = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);
        
        if (currentPreview != null)
        {
            UpdatePreviewPosition(cellPosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject(cellPosition);
            }

            if (Input.GetKeyDown(GameManager.Instance.inputSettings.editorRotationButton))
            {
                selectedRotationIndex = (selectedRotationIndex + 1) % 4;
                currentPreview.transform.rotation = Quaternion.Euler(0, 0, selectedRotationIndex * 90);
            }

            if (Input.GetKeyDown(GameManager.Instance.inputSettings.editorAnchorButton))
            {
                CycleAnchor();
                UpdatePreviewPosition(cellPosition);
            }
        }
    }

    private void HandleObjectSelection()
    {
        Vector3 mousePosition = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        
        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (placedObjects.Contains(hitObject))
            {
                selectedObject = hitObject;
                
                Vector2 objPos = new Vector2(hitObject.transform.position.x, hitObject.transform.position.y);
                selectedObjectData = currentLevel.levelObjects.Find(obj => 
                    Vector2.Distance(new Vector2(obj.position.x, obj.position.y), objPos) < 0.1f
                );
            }
        }
        else
        {
            selectedObject = null;
            selectedObjectData = null;
        }
    }

    private void HandleObjectRemoval()
    {
        if (IsPointerOverUI()) return;
        
        Vector3 mousePosition = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider == null) return;

        GameObject hitObject = hit.collider.gameObject;
        GameObject placedObject = null; 

        if (placedObjects.Contains(hitObject))
        {
            placedObject = hitObject;
        }
        else
        {
            Transform parent = hitObject.transform.parent;
            while (parent != null)
            {
                if (placedObjects.Contains(parent.gameObject))
                {
                    placedObject = parent.gameObject;
                    break;
                }
                parent = parent.parent;
            }
        }
        
        if (placedObject != null)
        {
            RemoveObject(placedObject);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void RemoveObject(GameObject obj)
    {
        Vector3Int cellPos = grid.WorldToCell(obj.transform.position);
        
        int removed = currentLevel.levelObjects.RemoveAll(levelObj => 
            Vector2.Distance(new Vector2(levelObj.position.x, levelObj.position.y), 
                            new Vector2(cellPos.x, cellPos.y)) < 1.5f);
        
        
        if (removed == 0)
        {
            Debug.LogWarning("Failed to find matching level object data for deletion");
        }
        
        placedObjects.Remove(obj);
        Destroy(obj);
        
        if (selectedObject == obj)
        {
            selectedObject = null;
            selectedObjectData = null;
        }
    }

    private void UpdatePreviewPosition(Vector3Int cellPosition)
    {
        if (currentPreview != null)
        {
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
            Vector3 finalPosition = cellCenter;
            
            SpriteRenderer spriteRenderer = currentPreview.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float objectHeight = spriteRenderer.bounds.size.y;
                float cellHeight = grid.cellSize.y;
                
                switch (selectedAnchor.ToLower())
                {
                    case "bottom":
                        finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                        break;
                    case "top":
                        finalPosition.y += (cellHeight - objectHeight) * 0.5f;
                        break;
                }
            }
            
            currentPreview.transform.position = finalPosition;
        }
    }

    private void SelectObjectType(string objectName)
    {
        selectedObject = null;
        selectedObjectData = null;
        selectedObjectName = objectName;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        if (!prefabDictionary.TryGetValue(selectedObjectName, out GameObject prefab))
            return;
            
        currentPreview = Instantiate(prefab);
        
        Vector3 mousePosition = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);
        UpdatePreviewPosition(cellPosition);
        
        currentPreview.transform.rotation = Quaternion.Euler(0, 0, selectedRotationIndex * 90);
        
        SpriteRenderer[] spriteRenderers = currentPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in spriteRenderers)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);
        }

        foreach (var collider in currentPreview.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        foreach (var script in currentPreview.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false;
    }

    private void PlaceObject(Vector3Int cellPosition)
    {
        if (IsPointerOverUI() || !prefabDictionary.TryGetValue(selectedObjectName, out GameObject prefab))
            return;

        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
        Vector3 finalPosition = CalculateObjectPosition(cellCenter, prefab);
        
        GameObject placedObject = Instantiate(prefab, finalPosition, Quaternion.Euler(0, 0, selectedRotationIndex * 90));
        placedObject.transform.SetParent(grid.transform);
        placedObjects.Add(placedObject);

        currentLevel.levelObjects.Add(new LevelObjectData
        {
            type = selectedObjectName,
            position = new CustomVector2(cellPosition.x, cellPosition.y),
            rotation = selectedRotationIndex * 90,
            anchor = selectedAnchor
        });

        if (cellPosition.x > groundWidth)
        {
            groundWidth = cellPosition.x;
            CreateGround(groundWidth);
        }
    }

    private Vector3 CalculateObjectPosition(Vector3 cellCenter, GameObject prefab)
    {
        Vector3 finalPosition = cellCenter;
        
        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float objectHeight = spriteRenderer.bounds.size.y;
            float cellHeight = grid.cellSize.y;

            switch (selectedAnchor.ToLower())
            {
                case "bottom":
                    finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                    break;
                case "top":
                    finalPosition.y += (cellHeight - objectHeight) * 0.5f;
                    break;
            }
        }
        
        return finalPosition;
    }

    private void CycleAnchor()
    {
        selectedAnchor = selectedAnchor.ToLower() switch
        {
            "bottom" => "top",
            "top" => "center",
            _ => "bottom"
        };
    }

    private void CreateGround(float endPos)
    {
        if (groundObject != null)
        {
            Destroy(groundObject);
        }

        float start = 0 - offsetX;
        float end = groundWidth + offsetX;
        float center = (start + end) / 2;
        
        groundObject = Instantiate(groundPrefab, new Vector3(center, -2.5f, 0), Quaternion.identity);
        groundObject.transform.SetParent(transform);
        
        SpriteRenderer renderer = groundObject.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            groundObject.transform.localScale = Vector3.one;
            renderer.size = new Vector2(end - start, 5);
            
            if (!string.IsNullOrEmpty(currentLevel.groundColor) && 
                ColorUtility.TryParseHtmlString(currentLevel.groundColor, out Color color))
            {
                renderer.color = color;
            }
            
            BoxCollider2D boxCollider = groundObject.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(end - start, 5);
                boxCollider.offset = Vector2.zero;
            }
        }
        
        if (backgroundObject != null)
        {
            SpriteRenderer bgRenderer = backgroundObject.GetComponentInChildren<SpriteRenderer>();
            if (bgRenderer != null)
            {
                bgRenderer.drawMode = SpriteDrawMode.Tiled;
                backgroundObject.transform.position = new Vector3(center, 5, 10);
                bgRenderer.size = new Vector2((end - start) * 0.2f, 20);
                
                if (!string.IsNullOrEmpty(currentLevel.backgroundColor) && 
                    ColorUtility.TryParseHtmlString(currentLevel.backgroundColor, out Color bgColor))
                {
                    bgRenderer.color = bgColor;
                }
            }
        }
    }

    public void LoadLevel(string levelPath)
    {
        if (File.Exists(levelPath))
        {
            string json = File.ReadAllText(levelPath);
            currentLevel = JsonConvert.DeserializeObject<LevelData>(json);

            foreach (var obj in placedObjects)
            {
                Destroy(obj);
            }
            placedObjects.Clear();

            float endPos = 0;

            foreach (var objData in currentLevel.levelObjects)
            {
                if (prefabDictionary.TryGetValue(objData.type, out GameObject prefab))
                {
                    Vector3Int cellPosition = grid.WorldToCell(new Vector3(objData.position.x, objData.position.y, 0));
                    Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
                    Vector3 finalPosition = cellCenter;

                    if (cellPosition.x > endPos)
                    {
                        endPos = cellPosition.x;
                    }
                    
                    SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        float objectHeight = spriteRenderer.bounds.size.y;
                        float cellHeight = grid.cellSize.y;

                        switch (objData.anchor.ToLower())
                        {
                            case "bottom":
                                finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                                break;
                            case "top":
                                finalPosition.y += (cellHeight - objectHeight) * 0.5f;
                                break;
                        }
                    }
                    
                    GameObject loadedObject = Instantiate(prefab, finalPosition, Quaternion.Euler(0, 0, objData.rotation));
                    loadedObject.transform.SetParent(grid.transform); 
                    placedObjects.Add(loadedObject);
                }
            }
            CreateGround(endPos);
        
            Debug.Log($"Level '{currentLevel.name}' loaded successfully");
        }
    }

    public void SaveLevel(string levelName, int levelNumber, int difficulty, string musicFile)
    {
        currentLevel.name = levelName;
        currentLevel.levelNumber = levelNumber;
        currentLevel.difficulty = difficulty;
        currentLevel.musicFile = musicFile;

        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        Directory.CreateDirectory(levelsPath);

        string json = JsonConvert.SerializeObject(currentLevel, Formatting.Indented);
        string filePath = Path.Combine(levelsPath, $"{levelName}.json");
        File.WriteAllText(filePath, json);
        
        Debug.Log($"Level '{levelName}' saved successfully at {filePath}");
    }
}
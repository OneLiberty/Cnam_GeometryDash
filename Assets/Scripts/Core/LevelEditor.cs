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
    [SerializeField] private GameObject objectsPanel, propertiesPanel, viewItem, groundPrefab, backgroundObject;
    [SerializeField] private float groundWidth = 200f, offsetX = 20f;

    private GameObject groundObject, currentPreview, selectedObject;

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private string selectedObjectName;
    private int selectedRotationIndex = 0;
    private string selectedAnchor = "bottom";
    private LevelData currentLevel = new LevelData();
    private List<GameObject> placedObjects = new List<GameObject>();
    private Vector3Int lastPlacedPosition;
    private float cameraSpeed = 20f;

    private void Start()
    {
        LoadAllPrefabs();
        InitializeNewLevel();
        InitializeUI();
        CreateGround(groundWidth);

        lastPlacedPosition = new Vector3Int(int.MinValue, int.MinValue, 0);
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
        viewItem.SetActive(false);

        foreach (var prefab in prefabDictionary)
        {
            GameObject newViewItem = Instantiate(viewItem, objectsPanel.transform);

            Transform itemImage = newViewItem.transform.Find("Img");
            itemImage.GetComponent<Image>().sprite = prefab.Value.GetComponentInChildren<SpriteRenderer>().sprite;
            itemImage.GetComponent<Image>().preserveAspect = true;

            Button button = newViewItem.AddComponent<Button>();
            string prefabName = prefab.Key;
            button.onClick.AddListener(() => { SelectObjectType(prefabName); });
            newViewItem.SetActive(true);
        }

    }

    private void Update()
    {
        if (!IsPointerOverUI())
        {
            HandleCameraZoom();
        }
        HandleCameraMovement();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }
        
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

        if (Input.GetKey(GameManager.Instance.inputSettings.editorRemoveButton))
        {
            HandleObjectRemoval();
        }
    }

    private void HandleCameraMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(GameManager.Instance.inputSettings.editorUpButton) || Input.GetKey(KeyCode.UpArrow)) { movement += Vector3.up; }
        if (Input.GetKey(GameManager.Instance.inputSettings.editorDownButton) || Input.GetKey(KeyCode.DownArrow)) { movement += Vector3.down; }
        if (Input.GetKey(GameManager.Instance.inputSettings.editorLeftButton) || Input.GetKey(KeyCode.LeftArrow)) { movement += Vector3.left; }
        if (Input.GetKey(GameManager.Instance.inputSettings.editorRightButton) || Input.GetKey(KeyCode.RightArrow)) { movement += Vector3.right; }
        
        // speed boost
        cameraSpeed = Input.GetKey(KeyCode.LeftShift) ? 100f : 20f;
        editorCamera.transform.position += movement * Time.deltaTime * cameraSpeed;
    }

    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0) { editorCamera.orthographicSize = Mathf.Clamp(editorCamera.orthographicSize - Input.mouseScrollDelta.y, 4f, 20f); }
        editorCamera.transform.Find("Background").localScale = Vector3.one * (editorCamera.orthographicSize / 5f);
    }

    private void ClearSelection()
    {
        selectedObject = null;

        if (currentPreview) { Destroy(currentPreview); }
        currentPreview = null;
        selectedObjectName = null;
    }

    private void HandleObjectPlacement()
    {
        Vector3 mousePosition = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);
        
        if (currentPreview != null)
        {
            UpdatePreviewPosition(cellPosition);
            
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
            
            if (Input.GetMouseButton(0) && !BlockExistsAtPosition(cellPosition))
            {
                PlaceObject(cellPosition);
                lastPlacedPosition = cellPosition;
            }
            
        }
    }

    private bool BlockExistsAtPosition(Vector3Int cellPosition)
    {
        foreach (var obj in placedObjects)
        {
            Vector3Int pos = grid.WorldToCell(obj.transform.position);
            if (pos == cellPosition)
            {
                string existingType = obj.name.Replace("(Clone)", "").Trim();
                if (existingType == selectedObjectName)
                    return true; 
            }
        }
        return false;
    }

    private void HandleObjectSelection()
    {
        RaycastHit2D hit = Physics2D.Raycast(editorCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider && placedObjects.Contains(hit.collider.gameObject))
        {
            selectedObject = hit.collider.gameObject;
        }
        else
        {
            selectedObject = null;
        }
    }

    private void HandleObjectRemoval()
    {
        if (IsPointerOverUI()) return;
        
        RaycastHit2D hit = Physics2D.Raycast(editorCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return;

        GameObject hitObject = hit.collider.gameObject;
        while (!placedObjects.Contains(hitObject) && hitObject.transform.parent)
        {
            hitObject = hitObject.transform.parent.gameObject;
        }

        if (placedObjects.Contains(hitObject)) { RemoveObject(hitObject); }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    private void RemoveObject(GameObject obj)
    {
        placedObjects.Remove(obj);
        Destroy(obj);

        if (selectedObject == obj) ClearSelection();
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
        foreach (var renderer in spriteRenderers) { renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f); }

        // We disable colliders and scripts to prevent interaction
        foreach (var collider in currentPreview.GetComponentsInChildren<Collider2D>()) { collider.enabled = false; }
        foreach (var script in currentPreview.GetComponentsInChildren<MonoBehaviour>()) { script.enabled = false; }
    }

    private void PlaceObject(Vector3Int cellPosition)
    {
        if (IsPointerOverUI() || !prefabDictionary.TryGetValue(selectedObjectName, out GameObject prefab))
            return;

        if (selectedObjectName == "EndPortal")
        {
            bool portalExists = placedObjects.Exists(obj => obj.name.Contains("EndPortal"));
            if (portalExists)
            {
                return;
            }
        }

        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
        Vector3 finalPosition = CalculateObjectPosition(cellCenter, prefab);

        GameObject placedObject = Instantiate(prefab, finalPosition, Quaternion.Euler(0, 0, selectedRotationIndex * 90));
        placedObject.name = selectedObjectName;
        placedObject.transform.SetParent(grid.transform);
        placedObjects.Add(placedObject);

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
        }
    }

    public void SaveLevel(string levelName, int levelNumber, int difficulty, string musicFile)
    {
        currentLevel.name = levelName;
        currentLevel.levelNumber = levelNumber;
        currentLevel.difficulty = difficulty;
        currentLevel.musicFile = musicFile;

        currentLevel.levelObjects.Clear();

        foreach (var obj in placedObjects)
        {
            Vector3Int cellPos = grid.WorldToCell(obj.transform.position);
            int rotation = Mathf.RoundToInt(obj.transform.rotation.eulerAngles.z);

            string type = obj.name.Replace("(Clone)", "").Trim();

            string anchor = "bottom";
            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                float objectHeight = sr.bounds.size.y;
                float cellHeight = grid.cellSize.y;
                float offsetY = obj.transform.position.y - grid.GetCellCenterWorld(cellPos).y;

                if (Mathf.Approximately(offsetY, (cellHeight - objectHeight) * 0.5f))
                    anchor = "top";
                else if (Mathf.Approximately(offsetY, -(cellHeight - objectHeight) * 0.5f))
                    anchor = "bottom";
                else
                    anchor = "center";
            }

            currentLevel.levelObjects.Add(new LevelObjectData
            {
                type = type,
                position = new CustomVector2(cellPos.x, cellPos.y),
                rotation = rotation,
                anchor = anchor
            });
        }

        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        Directory.CreateDirectory(levelsPath);

        string json = JsonConvert.SerializeObject(currentLevel, Formatting.Indented);
        string filePath = Path.Combine(levelsPath, $"{levelName}.json");
        File.WriteAllText(filePath, json);

        Debug.Log($"Level '{levelName}' saved successfully at {filePath}");
    }

}
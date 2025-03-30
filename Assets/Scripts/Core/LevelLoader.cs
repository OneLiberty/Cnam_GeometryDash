using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour {
    private Dictionary<string, GameObject> prefabsDictionnary = new Dictionary<string, GameObject>();

    [SerializeField] private Grid grid;

    [Header("Ground Settings")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private float groundOffset = 10f;
    [SerializeField] private float endPosition = 1000f; // this is the default value for the end position
    [SerializeField] private string endingObject = "LevelEnd";

    private void Awake()
    {
        if (grid == null) 
        {
            grid = FindFirstObjectByType<Grid>();
        }

        LoadAllPrefabs();
    }

    // test 
    void Start()
    {
        // Load level 1 automatically for testing
        LoadLevel(2);
    }

    private void LoadAllPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/"); 

        foreach (GameObject prefab in prefabs)
        {
            prefabsDictionnary[prefab.name] = prefab;
        }

        Debug.Log("All prefabs loaded: " + prefabsDictionnary.Count);
    }

    public void LoadLevel(int levelNumber)
    {
        string LevelPath = Path.Combine(Application.dataPath, "Levels", $"level{levelNumber}.json");
        if (!File.Exists(LevelPath))
        {
            Debug.LogError($"Level file not found: {LevelPath}");
            return;
        }

        string json = File.ReadAllText(LevelPath);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        // don't forger the AudioManager here

        foreach (var gameObj in levelData.levelObjects) 
        {
            if (prefabsDictionnary.TryGetValue(gameObj.type, out GameObject prefab))
            {
                if(gameObj.type == endingObject) 
                {
                    endPosition = gameObj.position.x;
                    continue;
                } 

                if (!string.IsNullOrEmpty(gameObj.anchor)) 
                {
                    Vector3Int cellPosition = grid.WorldToCell(gameObj.position);
                    PlaceObjectWithAnchor(
                        prefab,
                        cellPosition, 
                        gameObj.rotation, 
                        gameObj.anchor
                    );
                } 
            }      
        }

        string groundColor = levelData.groundColor;
        if (string.IsNullOrEmpty(groundColor)) 
        {
            groundColor = "#FFFFFF"; // default color
        }

        CreateGround(0 - groundOffset, endPosition + groundOffset, groundColor);
    }

    private void CreateGround(float start, float end, string groundColor) 
    {
        Debug.Log($"Creating ground from {start} to {end}");
        float groundWidth = end - start;
        float center = (start + end) / 2;

        GameObject ground = Instantiate(groundPrefab, new Vector3(center -10, -2.5f, 0), Quaternion.identity);
        SpriteRenderer renderer = ground.GetComponentInChildren<SpriteRenderer>();

        Color color;
        if (ColorUtility.TryParseHtmlString(groundColor, out color))
        {
            renderer.color = color;
        } 
        

        if(renderer != null) 
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            ground.transform.localScale = Vector3.one;

            renderer.size = new Vector2(groundWidth, 5);
            
            BoxCollider2D boxCollider = ground.GetComponent<BoxCollider2D>();
            if(boxCollider != null) {
                boxCollider.size = new Vector2(groundWidth, 5);
                boxCollider.offset = Vector2.zero;
            }
        } 
    }

    private void PlaceObjectWithAnchor(GameObject prefab, Vector3Int cellPosition, float rotation, string anchor)
    {
        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();

        Vector3 finalPosition = cellCenter;

        float objectHeight = spriteRenderer.bounds.size.y;
        float cellHeight = grid.cellSize.y;

        switch (anchor.ToLower())
        {
            case "bottom":
                finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                break;
            case "top":
                finalPosition.y += (cellHeight - objectHeight) * 0.5f;
                break;
        }

        Instantiate(
            prefab, 
            finalPosition, 
            Quaternion.Euler(0, 0, rotation)
        ).transform.SetParent(grid.transform);
    }
}
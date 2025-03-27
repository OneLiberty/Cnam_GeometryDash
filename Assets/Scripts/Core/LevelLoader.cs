using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour {
    private Dictionary<string, GameObject> prefabsDictionnary = new Dictionary<string, GameObject>();

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;

    [Header("Ground Settings")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private float groundOffset = 10f;
    [SerializeField] private string endingObject = "LevelEnd";

    private void Awake()
    {
        if (grid == null) 
        {
            grid = FindFirstObjectByType<Grid>();
        }

        if (tilemap == null) 
        {
            tilemap = FindFirstObjectByType<Tilemap>();
        }

        LoadAllPrefabs();
    }

    // test 
    void Start()
    {
        // Load level 1 automatically for testing
        LoadLevel(1);
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
                    CreateGround(0 - groundOffset , gameObj.position.x + groundOffset);
                }

                if (!string.IsNullOrEmpty(gameObj.anchor) && gameObj.anchor != "center") 
                {
                    Vector3Int cellPosition = grid.WorldToCell(gameObj.position);
                    PlaceObjectWithAnchor(
                        prefab,
                        cellPosition, 
                        gameObj.rotation, 
                        gameObj.anchor
                    );
                } 
                else 
                {
                    Instantiate(
                        prefab,
                        gameObj.position,
                        Quaternion.Euler(0, 0, gameObj.rotation)
                    );
                }

            }      
        }
    }

    private void CreateGround(float start, float end) {

        float groundWidth = end - start;
        float center = (start + groundWidth) / 2;
        GameObject ground = Instantiate(groundPrefab, new Vector3(center, 0, 0), Quaternion.identity);

        SpriteRenderer renderer = ground.GetComponent<SpriteRenderer>();

        if(renderer != null) 
        {
            float originalWidth = 1f;
            float scale = groundWidth / originalWidth;

            ground.transform.localScale = new Vector3(scale, ground.transform.localScale.y, 1);
        } 
    }

    private void PlaceObjectWithAnchor(GameObject prefab, Vector3Int cellPosition, float rotation, string anchor)
    {
        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();

        Vector3 finalPosition = cellCenter;

        float objectHeight = spriteRenderer.bounds.size.y;
        float cellHeight = grid.cellSize.y;

        switch (anchor.ToLower())
        {
            case "bottom":
                finalPosition.y += (cellHeight - objectHeight) * 0.5f;
                break;
            case "top":
                finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                break;
        }

        Instantiate(
            prefab, 
            finalPosition, 
            Quaternion.Euler(0, 0, rotation)
        );
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private Dictionary<string, GameObject> prefabsDictionnary = new Dictionary<string, GameObject>();

    [SerializeField] private Grid grid;

    [Header("General Settings")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private string backgroundColor = "#000000"; // default color
    [SerializeField] private string groundColor = "#FFFFFF"; // default color

    [Header("Ending Settings")]
    [SerializeField] private float endPosition = 1000f; // this is the default value for the end position
    [SerializeField] private string endingObject = "EndPortal";

    private float offsetX = 20f;

    private void Awake()
    {
        if (grid == null)
        {
            grid = FindFirstObjectByType<Grid>();
        }

        LoadAllPrefabs();
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

        AudioManager.Instance.StopMusic();

        string musicName = levelData.musicFile;
        if (string.IsNullOrEmpty(musicName))
        {
            musicName = "menuLoop"; // default music
        }
        
        AudioManager.Instance.SetMusicClip(musicName);
        AudioManager.Instance.PlayMusic();
        
        foreach (var gameObj in levelData.levelObjects)
        {
            if (prefabsDictionnary.TryGetValue(gameObj.type, out GameObject prefab))
            {
                if (gameObj.type == endingObject)
                {
                    endPosition = gameObj.position.x;
                }

                Vector3Int cellPosition = grid.WorldToCell(gameObj.position);
                PlaceObjectWithAnchor(
                    prefab,
                    cellPosition,
                    gameObj.rotation,
                    gameObj.anchor
                );
            }
        }

        groundColor = levelData.groundColor;
        backgroundColor = levelData.backgroundColor;

        CreateGround(0 - offsetX, endPosition + offsetX, groundColor);
        ModifyBackground(0 - offsetX / 5, (endPosition + offsetX) / 5, backgroundColor);
    }

    private void CreateGround(float start, float end, string groundColor)
    {
        float groundWidth = end - start;
        float center = (start + end) / 2;

        if (groundWidth < 100)
        {
            groundWidth = 100;
        }

        GameObject ground = Instantiate(groundPrefab, new Vector3(center, -2.5f, 0), Quaternion.identity);
        SpriteRenderer renderer = ground.GetComponentInChildren<SpriteRenderer>();

        Color color;
        if (ColorUtility.TryParseHtmlString(groundColor, out color))
        {
            renderer.color = color;
        }

        if (renderer != null)
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            ground.transform.localScale = Vector3.one;

            renderer.size = new Vector2(groundWidth, 5);

            BoxCollider2D boxCollider = ground.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(groundWidth, 5);
                boxCollider.offset = Vector2.zero;
            }
        }
    }

    private void ModifyBackground(float start, float end, string backgroundColor)
    {
        float backgroundWidth = end - start;
        float center = (start + end) / 2;

        if (backgroundWidth < 100)
        {
            backgroundWidth = 100;
        }

        SpriteRenderer renderer = backgroundObject.GetComponentInChildren<SpriteRenderer>();
        backgroundObject.transform.position = new Vector3(center, 5, 10);

        Color color;
        if (ColorUtility.TryParseHtmlString(backgroundColor, out color))
        {
            renderer.color = color;
        }

        if (renderer != null)
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            backgroundObject.transform.localScale = Vector3.one;

            renderer.size = new Vector2(backgroundWidth, 20);
        }
    }

    private void PlaceObjectWithAnchor(GameObject prefab, Vector3Int cellPosition, float rotation, AnchorType anchor)
    {
        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);
        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();

        Vector3 finalPosition = cellCenter;

        float objectHeight = spriteRenderer.bounds.size.y;
        float cellHeight = grid.cellSize.y;

        switch (anchor)
        {
            case AnchorType.Bottom:
                finalPosition.y -= (cellHeight - objectHeight) * 0.5f;
                break;
            case AnchorType.Top:
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
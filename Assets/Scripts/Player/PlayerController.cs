using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum GameMode { Cube, Ship, Wave }

    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject wavePrefab;

    [Header("Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.Cube; 

    public Rigidbody2D rb { get; private set; } // public for testing

    private IPlayerMode currentController;
    private GameObject currentCharacterInstance;
    private Dictionary<GameMode, IPlayerMode> controllers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        controllers = new Dictionary<GameMode, IPlayerMode> {
            { GameMode.Cube, new CubeController(this, rb) },
            { GameMode.Ship, new ShipController(this, rb) },
            { GameMode.Wave, new WaveController(this, rb) }
        };

        currentController = controllers[currentGameMode];
        SpawnPlayerPrefab(currentGameMode);
    }

    public void ChangeGameMode(GameMode newGameMode) {
        currentGameMode = newGameMode;
        currentController = controllers[newGameMode];
        SpawnPlayerPrefab(newGameMode);
    }

    private void SpawnPlayerPrefab(GameMode mode)
    {
        if (currentCharacterInstance != null) 
        {
            Destroy(currentCharacterInstance);
        }
            
        GameObject prefabToSpawn = null;
        switch (mode)
        {
            case GameMode.Cube:
                prefabToSpawn = cubePrefab;
                break;
            case GameMode.Ship:
                prefabToSpawn = shipPrefab;
                break;
            case GameMode.Wave:
                prefabToSpawn = wavePrefab;
                break;
        }
        
        if (prefabToSpawn != null)
        {
            currentCharacterInstance = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            currentCharacterInstance.transform.SetParent(transform);
            
            Rigidbody2D rb = currentCharacterInstance.GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        // main loop, handles input and other updates
        currentController.Update();
    }

    private void FixedUpdate()
    {
        // physics loop
        currentController.FixedUpdate();
    }

    public void OnClick(InputValue value)
    {
        // handle click input
        currentController.OnClick(value);
    }
}

public interface IPlayerMode 
{
    // Interface for player modes
    void Update();
    void FixedUpdate();
    void OnClick(InputValue value);
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IPlayerMode
{
    // Interface for player modes
    void Initialize(GameObject characterInstance);
    void Update();
    void FixedUpdate();
    void OnClick(InputValue value);
}

public class PlayerController : MonoBehaviour
{
    public enum GameMode { Cube, Ship, Wave }

    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject wavePrefab;

    [Header("Settings")]
    [SerializeField] public GameMode currentGameMode { get; private set; } = GameMode.Cube;
    [SerializeField] private const float defaultStartingX = -18f;

    public Rigidbody2D rb { get; private set; } // public for testing

    private IPlayerMode currentController;
    private GameObject currentCharacterInstance;
    private Dictionary<GameMode, IPlayerMode> controllers;

    public bool isDead { get; private set; } = false;

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

    public void ChangeGameMode(GameMode newGameMode)
    {
        currentGameMode = newGameMode;
        currentController = controllers[newGameMode];
        SpawnPlayerPrefab(newGameMode, transform.position.x, transform.position.y);
    }

    public void Die()
    {
        Debug.Log("Player died");
        isDead = true;
        rb.simulated = false;

        rb.linearVelocity = Vector2.zero;
        Destroy(currentCharacterInstance);
        // play death animation or sound here
        // animator.SetTrigger("Die");
        // maybe play a sound
        Invoke(nameof(Respawn), 1f);
    }

    public void Respawn()
    {
        isDead = false;
        rb.simulated = true;
        SpawnPlayerPrefab(currentGameMode);

    }

    private void SpawnPlayerPrefab(GameMode mode, float posX = defaultStartingX, float posY = 0)
    {
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        transform.position = new Vector2(posX, posY);

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

            currentController.Initialize(currentCharacterInstance);

        }
    }

    private void Update()
    {
        // main loop, handles input and other updates
        if (isDead) return;
        currentController.Update();
    }

    private void FixedUpdate()
    {
        // physics loop
        if (isDead) return;
        currentController.FixedUpdate();
    }

    public void OnClick(InputValue value)
    {
        // handle click input
        if (isDead) return;
        currentController.OnClick(value);
    }
}
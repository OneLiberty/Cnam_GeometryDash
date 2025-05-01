using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public interface IPlayerMode
{
    // Interface for player modes
    void Initialize(GameObject characterInstance);
    void Update();
    void FixedUpdate();
    void OnClick();
}

[Serializable]
public class DeathAnimations
{
    public string animationName; 
    public Sprite[] frames;
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

    [Header("Death Animations")]
    [SerializeField] private List<DeathAnimations> deathAnimations; 
    [SerializeField] private SpriteRenderer deathSpriteRenderer; 
    [SerializeField] private float animationFrameDelay = 0.05f; 

    [Header("Input Settings")]
    [SerializeField] private InputSettings inputSettings;

    public Rigidbody2D rb { get; private set; } // public for testing

    private IPlayerMode currentController;
    private GameObject currentCharacterInstance;
    private Dictionary<GameMode, IPlayerMode> controllers;
    private Coroutine currentDeathAnimation;

    public bool isDead { get; private set; } = false;
    public bool isButtonPressed { get ; private set; } = false;
    public float speedModifier = 1f;

    // experimental
    public event Action<Vector2> OnPlayerPositionChanged;

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

        inputSettings = GameManager.Instance.inputSettings;
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

        if (deathAnimations.Count > 0)
        {
            if (currentDeathAnimation != null)
            {
                StopCoroutine(currentDeathAnimation);
            }

            currentDeathAnimation = StartCoroutine(PlayRandomDeathAnimation());
        }

        Invoke(nameof(Respawn), 1f);
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.PlaySFX("deathSfx");
    }

    public void Respawn()
    {
        isDead = false;
        rb.simulated = true;
        SpawnPlayerPrefab(currentGameMode);
        AudioManager.Instance.musicSource.Play();
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
        // notify listeners of player position
        OnPlayerPositionChanged?.Invoke(transform.position);

        if (Input.GetKey(inputSettings.jumpButton_0) || Input.GetKey(inputSettings.jumpButton_1))
        {
            OnClick();
            isButtonPressed = true;
        } 
        else { isButtonPressed = false; }

        if (Input.GetKeyDown(inputSettings.pauseButton))
        {
            GameManager.Instance.PauseGame();
        }

        if (Input.GetKeyDown(inputSettings.restartButton))
        {
            GameManager.Instance.RestartLevel();
        }

        currentController.Update();
    }

    private void FixedUpdate()
    {
        // physics loop
        if (isDead) return;
        currentController.FixedUpdate();
    }

    public void OnClick()
    {
        // handle click input
        if (isDead) return;

        currentController.OnClick();
    }

    private IEnumerator PlayRandomDeathAnimation() 
    {
        DeathAnimations selectedAnimation = deathAnimations[UnityEngine.Random.Range(0, deathAnimations.Count - 1)];
        deathSpriteRenderer.enabled = true;

        foreach (Sprite frame in selectedAnimation.frames)
        {
            deathSpriteRenderer.sprite = frame;
            yield return new WaitForSeconds(animationFrameDelay);
        }
        deathSpriteRenderer.enabled = false;
    }
}
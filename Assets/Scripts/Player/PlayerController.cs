using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum GameMode { Cube, Ship, Wave }

    private GameMode currentGameMode = GameMode.Wave; 
    private IPlayerMode currentController;
    private Dictionary<GameMode, IPlayerMode> controllers;

    private void Awake()
    {
        controllers = new Dictionary<GameMode, IPlayerMode> {
            { GameMode.Cube, new CubeController(this) },
            { GameMode.Ship, new ShipController(this) },
            { GameMode.Wave, new WaveController(this) }
        };

        currentController = controllers[currentGameMode];
    }

    public void ChangeGameMode(GameMode newGameMode) {
        currentGameMode = newGameMode;
        currentController = controllers[newGameMode];
    }

    private void Update()
    {
        // Main loop, handles input and other updates
        currentController.Update();
    }

    private void FixedUpdate()
    {
        // Physics loop
        currentController.FixedUpdate();
    }

    public void OnClick(InputValue value)
    {
        // Handle click input
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

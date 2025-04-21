using System;
using System.Collections;
using UnityEngine;

public class InputSettings : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] public KeyCode jumpButton_0 { get ; private set; } = KeyCode.Space;
    [SerializeField] public KeyCode jumpButton_1 { get ; private set; } = KeyCode.Mouse0;
    [SerializeField] public  KeyCode pauseButton { get ; private set; } = KeyCode.Escape;
    [SerializeField] public KeyCode restartButton { get ; private set; } = KeyCode.R;

    public void SetJumpBtn(int index, KeyCode key)
    {
        if (index == 0) { jumpButton_0 = key; }
        else if (index == 1) { jumpButton_1 = key; }
    }

    public void SetPauseBtn(KeyCode key)
    {
        pauseButton = key;
    }

    public void SetRestartBtn(KeyCode key)
    {
        restartButton = key;
    }

    public void ListenForInput(Action<KeyCode> callback) 
    {
        StartCoroutine(WaitForNewInput(callback));
    }

    private IEnumerator WaitForNewInput(Action<KeyCode> callback) {
        bool keyPressed = false;
        KeyCode key = KeyCode.None;

        yield return null;

        while (!keyPressed)
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    key = k;
                    keyPressed = true;
                    break;
                }
            }
            yield return null;
        }
        
        callback(key);
    }
}
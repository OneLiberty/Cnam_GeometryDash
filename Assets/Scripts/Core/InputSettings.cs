using System;
using System.Collections;
using UnityEngine;

public class InputSettings : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] public KeyCode jumpButton_0 { get ; set; } = KeyCode.Space;
    [SerializeField] public KeyCode jumpButton_1 { get ; set; } = KeyCode.Mouse0;
    [SerializeField] public KeyCode pauseButton { get ; set; } = KeyCode.Escape;
    [SerializeField] public KeyCode restartButton { get ; set; } = KeyCode.R;

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
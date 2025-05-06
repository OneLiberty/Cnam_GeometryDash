using System;
using System.Collections;
using UnityEngine;

public class InputSettings : MonoBehaviour
{
    public KeyCode jumpButton_0
    {
        get => _jumpButton0;
        set
        {
            _jumpButton0 = value;
            GameManager.Instance?.SaveData();
        }
    }
    public KeyCode jumpButton_1
    {
        get => _jumpButton1;
        set
        {
            _jumpButton1 = value;
            GameManager.Instance?.SaveData();
        }
    }
    public KeyCode pauseButton
    {
        get => _pauseButton;
        set
        {
            _pauseButton = value;
            GameManager.Instance?.SaveData();
        }
    }
    public KeyCode restartButton
    {
        get => _restartButton;
        set
        {
            _restartButton = value;
            GameManager.Instance?.SaveData();
        }
    }

    private KeyCode _jumpButton0 = KeyCode.Space;
    private KeyCode _jumpButton1 = KeyCode.Mouse0;
    private KeyCode _pauseButton = KeyCode.Escape;
    private KeyCode _restartButton = KeyCode.R;

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

    public void LoadInputSettings(UserData userData) {
        _jumpButton0 = userData.jumpButton_0;
        _jumpButton1 = userData.jumpButton_1;
        _pauseButton = userData.pauseButton;
        _restartButton = userData.restartButton;
    }
}
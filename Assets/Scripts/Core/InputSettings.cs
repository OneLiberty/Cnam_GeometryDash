using System;
using System.Collections;
using UnityEngine;

public class InputSettings : MonoBehaviour
{
    public event Action onInputChanged;

    [Header("Game Controls")]
    public KeyCode jumpButton_0 = KeyCode.Space;
    public KeyCode jumpButton_1 = KeyCode.Mouse0;
    public KeyCode pauseButton = KeyCode.Escape;
    public KeyCode restartButton = KeyCode.R;

    [Header("Editor Controls")]
    public KeyCode editorUpButton = KeyCode.Z;
    public KeyCode editorDownButton = KeyCode.S;
    public KeyCode editorLeftButton = KeyCode.Q;
    public KeyCode editorRightButton = KeyCode.D;
    public KeyCode editorRotationButton = KeyCode.R; 
    public KeyCode editorAnchorButton = KeyCode.A; 
    public KeyCode editorRemoveButton = KeyCode.Mouse1;

    public void SetBinding(ref KeyCode key, KeyCode newKey)
    {
        if (key != newKey)
        {
            key = newKey;
            onInputChanged?.Invoke();
            GameManager.Instance?.SaveData();
        }
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

    public void LoadInputSettings(UserData userData)
    {
        jumpButton_0 = userData.jumpButton_0;
        jumpButton_1 = userData.jumpButton_1;
        pauseButton = userData.pauseButton;
        restartButton = userData.restartButton;
        editorUpButton = userData.editorUpButton;
        editorDownButton = userData.editorDownButton;
        editorLeftButton = userData.editorLeftButton;
        editorRightButton = userData.editorRightButton;
        editorRotationButton = userData.editorRotationButton;
        editorAnchorButton = userData.editorAnchorButton;
        editorRemoveButton = userData.editorRemoveButton;
    }

    public void ExportToUserData(UserData userData)
    {
        userData.jumpButton_0 = jumpButton_0;
        userData.jumpButton_1 = jumpButton_1;
        userData.pauseButton = pauseButton;
        userData.restartButton = restartButton;
        userData.editorUpButton = editorUpButton;
        userData.editorDownButton = editorDownButton;
        userData.editorLeftButton = editorLeftButton;
        userData.editorRightButton = editorRightButton;
        userData.editorRotationButton = editorRotationButton;
        userData.editorAnchorButton = editorAnchorButton;
        userData.editorRemoveButton = editorRemoveButton;
    }
}
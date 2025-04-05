using UnityEngine;

public class InputSettings : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] public KeyCode jumpButton_0 { get ; private set; } = KeyCode.Space;
    [SerializeField] public KeyCode jumpButton_1 { get ; private set; } = KeyCode.Mouse0;
    [SerializeField] public  KeyCode pauseButton { get ; private set; } = KeyCode.Escape;
    

    public void SetJumpBtn(int index, KeyCode key)
    {
        if (index == 0) { jumpButton_0 = key; }
        else if (index == 1) { jumpButton_1 = key; }
    }

    public void SetPauseBtn(KeyCode key)
    {
        pauseButton = key;
    }
}
using UnityEngine;

public abstract class UIManager : MonoBehaviour 
{
    protected Canvas uiCanvas;

    protected virtual void Awake() {
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        if (uiCanvas == null) {
            Debug.LogError("UICanvas not found in the scene.");
        }
    }

    protected virtual void Start() {
        InitializePanels();
    }

    public abstract void InitializePanels();

}

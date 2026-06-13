using UnityEngine;

public class CursorControllerSystem : MonoBehaviour
{
    private void Awake()
    {
        GameServiceLocator.RegisterService<CursorControllerSystem>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<CursorControllerSystem>();
    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }
}

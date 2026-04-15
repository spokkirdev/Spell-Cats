using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Awake() 
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }
}

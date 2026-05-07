using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }



    [Header("References")]
    [SerializeField] private GameObject radialMenu;
    [SerializeField] private GameObject cursorController;
    [SerializeField] private GameObject camera;
    [SerializeField] private InputActionReference openRadialMenuAction;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (radialMenu == null)
        {
            Debug.LogError("Radial Menu reference is missing in UIManager.");
        }
        if (cursorController == null)
        {
            Debug.LogError("Cursor Controller reference is missing in UIManager.");
        }
        cursorController.GetComponent<CursorController>().HideCursor();
    }

    private void OnEnable()
    {
        openRadialMenuAction.action.performed += ToggleRadialMenu;
        openRadialMenuAction.action.canceled += ToggleRadialMenu;
    }

    private void OnDisable()
    {
        openRadialMenuAction.action.performed -= ToggleRadialMenu;
        openRadialMenuAction.action.canceled -= ToggleRadialMenu;
    }

    private void ToggleRadialMenu(InputAction.CallbackContext context)
    {
        if (radialMenu.GetComponent<RadialMenu>().ToggleMenu())
        {
            camera.GetComponent<CinemachineInputAxisController>().enabled = false;
            cursorController.GetComponent<CursorController>().ShowCursor();
        }
        else
        {
            camera.GetComponent<CinemachineInputAxisController>().enabled = true;
            cursorController.GetComponent<CursorController>().HideCursor();
        }
    }
}

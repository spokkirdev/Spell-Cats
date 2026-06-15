using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class UIManager : MonoBehaviour
{



    [Header("References")]
    [SerializeField] private GameObject radialMenu;
    [SerializeField] private GameObject camera;
    [SerializeField] private InputActionReference openRadialMenuAction;


    private void Awake()
    {
        GameServiceLocator.RegisterService<UIManager>(this);
        
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<UIManager>();
    }

    private void Start()
    {
        radialMenu = GameServiceLocator.GetService<RadialMenu>().gameObject;
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
            ((Component)camera.GetComponent<CinemachineBrain>().ActiveVirtualCamera).GetComponent<CinemachineInputAxisController>().enabled = false;
            GameServiceLocator.GetService<CursorControllerSystem>().ShowCursor();
        }
        else
        {
            ((Component)camera.GetComponent<CinemachineBrain>().ActiveVirtualCamera).GetComponent<CinemachineInputAxisController>().enabled = true;
            GameServiceLocator.GetService<CursorControllerSystem>().HideCursor();
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;
using System;


public class UIManager : MonoBehaviour
{



    [Header("References")]
    [SerializeField] private GameObject radialMenu;
    [SerializeField] private GameObject camera;
    [SerializeField] private InputActionReference openRadialMenuAction;
    [SerializeField] private InputActionReference openPauseMenuAction;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private List<GameObject> healthHearts = new List<GameObject>(5);


    private NetworkHealthComponent _playerHealth;



    private void Awake()
    {
        GameServiceLocator.RegisterService<UIManager>(this);

    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<UIManager>();
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void Start()
    {
        radialMenu = GameServiceLocator.GetService<RadialMenu>().gameObject;
        GameServiceLocator.GetService<NetworkPoints>().Points.OnValueChanged += UpdateScoreText;

    }

    private void OnEnable()
    {
        openRadialMenuAction.action.performed += ToggleRadialMenu;
        openRadialMenuAction.action.canceled += ToggleRadialMenu;
        openPauseMenuAction.action.performed += TogglePauseMenu;
    }

    private void OnDisable()
    {
        openRadialMenuAction.action.performed -= ToggleRadialMenu;
        openRadialMenuAction.action.canceled -= ToggleRadialMenu;
        openPauseMenuAction.action.performed -= TogglePauseMenu;
        UnSubscribeNetworkPoints();


    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            GameServiceLocator.GetService<CursorControllerSystem>().HideCursor();
        }
        else
        {
            pauseMenu.SetActive(true);
            GameServiceLocator.GetService<CursorControllerSystem>().ShowCursor();
        }
    }

    private void Update()
    {
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
       if (GameServiceLocator.TryGetService<NetworkTimer>() && timerText != null)
        {
            if (!GameServiceLocator.GetService<NetworkTimer>().IsTimerRunning)
            {
                GameServiceLocator.GetService<NetworkTimer>().StartTimerServerRpc(600f);
                return;
            }
            double remainingTime = GameServiceLocator.GetService<NetworkTimer>().GetRemainingTime();
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
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

    private void UpdateScoreText(int previousValue, int newValue)
    {
        scoreText.text = newValue.ToString();
    }

    public void InitializeHealthBar()
    {
        _playerHealth = GameServiceLocator.GetService<NetworkHealthComponent>();
        UpdateHealthBar(_playerHealth.CurrentHealth, 100f);
        _playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float previousHealth, float newHealth)
    {
        switch (newHealth)
        {
            case float health when health > 80:
                SetHeartsActive(5);
                break;
            case float health when health > 60:
                SetHeartsActive(4);
                break;
            case float health when health > 40:
                SetHeartsActive(3);
                break;
            case float health when health > 20:
                SetHeartsActive(2);
                break;
            case float health when health > 0:
                SetHeartsActive(1);
                break;
            default:
                SetHeartsActive(0);
                break;
        }
    }

    public void UnSubscribeNetworkPoints()
    {
        if (GameServiceLocator.TryGetService<NetworkPoints>())
        {
            GameServiceLocator.GetService<NetworkPoints>().Points.OnValueChanged -= UpdateScoreText;
        }
    }

    private void SetHeartsActive(int v)
    {
        for (int i = 0; i < healthHearts.Count; i++)
        {
            healthHearts[i].SetActive(i < v);
        }
    }
}

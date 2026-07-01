using UnityEngine;

public class LoadScreen_System : MonoBehaviour
{

    [Header("Load Screen Reference")]
    [SerializeField]
    private GameObject _loaderCameraAndCanvas;

    private void Awake()
    {
        GameServiceLocator.RegisterService<LoadScreen_System>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<LoadScreen_System>();
    }

    public void ToggleLoaderScreen(bool active)
    {
        _loaderCameraAndCanvas.SetActive(active);
    }



}

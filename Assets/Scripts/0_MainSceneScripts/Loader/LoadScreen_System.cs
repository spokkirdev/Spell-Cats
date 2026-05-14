using UnityEngine;

public class LoadScreen_System : MonoBehaviour
{
    public static LoadScreen_System Reference => GameCoreApplication.Instance.Linker_MainScene.LoadScreen_System;

    [Header("Load Screen Reference")]
    [SerializeField]
    private GameObject _loaderCameraAndCanvas;

    public void ToggleLoaderScreen(bool active)
    {
        _loaderCameraAndCanvas.SetActive(active);
    }

}

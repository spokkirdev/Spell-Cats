using UnityEngine;

public class UIPanelMenu_MainMenu : MonoBehaviour
{
    public void Out_LoadGame()
    {
        GameManager.Instance.RequestOnlineMenuScene();
    }
}

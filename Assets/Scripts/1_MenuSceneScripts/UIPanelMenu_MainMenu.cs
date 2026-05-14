using UnityEngine;

public class UIPanelMenu_MainMenu : MonoBehaviour
{
    public void Out_LoadGame()
    {
        GameCoreApplication.Instance.Linker_MainScene.Execution_GameScene.LoadGame_Scene();
    }
}

using UnityEngine;

public class Execution_Initial : MonoBehaviour
{
    void Start()
    {
        InitializeGame();
    }

    // This method is used to initialize the game, by loading the saved data and loading the menu scene.
    private void InitializeGame()
    {
        GameCoreApplication.Instance.Linker_MainScene.SaveLoadSystem.LoadGame();
        GameCoreApplication.Instance.Linker_MainScene.Execution_MenuScene.LoadMenu_Scene();
    }
}

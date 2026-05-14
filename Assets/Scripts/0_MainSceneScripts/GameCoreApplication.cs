using UnityEngine;

public enum EGameState
{
    Loading = 0,
    OnMenu = 1,
    OnGame = 2
}

public class GameCoreApplication : MonoBehaviour
{
    [SerializeField] private Linker_MainScene linker_MainScene;

    // These two linkers are set by the execution scripts of the menu and game scenes, when they are loaded.
    private Linker_GameScene _linker_GameScene;
    private Linker_MenuScene _linker_MenuScene;

    // These properties are used to access the linkers from other scripts, without having to reference the GameCoreApplication instance directly.
    public Linker_MainScene Linker_MainScene => linker_MainScene;
    public Linker_GameScene Linker_GameScene => _linker_GameScene;
    public Linker_MenuScene Linker_MenuScene => _linker_MenuScene;


    private static GameCoreApplication _instance;
    public static GameCoreApplication Instance => _instance;

    [SerializeField, EditorReadOnly] private EGameState currentGameState;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("There are other instance of GamecoreApplication");
            DestroyImmediate(this);
            return;
        }

        _instance = this;
    }

    public void SetGameState(EGameState state)
    {
        currentGameState = state;
    }

    public void SetGameSceneLinker(Linker_GameScene linker)
    {
        _linker_GameScene = linker;
    }

    public void SetMenuSceneLinker(Linker_MenuScene linker)
    {
        _linker_MenuScene = linker;
    }

}

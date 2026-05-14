using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Execution_GameScene : MonoBehaviour
{
    [Header("Scenes To Load")]
    [SerializeField] private SceneObject gameScene;
    [SerializeField] private SceneObject mainScene;


    private List<SceneObject> _scenesLoadedOnThisPhase;


    // This function is called from the main menu scene, when the player clicks on the "Play" button.
    public void LoadGame_Scene() 
    {
        LoadScreen_System.Reference.ToggleLoaderScreen(true);
        GameCoreApplication.Instance.SetGameState(EGameState.Loading);

        _scenesLoadedOnThisPhase = new List<SceneObject>
        {
            gameScene,
            mainScene
        };

        LoadScene();
    }

    // This function is called from the main menu scene, when the player clicks on the "Play" button.
    private void LoadScene()
    {
        if (SceneUtils.CheckIfSceneIsLoaded(gameScene))
        {
            GameSceneIsLoaded();
        }
        else
        {
            // Load the game scene additively, and when it's loaded, call the GameSceneIsLoaded() method.
            AsyncOperation async = SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            async.completed += ((AsyncOperation async) =>
            {
                GameSceneIsLoaded();
            });
        }
    }

    // This method is called when the game scene is loaded, and it's 
    // used to set the game scene linker, unload the unnecessary scenes and start the game execution.
    private void GameSceneIsLoaded()
    {
        GameCoreApplication.Instance.SetGameSceneLinker(FindFirstObjectByType<Linker_GameScene>());
        UnloadUnnecessaryScenes_StartGameExecution();
    }

    // This method is used to unload the unnecessary scenes and start the game execution.
    private Coroutine UnloadUnnecessaryScenes_StartGameExecution()
    {
        // Unload the unnecessary scenes, and when all the scenes are unloaded, call 
        // the StartGameSceneExecution() method.
        List<AsyncOperation> asyncOperations_UnloadingScenes = new List<AsyncOperation>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (CheckIfNeedUnloadThisScene(SceneManager.GetSceneAt(i).name))
            {
                asyncOperations_UnloadingScenes.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
            }
        }

        return StartCoroutine(WaitUntilUnloadScenesCoroutine());

        // This coroutine is used to wait until all the scenes are unloaded, and 
        // then call the StartGameSceneExecution() method.
        IEnumerator WaitUntilUnloadScenesCoroutine()
        {
            if (asyncOperations_UnloadingScenes.Count > 0)
            {
                yield return new WaitUntil(() =>
                {
                    for (int i = 0; i < asyncOperations_UnloadingScenes.Count; i++)
                    {
                        if (asyncOperations_UnloadingScenes[i].isDone == false)
                        {
                            return false;
                        }
                    }
                    return true;
                });
            }
            yield return null;

            StartGameSceneExecution();
        }
    }

    // This method is used to start the game execution, by setting the 
    // game state to "OnGame", setting the active scene to the game scene and calling the
    // StartExecution() method of the game scene linker.
    private void StartGameSceneExecution()
    {
        GameCoreApplication.Instance.SetGameState(EGameState.OnGame);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameScene));

        Linker_GameScene.Reference.GameScene_Starter.StartExecution();
    }

    private bool CheckIfNeedUnloadThisScene(string sceneToCheck)
    {
        for (int i = 0; i < _scenesLoadedOnThisPhase.Count; i++)
        {
            if (_scenesLoadedOnThisPhase[i] == sceneToCheck)
            {
                return false;
            }
        }
        return true;
    }

    
}

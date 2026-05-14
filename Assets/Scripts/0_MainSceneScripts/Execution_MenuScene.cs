using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class is used to execute the loading of the menu scene, and the unloading 
// of the unnecessary scenes, when the game is initialized or when the player returns to the menu from the game scene.
public class Execution_MenuScene : MonoBehaviour
{
    [Header("Scenes To Load")]
    [SerializeField] private SceneObject menuScene;
    [SerializeField] private SceneObject mainScene;

    private List<SceneObject> _scenesLoadedOnThisPhase;

    // This method is called by the Execution_Initial script, when the game is initialized, and by 
    // the Execution_GameScene script, when the player returns to the menu from the game scene.
    public void LoadMenu_Scene() 
    {
        LoadScreen_System.Reference.ToggleLoaderScreen(true);
        GameCoreApplication.Instance.SetGameState(EGameState.Loading);

        _scenesLoadedOnThisPhase = new List<SceneObject>
        {
            menuScene,
            mainScene
        };

        LoadScene();
    }

    // This method is used to load the menu scene, and when it is loaded, it calls the MenuSceneIsLoaded method.
    private void LoadScene()
    {
        if (SceneUtils.CheckIfSceneIsLoaded(menuScene))
        {
            MenuSceneIsLoaded();
        }
        else
        {
            // The menu scene is loaded additively, so that the main scene is not unloaded, and 
            // the loading screen can still be accessed.
            AsyncOperation async = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
            async.completed += ((AsyncOperation async) =>
            {
                // When the menu scene is loaded, the MenuSceneIsLoaded method is called, to 
                // set the menu scene linker and unload the unnecessary scenes.
                MenuSceneIsLoaded();
            });
        }
    }

    private void MenuSceneIsLoaded()
    {
        // The menu scene linker is set, so that it can be accessed from the main scene and the game scene.
        GameCoreApplication.Instance.SetMenuSceneLinker(FindFirstObjectByType<Linker_MenuScene>());
        UnloadUnnecessaryScenes_StartMenuExecution();
    }
    
    private Coroutine UnloadUnnecessaryScenes_StartMenuExecution()
    {
        // The unnecessary scenes are unloaded, so that the game scene is unloaded 
        // when the player returns to the menu from the game scene.
        List<AsyncOperation> asyncOperations_UnloadingScenes = new List<AsyncOperation>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (CheckIfNeedUnloadThisScene(SceneManager.GetSceneAt(i).name))
            {
                asyncOperations_UnloadingScenes.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
            }
        }

        // The StartMenuExecution method is called when all the unnecessary scenes are unloaded, to 
        // set the menu scene as the active scene and start the menu execution.
        return StartCoroutine(WaitUntilUnloadScenesCoroutine());

        // This coroutine is used to wait until all the unnecessary scenes are unloaded, and then start the menu execution.
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
            
            StartMenuExecution();
        }
    }

    // This method is used to set the menu scene as the active scene and start the menu 
    // execution, by calling the StartExecution method of the MenuScene_Starter script.
    private void StartMenuExecution()
    {
        GameCoreApplication.Instance.SetGameState(EGameState.OnMenu);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(menuScene));

        Linker_MenuScene.Reference.MenuScene_Starter.StartExecution();
    }

    // This method is used to check if a scene needs to be unloaded, by
    // checking if it is in the list of scenes loaded on this phase.
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
  
    public async Task TransitionTo(
    string sceneToLoad,
    string[] scenesToKeep,
    EGameState nextState)
    {
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(true);
        GameManager.Instance.ChangeGameState(EGameState.Loading);

        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            await LoadSceneTask(sceneToLoad);

        var unloadTasks = new List<Task>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue; // ✅ ignorar escenas en proceso de descarga
            if (scenesToKeep.Contains(scene.name)) continue;
            if (scene.name == sceneToLoad) continue;

            unloadTasks.Add(UnloadSceneTask(SceneManager.UnloadSceneAsync(scene)));
        }

        await Task.WhenAll(unloadTasks);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        GameManager.Instance.ChangeGameState(nextState);
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(false);
    }

    private Task LoadSceneTask(string sceneName)
    {
        var tcs = new TaskCompletionSource<bool>();
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }

    private Task UnloadSceneTask(AsyncOperation op)
    {
        var tcs = new TaskCompletionSource<bool>();
        op.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }
}

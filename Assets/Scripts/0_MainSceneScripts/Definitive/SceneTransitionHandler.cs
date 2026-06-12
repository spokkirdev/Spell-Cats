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

        // Cargar si no está cargada
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        
        var unloadTasks = new List<AsyncOperation>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scenesToKeep.Contains(scene.name) && scene.name != sceneToLoad)
                unloadTasks.Add(SceneManager.UnloadSceneAsync(scene));
        }
        foreach (var op in unloadTasks)
            await op; 

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        GameManager.Instance.ChangeGameState(nextState);
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(false);
    }
}

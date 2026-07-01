using UnityEngine;
using UnityEngine.SceneManagement;

public class Dev_LoadMainSceneIfNotExists : MonoBehaviour
{
    [SerializeField] private SceneObject mainScene;

    [SerializeField] private bool automaticLoad = true;

    private void Awake()
    {
        if (automaticLoad)
        {
            LoadMainSceneIfNeccessary();
        }
    }

    private void LoadMainSceneIfNeccessary()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == mainScene)
            {
                return;
            }
        }

        SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Additive);
    }
}
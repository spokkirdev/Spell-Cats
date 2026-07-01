using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils
{
    public static bool CheckIfSceneIsLoaded(string sceneToCheck)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneToCheck)
            {
                return true;
            }
        }

        return false;
    }
}

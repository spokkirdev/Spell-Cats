using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Reference => GameCoreApplication.Instance.Linker_MainScene.SaveLoadSystem;

    private string _savePath = "Assets/4_Common/20_SaveGames/";
    private string _saveFileName = "MainGame_SaveGame";

    #if !UNITY_EDITOR
        private string _savePathForBuild = "Savegames/";
    #endif

    [SerializeField]
    private float timeToLetSaveAgain = 5f;

    private bool recentlyGameSaved = false;
    private bool wantToSave = false;

    private Coroutine waitForSaveAgainCoroutine;

    // This method is used to load the game data, and it's called when the player starts the game.
    public void LoadGame()
    {
    #if !UNITY_EDITOR
        _savePath = Application.persistentDataPath +"/"+_savePathForBuild;
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
    #endif
        LoadDataJSON();
    }

    // This method is used to save the game data, and it's called when the player saves the game.
    public void ForceSaveGame()
    {
        ControlRecentlySaving();
        Persistance_Data.GameMain_Data.lastSaveDate = DateTime.UtcNow;
        string saveDataSerialized = StringSerializationAPI.Serialize(Persistance_Data.GameMain_Data.GetType(), Persistance_Data.GameMain_Data);
        JsonReaderWriter.SaveJSON(_savePath + _saveFileName + ".json", saveDataSerialized);
    }

    // This method is used to save the game data manually if possible.
    [ContextMenu("Save Manually If Possible")]
    public void SaveGameIfPossible()
    {
        if (!recentlyGameSaved)
        {
            ForceSaveGame();
        }
        else
        {
            wantToSave = true;
        }
    }

    // This method is used to control the recently saving of the game, to 
    // avoid saving the game too many times in a short period of time, which 
    // can cause performance issues.
    private void ControlRecentlySaving()
    {
        recentlyGameSaved = true;
        if (waitForSaveAgainCoroutine != null)
        {
            StopCoroutine(waitForSaveAgainCoroutine);
        }
        waitForSaveAgainCoroutine = StartCoroutine(WaitForSaveAgain());
    }

    // This coroutine is used to wait until the player can save the game 
    // again, and if the player wants to save the game while it's not possible, it 
    // will save the game when it's possible again.
    IEnumerator WaitForSaveAgain()
    {
        yield return new WaitForSeconds(timeToLetSaveAgain);
        recentlyGameSaved = false;
        if (wantToSave)
        {
            wantToSave = false;
            ForceSaveGame();
        }
    }

    // This method is used to load the game data from the JSON file, and if the file doesn't 
    // exist or it's not possible to deserialize it, it will create a new save game data with 
    // the default values.
    private void LoadDataJSON()
    {
        if (JsonReaderWriter.LoadJSON(_savePath, _saveFileName, out string saveDataSerialized))
        {
            DeserializeData(saveDataSerialized);
        }
        else
        {
            CreateNewSaveDataGame();
        }
    }

    // This method is used to deserialize the game data from the JSON file, and if it's not possible to deserialize 
    // it, it will create a new save game data with the default values.
    private void DeserializeData(string saveDataSerialized)
    {
        try
        {
            GameMain_Data gameMain_Data_Aux = new GameMain_Data();
            gameMain_Data_Aux = (GameMain_Data)StringSerializationAPI.Deserialize(Persistance_Data.GameMain_Data.GetType(), saveDataSerialized);
            SetDataOnPersistance(gameMain_Data_Aux);
        }
        catch
        {
            Debug.LogWarning("Creating default saveDataPersistanceAux - Not Is Possible to deserialize " + _saveFileName);
            CreateNewSaveDataGame();
        }
    }

    // This method is used to set the deserialized data on the persistance, and if the deserialized data is null, it will 
    // create a new save game data with the default values.
    private void SetDataOnPersistance(GameMain_Data gameMain_Data_Aux)
    {
        if (gameMain_Data_Aux != null)
        {
            GameCoreApplication.Instance.Linker_MainScene.Persistance_Data.gameMain_Data = gameMain_Data_Aux;
            CheckDataOnPersistanceAndSolveIssues();
        }
        else
        {
            Debug.LogError("Creating default gameMain_Data - Is null deserialized of " + _saveFileName);
            CreateNewSaveDataGame();
        }
    }

    private void CheckDataOnPersistanceAndSolveIssues()
    {
        // We can check different files of the save and if something fails, we can create it again. For example, save version.
    }

    private void CreateNewSaveDataGame()
    {
        GameCoreApplication.Instance.Linker_MainScene.Persistance_Data.CreateNewSaveGame_Data();
        ForceSaveGame();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

// This class is used to store the data of the game, and it's 
// used to save and load the game data.
[System.Serializable]
public class GameMain_Data
{
    public string userName;
    public DateTime lastSaveDate;

}

public class Persistance_Data : MonoBehaviour
{
    public static GameMain_Data GameMain_Data => GameCoreApplication.Instance.Linker_MainScene.Persistance_Data.gameMain_Data;

    public GameMain_Data gameMain_Data;

    // This method is used to create a new save game data, with the default
    // values, and it's called when the player starts a new game.
    public void CreateNewSaveGame_Data()
    {
        gameMain_Data = new GameMain_Data();
        gameMain_Data.lastSaveDate = DateTime.UtcNow;
    }

}

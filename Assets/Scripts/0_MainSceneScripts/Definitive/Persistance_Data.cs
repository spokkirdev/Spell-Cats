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

[System.Serializable]
public class Persistance_Data: MonoBehaviour
{
    public static GameMain_Data GameMain_Data {get; private set;}

    public void Initialize(GameMain_Data gameMain_Data)
    {
        GameMain_Data = gameMain_Data;
    }
    
    public void CreateNewGameData()
    {
        GameMain_Data = new GameMain_Data();
        GameMain_Data.lastSaveDate = DateTime.UtcNow;
    }

}

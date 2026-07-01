using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour, ISaveLoadSystem
{
    private string _savePath => Path.Combine(Application.persistentDataPath, "saves");
    private string _saveFile => Path.Combine(_savePath, "savegame.json");

    private void Awake()
    {
        GameServiceLocator.RegisterService<ISaveLoadSystem>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<ISaveLoadSystem>();
    }
    public void Save(GameMain_Data data)
    {
        try
        {
            Directory.CreateDirectory(_savePath);
            data.lastSaveDate = DateTime.UtcNow;
            string json = StringSerializationAPI.Serialize(typeof(GameMain_Data), data);
            File.WriteAllText(_saveFile, json);
            Debug.Log($"Save guardado en {_saveFile}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al guardar: {e.Message}");
        }
    }

    public bool TryLoad(out GameMain_Data data)
    {
        data = null;

        if (!File.Exists(_saveFile))
        {
            Debug.Log("No save file found, creating a new one.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(_saveFile);
            data = (GameMain_Data)JsonUtility.FromJson(json, typeof(GameMain_Data));
            return data != null;
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred while loading save file: " + e.Message);
            return false;
        }
    }
}

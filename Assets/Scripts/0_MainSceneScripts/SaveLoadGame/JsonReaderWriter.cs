using System.IO;
using UnityEngine;

public static class JsonReaderWriter
{
    public static bool LoadJSON(string savePath, string resourceName, out string jsonRead)
    {
        jsonRead = "";
        bool succesfully = true;
        using (FileStream fs = new FileStream(savePath + resourceName + ".json", FileMode.OpenOrCreate))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                try
                {
                    jsonRead = reader.ReadToEnd();
                }
                catch
                {
                    Debug.LogError("Not found json where read" + savePath + resourceName);
                    succesfully = false;
                }
                finally
                {
                    reader.Close();
                }
            }
        }
        return succesfully;

    }
    public static void SaveJSON(string path, string json)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                try
                {
                    writer.Write(json);
                    Debug.Log("JSON File " + path + " Saved!");
                }
                catch
                {
                    Debug.LogError("Not found json where save" + json);
                }
                finally
                {
                    writer.Close();
                }
            }
        }
    }

}

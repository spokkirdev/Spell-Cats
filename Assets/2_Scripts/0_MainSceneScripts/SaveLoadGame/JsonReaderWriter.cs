using System.IO;
using UnityEngine;

public static class JsonReaderWriter
{
    public static bool TryReadJSON(string fullPath, out string json)
    {
        json = string.Empty;

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"Fichero no encontrado: {fullPath}");
            return false;
        }

        try
        {
            json = File.ReadAllText(fullPath);
            return true;
        }
        catch (IOException e)
        {
            Debug.LogError($"Error leyendo {fullPath}: {e.Message}");
            return false;
        }
    }

    public static bool TryWriteJSON(string fullPath, string json)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, json);
            return true;
        }
        catch (IOException e)
        {
            Debug.LogError($"Error escribiendo {fullPath}: {e.Message}");
            return false;
        }
    }
}
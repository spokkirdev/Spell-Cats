using System;
using System.Collections.Generic;
using UnityEngine;

public class GameServiceLocator : MonoBehaviour
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service)
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            Debug.LogError($"Service of type {type} is already registered.");
            return;
        }
        _services[type] = service;
        Debug.Log($"Service of type {type} registered successfully.");
    }

    public static void UnregisterService<T>()
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            _services.Remove(type);
        }
        else
        {
            Debug.LogError($"Service of type {type} is not registered.");
        }
    }

    public static T GetService<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        Debug.LogError($"Service of type {type} is not registered.");
        return default;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

/// <summary>
/// Creates a singleton that lives across scenes.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance => _instance;
    protected static T _instance;

    protected virtual void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this as T;
        }
    }
}

public class PersistantSerializedSingleton<T> : SerializedMonoBehaviour where T : Component
{
    public static T Instance => _instance;
    protected static T _instance;

    protected virtual void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this as T;
        }
    }
}

public class SceneSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance => _instance;
    protected static T _instance;
    protected virtual void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this as T;
        }
    }
}

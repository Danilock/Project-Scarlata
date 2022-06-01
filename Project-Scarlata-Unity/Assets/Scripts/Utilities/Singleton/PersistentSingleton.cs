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

    [SerializeField] protected bool LiveAcrossScenes = false; 

    protected virtual void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this as T;

            if(LiveAcrossScenes)
                DontDestroyOnLoad(this.gameObject);
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

/// <summary>
/// Singleton which only lives in one single scene.
/// </summary>
/// <typeparam name="T"></typeparam>
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

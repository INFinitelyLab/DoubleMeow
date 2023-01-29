using UnityEngine;
using System;

public abstract class SingleBehaviour<T> : MonoBehaviour where T: SingleBehaviour<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
                throw new Exception("На сцене нет экземляра " + typeof(T).Name);

            return _instance;
        }
    }


    public static bool IsExist => _instance != null;


    private static T _instance { get; set; }

    private void OnEnable()
    {
        if (_instance != null && _instance != GetComponent<T>())
            Destroy(_instance.gameObject);

        _instance = GetComponent<T>();
        
        OnActive();
    }

    private void OnDisable()
    {
        OnDisactive();
    }

    protected virtual void OnActive() { }
    protected virtual void OnDisactive() { }
}
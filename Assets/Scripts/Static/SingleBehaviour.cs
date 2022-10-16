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

    private static T _instance { get; set; }

    private void OnEnable()
    {
        //if (_instance != null)
        //    throw new Exception("На сцене должен быть активен только один " + typeof(T).Name);

        _instance = GetComponent<T>();
        
        OnActive();
    }

    private void OnDisable()
    {
        OnDisactive();

        //_instance = null;
    }

    protected virtual void OnActive() { }
    protected virtual void OnDisactive() { }
}
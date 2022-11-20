using System;
using UnityEngine;
using UnityEngine.Events;

public class TransiteAnimator : SingleBehaviour<TransiteAnimator>
{
    [SerializeField] private UnityEvent _onFade;

    private static Action _callback;


    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    public static void Fade(Action callback)
    {
        CreateAnInstanceIfNeed();

        Instance._onFade?.Invoke();

        _callback = callback;
    }


    private static void CreateAnInstanceIfNeed()
    {
        if (IsExist == false)
        {
            TransiteAnimator prefab = Resources.LoadAll<TransiteAnimator>("")[0];

            Instantiate(prefab);
        }
    }


    public void OnFadeEnd()
    {
        _callback?.Invoke();

        _callback = null;
    }
}

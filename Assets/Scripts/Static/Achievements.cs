using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Achievements
{
    public static Achievement[] _origins;


    public static void Initialize()
    {
        if (_origins != null)
            return;

        _origins = Resources.LoadAll<Achievement>("");

        Debug.Log("Всего достижений в игре: " + _origins.Length);
    }


    public static void Unlock(string codeName)
    {
        Achievement achi = Find(codeName);

        Secure secure = new Secure(achi.UUID, "UUID");

        if ( Has(codeName) == false )
        {
            PlayerPrefsExtentions.SetSecure(codeName, secure);

            Debug.Log("Получено достижение: " + achi.Name);
        }
    }


    public static bool Has(string codeName)
    {
        if (PlayerPrefsExtentions.HasSecure(codeName))
        {
            return PlayerPrefsExtentions.GetSecure(codeName, "UUID") == Find(codeName).UUID;
        }

        return false;
    }


    public static Achievement Find(string codeName)
    {
        foreach (Achievement achi in _origins)
        {
            if (achi.CodeName == codeName)
            {
                return achi;
            }
        }

        Debug.Log("Не найдено достижение с именем : " + codeName);

        return null;
    }
}
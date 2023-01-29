using UnityEngine;
using System;

public class MiniGames : SingleBehaviour<MiniGames>
{
    [field: SerializeField] public MiniRetrowaver Retrowaver { get; private set; }
    [field: SerializeField] public MiniMetroer Metroer { get; private set; }
    [field: SerializeField] public MiniCurver Curver { get; private set; }
    [field: SerializeField] public MiniTiler Tiler { get; private set; }

    public static MiniGameType CurrentMiniGame { get; private set; } = MiniGameType.None;

    public static bool IsReadyToActive => CurrentMiniGame != MiniGameType.None;
    public static bool IsActive { get; private set; }


    public static void Activate(MiniGameType gameType)
    {
        if (gameType == MiniGameType.None) throw new Exception("Нельзя запустить мини-игру: игровой тип не может быть None");

        CurrentMiniGame = gameType;
    }

    public static void Deactivate()
    {
        CurrentMiniGame = MiniGameType.None;
    }

    public static MiniGame GetMiniGameByType(MiniGameType gameType)
    {
        switch (gameType)
        {
            case MiniGameType.Retrowave:
                return Instance.Retrowaver;
            case MiniGameType.Metroer:
                return Instance.Metroer;
            case MiniGameType.Curver:
                return Instance.Curver;
            case MiniGameType.Tiler:
                return Instance.Tiler;
            case MiniGameType.None:
                return null;

            default: return null;
        }
    }


    public static void Launch()
    {
        if (IsReadyToActive == false) return;
        if (IsActive) return;

        GetMiniGameByType(CurrentMiniGame).Enable();

        IsActive = true;
    }

    public static void Stop()
    {
        if (IsActive == false) return;

        IsActive = false;
    }
}

    public enum MiniGameType
{
    Retrowave,
    Metroer,
    Curver,
    Tiler,
    None
}
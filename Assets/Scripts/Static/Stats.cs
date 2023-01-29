using UnityEngine;
using System;

public static class Stats
{
    public static SecureInt Coins { get; private set; } = new SecureInt(0,"Coins");
    public static SecureInt Level { get; private set; } = new SecureInt(0,"Level");

    public static int PerfectJumpCount { get; private set; }
    public static int DeathCount { get; private set; }

    public static GraphicPreset TargetGraphics { get; set; }
    public static int TargetFPS
    {
        get
        {
            return Application.targetFrameRate;
        }

        set
        {
            Application.targetFrameRate = value;
        }
    }

    public static string selectedSkin { get; private set; }

    private static SecureInt _levelCoins;

    public static Action<int> CoinsCountChanged;
    public static Action<int> LevelUped;

    private const string CoinsSaveKey = "Coins";
    private const string LevelSaveKey = "Level";
    private const string LevelCoinsSaveKey = "LevelCoins";



    public static void IncreaseCoin(int value)
    {
        if (value < 1)
            throw new System.Exception("Нельзя уменьшить число монеток через метод увеличения");

        Coins.Increase(value);

        if (_levelCoins + value >= 5)
            LevelUp();
        else
            _levelCoins.Increase(value);

        CoinsCountChanged?.Invoke( Coins );
    }

    public static bool TryDecreaseCoin(int value)
    {
        if (value < 1)
            throw new Exception("Нельзя увеличить число монеток через метод уменьшения");

        if (Coins >= value)
        {
            Coins.Decrease( value );

            CoinsCountChanged?.Invoke(Coins);

            return true;
        }

        return false;
    }


    public static void SelectSkin(string skinName)
    {
        selectedSkin = skinName;
    }


    
    private static void LevelUp()
    {
        Level.Increase(1);

        _levelCoins = new SecureInt(0, "LevelCoins");

        LevelUped?.Invoke(Level);
    }

    public static void OnDeath()
    {
        DeathCount++;
    }



    public static void OnPerfectJump()
    {
        PerfectJumpCount++;
    }

    public static void OnUnperfectJump()
    {
        PerfectJumpCount = 0;
    }



    public static void Save()
    {
        
    }

    public static void Load()
    {
        
    }
}
using UnityEngine;
using System;

public static class Stats
{
    public static SecureInt Coins { get; private set; } = new SecureInt(0,"Coins");
    public static SecureInt Level { get; private set; } = new SecureInt(0,"Level");

    public static int DeathCount { get; private set; }

    private static SecureInt _levelCoins;

    public static Action<int> CoinsCountChanged;
    public static Action<int> LevelUped;

    private const string CoinsSaveKey = "Coins";
    private const string LevelSaveKey = "Level";
    private const string LevelCoinsSaveKey = "LevelCoins";



    public static void IncreaseCoin(int value)
    {
        if (value < 1)
            throw new System.Exception("Ќельз€ уменьшить число монеток через метод увеличени€");

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
            throw new System.Exception("Ќельз€ увеличить число монеток через метод уменьшени€");

        if (Coins >= value)
        {
            Coins.Increase( -value );

            CoinsCountChanged?.Invoke(Coins);

            return true;
        }

        return false;
    }


    private static void LevelUp()
    {
        Level.Increase(1);

        _levelCoins = new SecureInt(0, "LevelCoins");

        LevelUped?.Invoke(Level);

        //Debug.Log("”ровень повышен! " + (int)Level);
    }


    public static void OnDeath()
    {
        DeathCount++;
    }


    public static void Save()
    {
        PlayerPrefsExtentions.SetSecure( CoinsSaveKey, Coins);
        PlayerPrefsExtentions.SetSecure( LevelSaveKey, Level);
        PlayerPrefsExtentions.SetSecure( LevelCoinsSaveKey, _levelCoins);

        PlayerPrefs.SetInt("Death", DeathCount);
    }


    public static void Load()
    {
        Coins = PlayerPrefsExtentions.GetSecure( CoinsSaveKey, "Coins" );
        Level = PlayerPrefsExtentions.GetSecure( LevelSaveKey, "Level" );
        _levelCoins = PlayerPrefsExtentions.GetSecure( LevelCoinsSaveKey, "LevelCoins" );

        DeathCount = PlayerPrefs.GetInt("Death");
    }
}
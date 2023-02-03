using UnityEngine;
using System;


public static class Bank
{
    public static int Coins { get; private set; }
    public static int ExchangeRate { get; private set; } = 125; //  оличество молочка за 1 монетку

    public static event Action<int> Exchanged;
    public static event Action<int> CoinsIncreased;
    public static event Action<int> CoinsDecreased;


    public static void Exchange(int milkCount)
    {
        if (milkCount < ExchangeRate) return;

        IncreaseCoins(Mathf.FloorToInt(milkCount / ExchangeRate));

        Exchanged?.Invoke(Coins);
    }

    public static void IncreaseCoins(int value)
    {
        if (value <= 0)
            throw new Exception("Ќельз€ уменьшить число монет методом увеличени€");

        Coins += value;

        CoinsIncreased?.Invoke( Coins );
    }

    public static bool TryDecreaseCoins(int value)
    {
        if (value <= 0)
            throw new Exception("Ќельз€ увеличить число монет методом уменьшени€");

        if (Coins - value < 0)
            return false;

        Coins -= value;

        CoinsDecreased?.Invoke( Coins );

        return true;
    }


    public static void Update(BankData data)
    {
        Coins = data.coins;

        CoinsIncreased?.Invoke( Coins );
    }
}

public struct BankData
{
    public int coins;


    public BankData(int coins)
    {
        this.coins = coins;
    }
}
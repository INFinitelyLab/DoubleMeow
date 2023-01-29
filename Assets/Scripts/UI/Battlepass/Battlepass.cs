using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu( fileName = "Battlepass", menuName = "Battlepass" )]
public class Battlepass : ScriptableObject
{
    #region Dynamic

    [SerializeField] private BattlepassCell[] _items;

    #endregion

    #region Editor

    [ExecuteInEditMode]
    private void OnEnable()
    {
        if (_selectedBattlepass != null)
            DestroyImmediate(this, true);
        else
            _selectedBattlepass = this;
    }

    [ExecuteInEditMode]
    private void OnDisable()
    {
        if (_selectedBattlepass != this) return;

        _selectedBattlepass = null;
    }

    #endregion

    #region Static

    private static Battlepass _selectedBattlepass;

    public static Battlepass SelectedBattlepass
    {
        get
        {
            if (_selectedBattlepass == null)
                return _selectedBattlepass = Resources.LoadAll("", typeof(Battlepass))[0] as Battlepass;
            else
                return _selectedBattlepass;
        }
    }

    public static BattlepassCell[] Items => SelectedBattlepass._items;

    public static List<InventoryItem> AvailableItems { get; private set; }

    public static int Destination => Items[Level].cost;
    public static int MaxLevel => Items.Length - 1;

    public static event Action<bool> LevelUped;

    public static int ExchangeRate { get; private set; } = 20; // молочка за одну MeowPass монетку
    public static int Level { get; private set; }
    public static int BattleCoins { get; private set; }

    public static bool IsPremium { get; private set; }


    public static void OnPremiumPurchased()
    {
        IsPremium = true;
    }

    public static void Exchange(int milkCount)
    {
        if (milkCount < 0)
            throw new Exception("Нельзя прокачать уровень отрицательным количеством молочка!");

        BattleCoins += Mathf.FloorToInt(milkCount / ExchangeRate);

        if (milkCount < Destination) return;

        while( BattleCoins >= Destination )
        {
            if (Level < MaxLevel)
                LevelUp();
            else
                return;
        }
    }

    public static void LevelUp()
    {
        if (Level + 1 > Items.Length) throw new Exception("Нельзя повысить уровень MeowPass'а: уровень максимален!");

        AvailableItems.Add( Items[Level].regularItem );

        if (IsPremium)
            AvailableItems.Add( Items[Level].premiumItem );

        BattleCoins -= Destination;
        Level++;


        LevelUped?.Invoke(IsPremium);
    }

    public static bool TryPickItem( InventoryItem item )
    {
        if (AvailableItems.Contains(item) == false) return false;

        AvailableItems.Remove(item);

        return true;
    }


    public static void Update(BattlepassData data)
    {
        Level = data.level;
        BattleCoins = data.progress;

        IsPremium = data.isPremium;

        AvailableItems = new List<InventoryItem>( data.availableItems.Length );
        for(int index = 0; index < data.availableItems.Length; index++)
        {
            if (Inventory.TryGetItemByID(data.availableItems[index], out var item))
                AvailableItems.Add(item);
        }
    }

    #endregion
}

#region Extras

[Serializable]
public struct BattlepassCell
{
    public InventoryItem regularItem;
    public InventoryItem premiumItem;

    public int cost;
}

public struct BattlepassData
{
    public int level;
    public int progress;

    public bool isPremium;

    public string[] availableItems;


    public BattlepassData(int level, int progress, bool isPremium, string[] availableItems)
    {
        this.level = level;
        this.progress = progress;
        this.isPremium = isPremium;
        this.availableItems = availableItems;
    }
}

#endregion
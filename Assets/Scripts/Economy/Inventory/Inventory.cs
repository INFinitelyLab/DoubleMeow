using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class Inventory
{
    private static List<InventoryItem> _items = new List<InventoryItem>(20);
    private static List<InventoryItem> _allItems = new List<InventoryItem>(20);
    private static List<SkinPuzzled> _puzzledItems = new List<SkinPuzzled>(20);

    public static IReadOnlyCollection<InventoryItem> Items => _items;
    public static IReadOnlyCollection<InventoryItem> AllItems => _allItems;
    public static IReadOnlyCollection<SkinPuzzled> PuzzledItems => _puzzledItems;


    public static void Recieve(InventoryItem item)
    {
        if (Has(item))
            throw new Exception("Нельзя положить предмет в инвентарь: предмет уже есть!");

        _items.Add(item);
    }


    public static bool Has(InventoryItem item)
    {
        if (item is SkinPuzzled skin)
            return skin.IsPurchased;

        return _items.Contains(item);
    }


    public static bool TryTake(InventoryItem item)
    {
        if (Has(item) == false)
            return false;

        _items.Remove(item);

        return true;
    }


    public static void Update( InventoryData data )
    {
        _items.Clear();

        foreach( InventoryItem item in _allItems )
        {
            if ( data.IDs.Contains( item.ProductID ) )
            {
                _items.Add( item );
            }
        }

        foreach(PuzzledItemData puzzledItem in data.PuzzledItems)
        {
            if (TryGetItemByID(puzzledItem.ID, out var item) == false) continue;

            if (item is SkinPuzzled == false) continue;

            (item as SkinPuzzled).Initialize( puzzledItem.PuzzleCollected );
        }
    }


    public static string[] GetItemsIDs()
    {
        return _items.Where(p => p is SkinPuzzled == false).Select( p => p.ProductID ).ToArray();
    }


    public static bool TryGetItemByID(string ID, out InventoryItem item)
    {
        foreach(InventoryItem itemInCollection in _allItems)
        {
            if (itemInCollection.ProductID == ID)
            {
                item = itemInCollection;

                return true;
            }
        }

        Debug.Log("Нет предмета с ID : " + ID);
        item = null;
        return false;
    }


    public static void Initialize()
    {
        _allItems = (Resources.LoadAll<InventoryItem>("")).ToList();

        _puzzledItems = _allItems.Where( p => p is SkinPuzzled ).Select( p => p as SkinPuzzled ).ToList();
    }
}


public struct InventoryData
{
    public string[] IDs;
    public PuzzledItemData[] PuzzledItems;


    public InventoryData(string[] IDs, PuzzledItemData[] puzzleDatas)
    {
        this.IDs = IDs;
        this.PuzzledItems = puzzleDatas;
    }
}
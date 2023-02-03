using UnityEngine;
using UnityEngine.Purchasing;
using System;

public sealed class Purchaser : IAPButton
{
    public static Action PurchaseCompleted;
    public static Action PurchaseFailed;

    private static Purchaser Instance;

    public static bool TryBuy(InventoryItem item)
    {
        Debug.Log("I'm trying to Buy!");

        if (item == null)
            throw new Exception("Нельзя купить предмет: предмет несуществует!");

        if (Inventory.Has(item) && item.PurchaseType == PurchaseType.NonCunsumable)
            throw new Exception("Нельзя купить предмет: он уже есть у игрока!");

        if (item is SkinPuzzled)
            return TryPurchaseForPuzzle(item as SkinPuzzled);
        else if (item.IsDonateItem)
            return TryPurchaseForMoney(item);
        else
            return TryPurchaseForCoins(item);
    }


    private void OnEnable()
    {
        onPurchaseComplete.AddListener(OnPurchaseComplete);
        onPurchaseFailed.AddListener(OnPurchaseFail);

        Instance = this;

        CodelessIAPStoreListener.Instance.AddButton(this);
    }

    private void OnDisable()
    {
        onPurchaseComplete.RemoveListener(OnPurchaseComplete);
        onPurchaseFailed.RemoveListener(OnPurchaseFail);

        Instance = null;

        CodelessIAPStoreListener.Instance.RemoveButton(this);
    }

    private void Start() { }


    private static void OnPurchaseComplete(Product product) => PurchaseCompleted?.Invoke();

    private static void OnPurchaseFail(Product product, PurchaseFailureReason reason) => PurchaseFailed?.Invoke();



    private static bool TryPurchaseForCoins(InventoryItem item)
    {
        if (Bank.TryDecreaseCoins(Mathf.RoundToInt(item.Price * (item == Discounter.TodayItem ? (1 - ((float)Discounter.RandomChance / 100f)) : 1))) == true)
        {
            PurchaseCompleted?.Invoke();

            return true;
        }
        else
        {
            PurchaseFailed?.Invoke();

            return false;
        }
    }

    private static bool TryPurchaseForMoney(InventoryItem item)
    {
        if (item == null)
            throw new Exception("Нельзя купить предмет: предмет несуществует!");

        if (CodelessIAPStoreListener.Instance.HasProductInCatalog( item.ProductID ) == false)
            throw new Exception("Нельзя купить предмет: несуществующий ID предмета!");

        Instance.productId = item.ProductID;

        CodelessIAPStoreListener.Instance.InitiatePurchase( item.ProductID );

        return true;
    }

    private static bool TryPurchaseForPuzzle(SkinPuzzled item)
    {
        if (Puzzle.GetByType(item.PuzzleType).CanUse() == false) { PurchaseFailed?.Invoke(); return false; }

        if (item.TryUppuzzle() == false) { PurchaseFailed?.Invoke(); return false; }

        Puzzle.GetByType(item.PuzzleType).TryUse();

        PurchaseCompleted?.Invoke();

        return true;
    }
}

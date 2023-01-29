using UnityEngine;
using System;

public class Shop : SingleBehaviour<Shop>
{
    [SerializeField] private ShopWindow _skinWindow;
    [SerializeField] private ShopWindow _caseWindow;
    [SerializeField] private ShopWindow _extraWindow;
    [SerializeField] private ShopWindow _donateWindow;
    [SerializeField] private Purchaser _purchaser;

    public static InventoryItem SelectedItem { get; private set; }
    public static ShopWindowType CurrentWindowType { get; private set; }

    public static bool IsWaitingForPurchase { get; private set; }

    public static Action<InventoryItem> Reselected;


    protected override void OnActive()
    {
        Purchaser.PurchaseCompleted += OnPurchaseComplete;
        Purchaser.PurchaseFailed += OnPurchaseFailed;

        _skinWindow.Disable();
        _caseWindow.Disable();
        _extraWindow.Disable();
        _donateWindow.Disable();

        SelectWindow( CurrentWindowType );
    }

    protected override void OnDisactive()
    {
        Purchaser.PurchaseCompleted -= OnPurchaseComplete;
        Purchaser.PurchaseFailed -= OnPurchaseFailed;
    }


    public static void TryEquip()
    {
        if (IsWaitingForPurchase == true) return;

        Debug.Log("I'm trying to Equip!");

        if (Inventory.Has(SelectedItem))
            Equip();
        else
            Purchase();
    }

    private static void Equip()
    {
        if (IsWaitingForPurchase == true) return;

        if (SelectedItem is Skin)
        {
            Skin.Update(SelectedItem as Skin);
            Presenter.UpdateSkin();
        }

        Debug.Log("Equiped : " + SelectedItem.Name);
    }

    public static void Select(InventoryItem item)
    {
        if (IsWaitingForPurchase == true) return;

        if (item == null)
            throw new Exception("Предмет не существует!");

        SelectedItem = item;

        Reselected?.Invoke(item);

        Debug.Log("Selected a item : " + item.Name);
    }


    public static void Purchase()
    {
        if (IsWaitingForPurchase == true) return;

        IsWaitingForPurchase = true;

        Purchaser.TryBuy(SelectedItem);
    }

    public static void OnPurchaseComplete()
    {
        if (IsWaitingForPurchase == false)
            throw new Exception("Что за хрень? Я не жду никакой покупки!");

        if (SelectedItem is LeatherBox)
            SelectedItem.OnPurchased();
        else
            if (SelectedItem is SkinPuzzled == false) Inventory.Recieve( SelectedItem );

        Debug.Log("I'm successfully purchase a item : " + SelectedItem);

        GetWindowByType(CurrentWindowType).Disable();
        GetWindowByType(CurrentWindowType).Enable(CurrentWindowType);

        Reselected?.Invoke(SelectedItem);

        Instance.CancelInvoke();
        Instance.Invoke(nameof(UnwaitPurchase), 0.3f);
    }

    public static void OnPurchaseFailed()
    {
        if (IsWaitingForPurchase == false)
            throw new Exception("Что за хрень? Я не жду никакой покупки!");

        // Хер игроку, а не крутой предмет!

        Debug.Log("Failed to purchase a item : " + SelectedItem);

        GetWindowByType(CurrentWindowType).Disable();
        GetWindowByType(CurrentWindowType).Enable(CurrentWindowType);

        Reselected?.Invoke(SelectedItem);

        Instance.CancelInvoke();
        Instance.Invoke(nameof(UnwaitPurchase), 0.3f);
    }


    private void UnwaitPurchase()
    {
        IsWaitingForPurchase = false;
    }

    
    public static void SelectWindow(ShopWindowType type)
    {
        GetWindowByType(CurrentWindowType).Disable();

        CurrentWindowType = type;

        GetWindowByType(CurrentWindowType).Enable(type);

        if (GetWindowByType(CurrentWindowType).TryGetElementByIndex(0, out var item))
        {
            Select( item );
        }
    }

    private static ShopWindow GetWindowByType(ShopWindowType type)
    {
        switch (type)
        {
            case ShopWindowType.Skin:
                return Instance._skinWindow;
            case ShopWindowType.Case:
                return Instance._caseWindow;
            case ShopWindowType.Extra:
                return Instance._extraWindow;
            case ShopWindowType.Donate:
                return Instance._donateWindow;

            default: return null;
        }
    }

    public static void SelectWindow_Skins() => SelectWindow( ShopWindowType.Skin );
    public static void SelectWindow_Cases() => SelectWindow( ShopWindowType.Case );
    public static void SelectWindow_Extras() => SelectWindow( ShopWindowType.Extra );
    public static void SelectWindow_Donate() => SelectWindow( ShopWindowType.Donate );
}

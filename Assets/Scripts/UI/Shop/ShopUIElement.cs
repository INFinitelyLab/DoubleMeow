using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopUIElement : MonoBehaviour
{
    [SerializeField] private Image _image;


    public InventoryItem Item { get; private set; }


    public virtual void Initialize(InventoryItem item, ShopUIElementState state)
    {
        Item = item;

        _image.sprite = item.Icon;

        _image.color = GetElementColorByState(state);
    }


    public virtual void Select()
    {
        Shop.Select(Item);
    }


    public static Color GetElementColorByState(ShopUIElementState state)
    {
        switch (state)
        {
            case ShopUIElementState.NonAvailable:
                return Color.red;
            case ShopUIElementState.Available:
                return Color.white;
            case ShopUIElementState.Purchased:
                return Color.green;

            default: return Color.gray;
        }
    }
}

public struct ShopUIData
{
    public Sprite icon;
    public string name;
    public int price;

    public bool isDonateItem;

    public ShopUIData(Sprite icon, string name, int price, bool isDonateItem)
    {
        this.icon = icon;
        this.name = name;
        this.price = price;
        this.isDonateItem = isDonateItem;
    }
}

public enum ShopUIElementState
{
    NonAvailable,
    Available,
    Purchased
}
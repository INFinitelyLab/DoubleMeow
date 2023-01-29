using UnityEngine;
using TMPro;

public class LeatherUI : ShopUIElement
{
    [SerializeField] private TextMeshProUGUI _priceBar;
    [SerializeField] private TextMeshProUGUI _worthBar;


    public override void Select()
    {
        Shop.Select(Item);

        Shop.Purchase();
    }

    public override void Initialize(InventoryItem item, ShopUIElementState state)
    {
        base.Initialize(item, state);

        _priceBar.text = (item as LeatherBox).Price.ToString();
        _worthBar.text = (item as LeatherBox).Worth.ToString();
    }
}

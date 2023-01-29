using UnityEngine;

// =====[ This is a very important magic class ]===== //
public class ShopWindow : MonoBehaviour
{
    [SerializeField] private ShopUIElement _prefab;
    [SerializeField] private Transform _content;


    public virtual void Enable(ShopWindowType type)
    {
        gameObject.SetActive(true);

        foreach (InventoryItem item in Inventory.AllItems)
        {
            if ((item is Skin && type == ShopWindowType.Skin) ||
                (item is Case && type == ShopWindowType.Case) ||
                (item is LeatherBox && type == ShopWindowType.Donate))
            {
                ShopUIElement element = Instantiate(_prefab, _content);
                ShopUIElementState state = Inventory.Has(item) ? ShopUIElementState.Purchased : ShopUIElementState.Available;

                element.Initialize(item, state);
            }
        }
    }

    public virtual void Disable()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        gameObject.SetActive(false);
    }

    public virtual bool TryGetElementByIndex(int index, out InventoryItem item)
    {
        var elements = GetComponentsInChildren<ShopUIElement>();

        if (elements.Length == 0)
        {
            item = null;
            return false;
        }

        item = elements[index].Item;

        return true;
    }
}

public enum ShopWindowType
{
    Skin = 0,
    Case = 1,
    Extra = 2,
    Donate = 3
}
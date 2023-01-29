using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string ProductID { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public PurchaseType PurchaseType { get; private set; }

    [field: SerializeField] public bool IsDonateItem { get; private set; }


    public virtual void OnPurchased() { }
}

public enum PurchaseType
{
    Consumable,
    NonCunsumable,
    Subscribe
}


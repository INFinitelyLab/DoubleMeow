using UnityEngine;

[CreateAssetMenu( fileName = "Кожаный коробка" )]
public class LeatherBox : InventoryItem
{
    [field: SerializeField] public int Worth { get; private set; }


    public override void OnPurchased()
    {
        Bank.IncreaseCoins(Worth);
    }
}

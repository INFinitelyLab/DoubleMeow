using UnityEngine;

[CreateAssetMenu( fileName = "������� �������" )]
public class LeatherBox : InventoryItem
{
    [field: SerializeField] public int Worth { get; private set; }


    public override void OnPurchased()
    {
        Bank.IncreaseCoins(Worth);
    }
}

using UnityEngine;

[CreateAssetMenu( fileName = "Скин" )]
public class Skin : InventoryItem
{
    [field: SerializeField] public Material Material { get; private set; }


    #region Static

    public static Skin Current { get; private set; }

    public static void Update(Skin skin)
    {
        if (skin == null)
            return;

        Current = skin;
    }

#endregion
}
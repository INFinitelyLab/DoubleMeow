using UnityEngine;

[CreateAssetMenu( fileName = "����" )]
public class Skin : InventoryItem
{
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public Mesh Mesh { get; private set; }


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
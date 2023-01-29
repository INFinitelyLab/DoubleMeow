using UnityEngine;

[CreateAssetMenu( fileName = "Паззловый скин" )]
public class SkinPuzzled : Skin
{
    [field: SerializeField] public PuzzleType PuzzleType { get; private set; }

    public int PuzzleCollected { get; private set; }
    public bool IsPurchased => PuzzleCollected >= Price;


    public void Initialize(int puzzleCollected)
    {
        PuzzleCollected = puzzleCollected;
    }

    public bool TryUppuzzle()
    {
        if (PuzzleCollected >= Price) return false;

        if (PuzzleCollected++ < Price) return true;

        // Do some magic

        return true;
    }

    public PuzzledItemData GetData() { return new PuzzledItemData(ProductID, PuzzleCollected); }
}

[System.Serializable]
public struct PuzzledItemData
{
    public string ID;
    public int PuzzleCollected;

    public PuzzledItemData(string id, int puzzleCollected)
    {
        this.ID = id;
        this.PuzzleCollected = puzzleCollected;
    }
}

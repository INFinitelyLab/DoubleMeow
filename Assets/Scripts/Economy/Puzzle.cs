using UnityEngine;

public sealed class Puzzle
{
    #region Dynamic

    public PuzzleType Type { get; private set; }
    public int Count { get; private set; }

    private Puzzle(PuzzleType type)
    {
        Type = type;
    }
    
    public bool TryUse()
    {
        if (Count < 1) return false;

        Count--;

        return true;
    }

    public bool CanUse()
    {
        return Count >= 1;
    }

    public void Initialize(int count)
    {
        Count = count;
    }

    #endregion

    #region Static

    public static Puzzle Gray { get; private set; } = new Puzzle( PuzzleType.Gray );
    public static Puzzle Blue { get; private set; } = new Puzzle( PuzzleType.Blue );
    public static Puzzle Purple { get; private set; } = new Puzzle( PuzzleType.Purple );
    public static Puzzle Gold { get; private set; } = new Puzzle( PuzzleType.Gold );


    public static Puzzle GetByType(PuzzleType type)
    {
        switch (type)
        {
            case PuzzleType.Gray:
                return Gray;
            case PuzzleType.Blue:
                return Blue;
            case PuzzleType.Purple:
                return Purple;
            case PuzzleType.Gold:
                return Gold;

            default: return null;
        }
    }

    public static void Initialize(int grayCount, int blueCount, int purpleCount, int goldCount)
    {
        Gray.Initialize(grayCount);
        Blue.Initialize(blueCount);
        Purple.Initialize(purpleCount);
        Gold.Initialize(goldCount);
    }

    #endregion
}

[System.Serializable]
public enum PuzzleType
{
    Gray,
    Blue,
    Purple,
    Gold
}
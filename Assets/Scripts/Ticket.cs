using UnityEngine;

public static class Ticket
{
    public static int Count { get; private set; }

    public static void Initialize(int count)
    {
        Count = count;
    }

    public static void Pickup()
    {
        Count++;
    }

    public static bool TryUse()
    {
        if (Count < 1) return false;

        Count--;

        return true;
    }
}

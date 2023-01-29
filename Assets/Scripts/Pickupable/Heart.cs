using UnityEngine;

public class Heart : Pickup
{
    public override bool IsCanPlace => IsAlreadyExist == false;

    public static bool IsAlreadyExist;

    private Transform _transform;


    protected override void OnPickup()
    {
        Count++;
    }

    public override void Init()
    {
        IsAlreadyExist = true;

        _transform = transform;
    }

    private void OnDestroy() => IsAlreadyExist = false;

    private void Update()
    {
        _transform.localRotation *= Quaternion.Euler(0f, Time.deltaTime * 360, 0f);
    }


#region Static

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

#endregion
}
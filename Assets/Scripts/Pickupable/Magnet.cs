using UnityEngine;

public class Magnet : Pickup
{
    public override bool IsCanPlace => Game.Mode.InMagnetMode == false && IsAlreadyExist == false;

    public static bool IsAlreadyExist;

    private Transform _transform;


    protected override void OnPickup()
    {
        Game.Mode.EnableMagnetMode();
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

    public static int Level { get; private set; }
    public static int MaxLevel { get; private set; } = 9;

    public static bool IsFullUpgraded => Level >= MaxLevel;

    public static void Initialize(int level)
    {
        Level = level;
    }

    public static void Upgrade()
    {
        if (Level >= MaxLevel) return;

        Level++;
    }

    public static float UseTime => 15 + Level * 5;

    #endregion
}
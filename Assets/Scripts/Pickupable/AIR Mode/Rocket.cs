using UnityEngine;

public class Rocket : Pickup
{
    public override bool IsCanPlace => Game.Mode.InxAxIxRxMode == false && IsAlreadyExist == false && Builder.Instance.IsCanPlaceEr;

    public static bool IsAlreadyExist;

    private Transform _transform;


    protected override void OnPickup()
    {
        Game.Mode.EnablexAxIxRxMode();
        Player.Presenter.RepositeUFO(_transform.position);
    }


    public override void Init()
    {
        if (IsAlreadyExist) Destroy( gameObject );

        IsAlreadyExist = true;

        _transform = transform.GetChild(0);
    }

    private void OnDestroy() => IsAlreadyExist = false;

    private void OnDisable()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
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

    public static float TargetDistance => 50 + 10 * Level;

    #endregion
}
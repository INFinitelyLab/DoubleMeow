using UnityEngine;

public class Rocket : Pickup
{
    public override bool IsCanPlace => Game.Mode.InxAxIxRxMode == false && IsAlreadyExist == false && Builder.Instance.IsCanPlaceMetro() == false && Builder.Instance.IsCanPlaceTurn() == false;

    public static bool IsAlreadyExist;

    private Transform _transform;


    protected override void OnPickup()
    {
        Game.Mode.EnablexAxIxRxMode();
    }


    public override void Init()
    {
        IsAlreadyExist = true;

        _transform = transform.GetChild(0);
    }

    private void OnDestroy() => IsAlreadyExist = false;


    private void Update()
    {
        _transform.localRotation *= Quaternion.Euler(0f, Time.deltaTime * 360, 0f);
    }
}
using UnityEngine;

public class Double : Pickup
{
    public override bool IsCanPlace => Game.Mode.InDoubleMode == false && IsAlreadyExist == false;

    public static bool IsAlreadyExist;

    private Transform _transform;


    protected override void OnPickup()
    {
        Game.Mode.EnableDoubleMode();
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
}
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
}
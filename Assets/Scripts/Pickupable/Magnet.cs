using UnityEngine;

public class Magnet : Pickup
{
    public override bool IsCanPlace => Game.Mode.InMagnetMode == false && IsAlreadyExist == false;

    public static bool IsAlreadyExist;


    protected override void OnPickup()
    {
        Game.Mode.EnableMagnetMode();
    }


    private void Awake() => IsAlreadyExist = true;

    private void OnDisable() => IsAlreadyExist = false;
}
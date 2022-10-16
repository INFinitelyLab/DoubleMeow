using UnityEngine;

public class Double : Pickup
{
    public override bool IsCanPlace => Game.Mode.InDoubleMode == false && IsAlreadyExist == false;

    public static bool IsAlreadyExist;


    protected override void OnPickup()
    {
        Game.Mode.EnableDoubleMode();
    }


    private void Awake() => IsAlreadyExist = true;

    private void OnDisable() => IsAlreadyExist = false;
}
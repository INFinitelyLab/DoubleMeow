using UnityEngine;

public class Hulk : Pickup
{
    public override bool IsCanPlace => Game.Mode.InHulkMode == false && IsAlreadyExist == false;

    public static bool IsAlreadyExist;


    protected override void OnPickup()
    {
        Game.Mode.EnableHulkMode();
    }


    public override void Init() => IsAlreadyExist = true;

    private void OnDestroy() => IsAlreadyExist = false;

}

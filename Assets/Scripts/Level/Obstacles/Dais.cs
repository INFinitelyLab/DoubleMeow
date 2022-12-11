using UnityEngine;

public class Dais : Obstacle
{
    [SerializeField] private bool IsHalf;

    private bool IsAlreadyExist(Building building) => building == lastBuilding;

    private Building lastBuilding;

    public override bool IsCanPlaceHere(Building building, ObstaclerBase.LargeCell[,] depthTiles, int x, int y, ObstaclerBase.PortalPosition portalPosition)
    {
        if ((IsHalf == true && y < depthTiles.GetLength(1) - 1) || IsAlreadyExist(building)) return false;

        lastBuilding = building;

        return true;
    }

    public override void EnableCollision() { }

    public override void DisableCollision() { }
}

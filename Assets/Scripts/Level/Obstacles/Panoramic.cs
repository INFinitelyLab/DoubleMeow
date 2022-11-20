using UnityEngine;

public class Panoramic : Obstacle
{
    private static bool IsAlreadyExist(Building building) => building == lastBuilding;

    private static Building lastBuilding;

    public override bool IsCanPlaceHere(Building building, ObstaclerBase.LargeCell[,] depthTiles, int x, int y, ObstaclerBase.PortalPosition portalPosition)
    {
        if (IsAlreadyExist(building)) return false;
        lastBuilding = building;

        if( building.GetTileID(new Vector2Int(1, 1)) > 0 )
            return true;

        return false;
    }
}

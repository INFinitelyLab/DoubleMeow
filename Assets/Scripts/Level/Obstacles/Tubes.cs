using UnityEngine;

public class Tubes : Obstacle
{
    private static int count;
    private Building lastBuilding;

    public override bool IsCanPlaceHere(Building building, ObstaclerBase.LargeCell[,] depthTiles, int x, int y, ObstaclerBase.PortalPosition portalPosition)
    {
        if (lastBuilding == building)
        {
            count++;
        }
        else
        {
            lastBuilding = building;

            count = 0;
        }

        //if (count > 2) return false;

        if (portalPosition != ObstaclerBase.PortalPosition.None) return false;

        if (y < depthTiles.GetLength(1) - 1) return true;

        return false;
    }
}

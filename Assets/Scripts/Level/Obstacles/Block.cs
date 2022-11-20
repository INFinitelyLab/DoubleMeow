using UnityEngine;

public class Block : Obstacle
{
    private static int count;
    private Building lastBuilding;

    public override bool IsCanPlaceHere(Building building, ObstaclerBase.LargeCell[,] depthTiles, int x, int y, ObstaclerBase.PortalPosition portalPosition)
    {
        if( lastBuilding == building )
        {
            count++;
        }
        else
        {
            lastBuilding = building;

            count = 0;
        }

        //if (count > 4) return false;

        return true;
    }
}

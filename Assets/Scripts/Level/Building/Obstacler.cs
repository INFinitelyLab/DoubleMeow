using UnityEngine;
using System.Collections;
using System.Linq;

public class Obstacler : ObstaclerBase
{
    [SerializeField] protected Obstacle[] obstacles;
    [SerializeField] protected Milk milk;
    [SerializeField] protected Pickup[] pickups;
    [SerializeField] protected Portal portal;
    [SerializeField] protected Accelerator accelerator;
    [SerializeField] protected PerfectJumpDetector jumpDetector;

    private LargeCell[,] DepthTile { get; set; }


    public RoadLine Generate(Building building, RoadLine startLine)
    {
        DepthTile = new LargeCell[3, building.GetSize().y / 3 ];

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < DepthTile.GetLength(1); y++)
            {
                DepthTile[x, y] = new LargeCell();
            }
        }

        PortalPosition isPortalize = PortalPosition.None;

        if (building.IsPortalFriendly)
        {
            if (Portal.IsCanPlace) isPortalize = PortalPosition.Forward;
            if (Portal.IsWaitingForSecondPortal) isPortalize = PortalPosition.Backward;
        }

        GeneratePath(building, startLine, isPortalize, out var endLine);
        GenerateObstacles(building, isPortalize);
        GeneratePickups(building, isPortalize);
        GeneratePortals(building, isPortalize);
        GenerateDetectors(building, isPortalize);
        GenerateStuff(building, isPortalize);

        return endLine;
    }


    private void GeneratePath(Building building, RoadLine startLine, PortalPosition isPortalize, out RoadLine endLine)
    {
        RoadLine x = startLine;
        
        if (isPortalize == PortalPosition.None || isPortalize == PortalPosition.Forward) DepthTile[ (int)x + 1, 0 ].EnablePath();

        int direction = 0;
        
        for( int y = 1; y < DepthTile.GetLength(1); y++ )
        {
            if (isPortalize != PortalPosition.Backward) direction = Random.Range(-1, 2);

            if ((int)x + direction >= -1 && (int)x + direction <= 1)
            {
                if(building.GetTileID(new Vector2Int((int)x + direction + 1, y)) != 0)
                {
                    x.TrySurf((Direction)direction);
                }
                else if (building.GetTileID(new Vector2Int((int)x + direction - 1, y)) != 0)
                {
                    x.TrySurf( (Direction)(-direction));
                }
            }

            if (building.GetTileID(new Vector2Int((int)x + 1, y)) == 0)
            {
                if (building.GetTileID(new Vector2Int((int)x + 2, y)) == 0)
                    x.TrySurf(Direction.Left);
                else
                    x.TrySurf(Direction.Right);
            }

            if ( y + 1 < DepthTile.GetLength(1) || isPortalize != PortalPosition.Forward ) DepthTile[(int)x + 1, y].EnablePath();
        }

        endLine = x;
    }


    private void GenerateObstacles(Building building, PortalPosition isPortalize)
    {
        for( int x = -1; x < 2; x++ )
        {
            for( int y = isPortalize == PortalPosition.Backward? 1 : 0; y < (DepthTile.GetLength(1)) - (isPortalize == PortalPosition.Forward? 1 : 0); y++ )
            {
                if ( DepthTile[x + 1, y].IsPath || building.GetTileID(new Vector2Int(x + 1,y * 3)) < 2 ) continue;

                Vector3 position = new Vector3( x, 0, y * 3);

                if (y > 0) if ( DepthTile[x + 1, y - 1].IsPath == false) DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(obstacles.Random(), position + Vector3.forward * 0, building.transform), 0);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(obstacles.Random(), position + Vector3.forward * 1, building.transform), 1);
                if (y < (DepthTile.GetLength(1)) - 1) if (DepthTile[x + 1, y + 1].IsPath == false) DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(obstacles.Random(), position + Vector3.forward * 2, building.transform), 2);
            }
        }
    }


    private void GeneratePickups(Building building, PortalPosition isPortalize)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = isPortalize == PortalPosition.Backward ? 1 : 0; y < (DepthTile.GetLength(1)) - (isPortalize == PortalPosition.Forward ? 1 : 0); y++)
            {
                if (DepthTile[x + 1, y].IsPath == false) continue;

                Vector3 position = new Vector3(x, 0, y * 3);

                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.zero * 0, building.transform), 0);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 1, building.transform), 1);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 2, building.transform), 2);
            }
        }
    }


    private void GeneratePortals(Building building, PortalPosition isPortalize)
    {
        if( isPortalize == PortalPosition.Forward )
        {
            Vector3 position = new Vector3( (int)GetPathLine(4), 0, DepthTile.GetLength(1) * 3 - 3 );

            DepthTile[(int)position.x + 1, 2].AddPlaceable(CreatePlaceable(portal, position, building.transform), 0);

            Portal.Initialize( position );
        }
        else if (isPortalize == PortalPosition.Backward)
        {
            Vector3 position = new Vector3((int)GetPathLine(1), 0, 3);

            DepthTile[(int)position.x + 1, 1].AddPlaceable(CreatePlaceable(portal, position, building.transform, true), 0);

            Portal.Initialize(position);
        }
    }


    private void GenerateDetectors(Building building, PortalPosition isPortalize)
    {
        if (isPortalize == PortalPosition.Forward) return;

        for( int x = -1; x < 2; x++ )
        {
            Vector3 position = new Vector3(x, 0, DepthTile.GetLength(1) * 3 - 1);

            if (building.GetTileID(new Vector2Int((int)position.x + 1, (int)position.y)) == 0) continue;

            CreatePlaceable( jumpDetector, position, building.transform);
        }
    }


    private void GenerateStuff(Building building, PortalPosition isPortalize)
    {
        //if (isPortalize != PortalPosition.None) return;

        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < (DepthTile.GetLength(1)); y++)
            {

                // === Accelerators ===
                if (y < DepthTile.GetLength(1) - 1 && Random.Range(0,5) == 4)
                {
                    if ( DepthTile[x + 1, y].IsPath && DepthTile[x + 1, y + 1].IsPath && building.GetTileID( new Vector2Int(x + 1, y * 3 + 1) ) > 1)
                    {
                        Vector3 position = new Vector3( x, 0, y * 3 + 1);

                        DepthTile[x + 1,y].AddPlaceable(CreatePlaceable(accelerator, position, building.transform), 1);
                    }
                }


            }
        }
    }


    protected Placeable CreatePlaceable(Placeable origin, Vector3 position, Transform parent, bool isInverse = false)
    {
        position.x *= 0.746f;
        position.z *= 0.75f;
        position.z += 0.25f;
        position.y = 0.2f;

        Placeable placeable = Instantiate(origin, parent);

        placeable.transform.localPosition = position;
        placeable.transform.localRotation = Quaternion.Euler(0, isInverse ? 180 : 0, 0);

        return placeable;
    }


    private RoadLine GetPathLine(int y)
    {
        if (DepthTile[0, y].IsPath)
            return RoadLine.Mercury;
        else if (DepthTile[1, y].IsPath)
            return RoadLine.Venus;
        else
            return RoadLine.Earth;
    }
}
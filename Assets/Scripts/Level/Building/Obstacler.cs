using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacler : ObstaclerBase
{
    [SerializeField] protected Obstacle[] obstacles;
    [SerializeField] protected Obstacle[] bridgeObstacles;
    [SerializeField] protected Obstacle _rise;
    [SerializeField] protected Milk milk;
    [SerializeField] protected Pickup[] pickups;
    [SerializeField] protected Portal portal;
    [SerializeField] protected Accelerator accelerator;
    [SerializeField] protected PerfectJumpDetector jumpDetector;

    private LargeCell[,] DepthTile { get; set; }


    public RoadLine Generate(Building building, RoadLine startLine, bool isNeedToRise, bool isBridge = false)
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
            if (Portal.IsCanPlace && isNeedToRise == false) isPortalize = PortalPosition.Forward;
            if (Portal.IsWaitingForSecondPortal) isPortalize = PortalPosition.Backward;
        }

        GeneratePath(building, startLine, isPortalize, out var endLine);
        
        if(isBridge)
        {
            GenerateBridgeObstacles(building);
            GenerateBridgePickups(building, startLine, endLine);
        }
        else
        {
            GenerateObstacles(building, isPortalize, isNeedToRise);
            GeneratePickups(building, isPortalize);
        }

        GeneratePortals(building, isPortalize);
        //GenerateDetectors(building, isPortalize);
        //GenerateStuff(building, isPortalize);


        return endLine;
    }

    private void Awake()
    {
        foreach(Obstacle prefab in obstacles)
        {
            prefab.Initialize();
        }
    }



    #region GenerationMethods

    private void GeneratePath(Building building, RoadLine startLine, PortalPosition isPortalize, out RoadLine endLine)
    {
        RoadLine x = startLine;
        
        if (isPortalize == PortalPosition.None || isPortalize == PortalPosition.Forward) DepthTile[ (int)x + 1, 0 ].EnablePath();

        int direction = 0;
        int identityCount = 0;

        for( int y = 1; y < DepthTile.GetLength(1); y++ )
        {
            if (DepthTile[(int)x + 1, y - 1].IsPath) identityCount++;

            // === Выбор направление для смещения === //
            if (isPortalize != PortalPosition.Backward || y > DepthTile.GetLength(1) - 2)
            {
                if (identityCount > 1)   
                {
                    identityCount = 0;
                    direction = Random.Range(0, 2) == 1 ? -1 : 1;
                }
                else
                {
                    direction = Random.Range(-1, 2);
                }
            }

            // === Смещение дорожки по направлению === //
            if (building.GetTileID(new Vector2Int((int)x + 1 + direction, y)) > 0)
            {
                x.TrySurf((Direction)direction);
            }
            else if (building.GetTileID(new Vector2Int((int)x + 1 - direction, y)) > 0)
            {
                x.TrySurf((Direction)(-direction));
            }

            // === Смещение с некорректной дорожки === //
            if (building.GetTileID(new Vector2Int((int)x + 1, y)) == 0)
            {
                if (building.GetTileID(new Vector2Int((int)x + 2, y)) == 0)
                    x.TrySurf(Direction.Left);
                else
                    x.TrySurf(Direction.Right);
            }

            if (y + 1 < DepthTile.GetLength(1) || isPortalize != PortalPosition.Forward)
            {
                DepthTile[(int)x + 1, y].EnablePath();
            }
        }

        if(building.IsObstacleInCenter && Random.Range(0,2) == 0)
        {
            for(int x2 = 0; x2 < 3; x2++)
            {
                if( building.GetTileID(new Vector2Int(x2, 5)) > 0 )
                {
                    DepthTile[x2, 1].EnableObstacle();
                }
            }
        }

        endLine = x;
    }


    private void GenerateObstacles(Building building, PortalPosition isPortalize, bool isNeedToRise)
    {
        int nextBuildingIndex = 0;
        Obstacle obstacle = null;

        for( int x = -1; x < 2; x++ )
        {
            for( int y = isPortalize == PortalPosition.Backward? 1 : 0; y < (DepthTile.GetLength(1)) - (isPortalize == PortalPosition.Forward? 1 : 0); y++ )
            {
                if( DepthTile[x + 1, y].IsObstacle == false )
                {
                    nextBuildingIndex--;

                    if ( DepthTile[x + 1, y].IsPath || DepthTile[x + 1, y].IsEmpty == false || building.GetTileID(new Vector2Int(x + 1,y * 3)) < 2) continue;

                    if (nextBuildingIndex > 0) continue;
                    
                    obstacle = GetCorrectObstacle(building, x, y, isPortalize);
                }
                else
                {
                    if(DepthTile[x + 1, y].IsEmpty == true)
                    {
                        if (x == -1 && Random.Range(0,2) == 0)
                            obstacle = obstacles[1];
                        else
                            obstacle = obstacles[0];
                    }

                    
                }

                Vector3 position = new Vector3( x, 0, y * 3 + 1);

                if (x + obstacle.Size.x + 1 > DepthTile.GetLength(0) || y * 3 + obstacle.Size.y - 1 > DepthTile.GetLength(1) * 3) continue;

                if ( obstacle != null )
                {
                    Placeable placeable = CreatePlaceable(obstacle, position, building.transform);

                    for (int obsX = 0; obsX < obstacle.Size.x; obsX++)
                    {
                        for (int obsY = 0; obsY < Mathf.CeilToInt(obstacle.Size.y / 3f); obsY++)
                        {
                            if (obstacle.IsEmpty[obsX, obsY]) continue;

                            DepthTile[obsX + x + 1, obsY + y].AddPlaceable(placeable, 1);
                        }
                    }

                    nextBuildingIndex = Random.Range(1,4) + 1;
                }
            }
        }

        if( isNeedToRise == true )
        {
            int y = DepthTile.GetLength(1) - 1;
            int x = (int)GetPathLine(y);

            Vector3 position = new Vector3( x, 0, y * 3 + 1 );

            DepthTile[x + 1, y].AddPlaceable( CreatePlaceable(_rise, position, building.transform), 1 );
        }
    }


    private void GenerateBridgeObstacles(Building building)
    {
        int nextBuildingIndex = 0;

        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < (DepthTile.GetLength(1)); y++)
            {
                nextBuildingIndex--;

                if (DepthTile[x + 1, y].IsPath || DepthTile[x + 1, y].IsEmpty == false || building.GetTileID(new Vector2Int(x + 1, y * 3)) < 2 || nextBuildingIndex > 0) continue;

                Obstacle obstacle = bridgeObstacles.Random();

                Vector3 position = new Vector3(x, 1.65f, y * 3 + 5);

                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(obstacle, position, building.transform), 1);

                nextBuildingIndex += Random.Range( 3, 6 );
            }
        }
    }


    private void GeneratePickups(Building building, PortalPosition isPortalize)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = isPortalize == PortalPosition.Backward ? 1 : 0; y < (DepthTile.GetLength(1)) - (isPortalize == PortalPosition.Forward ? 1 : 0); y++)
            {
                if (DepthTile[x + 1, y].IsPath == false || DepthTile[x + 1, y].IsObstacle || DepthTile[x + 1, y].IsEmpty == false) continue;

                Vector3 position = new Vector3(x, 0, y * 3);

                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.zero * 0, building.transform), 0);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 1, building.transform), 1);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 2, building.transform), 2);
            }
        }
    }


    private void GenerateBridgePickups(Building building, RoadLine startLine, RoadLine endLine)
    {
        for(int y = 1; y < 5; y++)
        {
            CreatePlaceable( milk, new Vector3( (int)startLine, y * 1.65f / 4, y - 1 ), building.transform );
        }


        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < (DepthTile.GetLength(1)); y++)
            {
                if (DepthTile[x + 1, y].IsPath == false || DepthTile[x + 1, y].IsObstacle || DepthTile[x + 1, y].IsEmpty == false) continue;

                Vector3 position = new Vector3(x, 1.65f, y * 3 + 4);

                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.zero * 0, building.transform), 0);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 1, building.transform), 1);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 2, building.transform), 2);
            }
        }


        for(int y = 1; y < 5; y++)
        {
            CreatePlaceable( milk, new Vector3( (int)endLine, 1.65f - (y * 1.65f / 4), y + building.GetSize().y + 3 ), building.transform );
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

    #endregion

    

    #region Extra's

    private Placeable CreatePlaceable(Placeable origin, Vector3 position, Transform parent, bool isInverse = false)
    {
        position.x *= 0.746f;
        position.z *= 0.746f;
        position.z += 0.373f;
        //position.y = 0.2f;

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


    private Obstacle GetCorrectObstacle(Building building, int x, int y, PortalPosition isPortalize)
    {
        Vector2Int buildingSize = building.GetSize();

        List<Obstacle> possibleObstacles = new List<Obstacle>();

        foreach( Obstacle obstacle in obstacles )
        {
            if (x + obstacle.Size.x + 1 > buildingSize.x || y * 3 + obstacle.Size.y - 1 > buildingSize.y) continue;

            if (IsObstacleCanPlaceHere(obstacle, x + 1, y) == false) continue;

            if (obstacle.IsCanPlaceHere(building, DepthTile, x, y, isPortalize) == false) continue;

            possibleObstacles.Add(obstacle);
        }

        return possibleObstacles.Random();
    }


    private bool IsObstacleCanPlaceHere(Obstacle obstacle, int x, int y)
    {
        for (int obsX = 0; obsX < obstacle.Size.x; obsX++)
        {
            for (int obsY = 0; obsY < Mathf.CeilToInt(obstacle.Size.y / 3f); obsY++)
            {
                if (obstacle.IsEmpty[obsX, obsY]) continue;

                if (DepthTile[obsX + x, obsY + y].IsPath) return false;
            }
        }

        return true;
    }

    #endregion
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Obstacler : ObstaclerBase
{
    [SerializeField] protected Obstacle[] obstacles;
    [SerializeField] protected Obstacle[] bridgeObstacles;
    [SerializeField] protected Milk milk;
    [SerializeField] protected Pickup[] pickups;
    [SerializeField] protected Portal portal;
    [SerializeField] protected PerfectJumpDetector jumpDetector;

    [SerializeField] protected int archMilkCount = 5;

    private LargeCell[,] DepthTile { get; set; }

    protected int milkChanсe = 100;



    private void Awake()
    {
        foreach(Obstacle prefab in obstacles)
        {
            prefab.Initialize();
        }
    }

    public RoadLine Generate(Building building, RoadLine startLine, bool isBridge = false)
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

        GeneratePath(building, startLine, isPortalize, isBridge, out var endLine);
        
        if(isBridge)
        {
            GenerateBridgeObstacles(building);
            GenerateBridgePickups(building, startLine, endLine);
        }
        else
        {
            Turner[] turner = building.GetComponentsInChildren<Turner>();

            if (turner.Length > 0)
            {
                GenerateObstaclesOnTurn(building, turner[0]);
                GeneratePickupOnTurn(building, GetPathLine( DepthTile.GetLength(1) / 2 ) , turner[0]);
            }
            else
            {
                GenerateObstacles(building, isPortalize);
                GeneratePickups(building, isPortalize);
            }

        }

        if ((Game.Mode.InParachuteMode == false && Game.Mode.InxAxIxRxMode == false) || Portal.IsWaitingForSecondPortal) GeneratePortals(building, isPortalize);
        //GenerateDetectors(building, isPortalize);
        //GenerateStuff(building, isPortalize);


        return endLine;
    }


    #region GenerationMethods

    public void GeneratePickupOnTurn(Building building, RoadLine line, Turner turner)
    {
        // === Generate Pickup's on a start lines === //

        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < (DepthTile.GetLength(1) / 2); y++)
            {
                if (DepthTile[x + 1, y].IsPath == false || DepthTile[x + 1, y].IsObstacle || DepthTile[x + 1, y].IsEmpty == false) continue;

                Vector3 position = new Vector3(x, 0, y * 3);

                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.zero * 0, building.transform), 0);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 1, building.transform), 1);
                DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 2, building.transform), 2);
            }
        }

        // === Generate Pickup's on a turn === //
        Vector3 startPoint = (turner.StartPoint.position) + turner.transform.rotation * Vector3.right * 0.746f * (int)line;
        Vector3 endPoint = (turner.EndPoint.position) + turner.transform.rotation * Vector3.forward * 0.746f * (int)line * (turner.Direction == Direction.Left? 1 : -1);

        float milkCount = 10 + (int)line * (turner.Direction == Direction.Left? 3 : -3);
        float distance = Vector3.Distance( startPoint, endPoint );

        for(float index = 1; index <= milkCount - 2; index++)
        {
            Vector3 position = building.transform.InverseTransformPoint(turner.Offset.position + turner.transform.rotation * Vector3.Slerp(turner.Offset.InverseTransformPoint(startPoint), turner.Offset.InverseTransformPoint(endPoint), index / milkCount));

            CreatePlaceable( milk, position, building.transform, isNeedToCorrectPosition: false );
        }

        // === Generate Pickup's on a end lines === //

        Quaternion rotation = Quaternion.Euler(0, (turner.Direction == Direction.Left ? -90 : 90), 0);

        for (int x = -1; x < 2; x++)
        {
            for (int y = (DepthTile.GetLength(1) / 2); y < (DepthTile.GetLength(1)); y++)
            {
                if (DepthTile[x + 1, y].IsPath == false || DepthTile[x + 1, y].IsObstacle || DepthTile[x + 1, y].IsEmpty == false) continue;

                Vector3 position = (turner.transform.localPosition) + rotation * new Vector3(x, 0, y * 3 - (DepthTile.GetLength(1) / 2 * 3) + 5.855927f) * 0.746f;

                for( int index = 0; index < 3; index++ )
                {
                    Placeable placeable = CreatePlaceable(milk, position + rotation * Vector3.forward * 0.746f * index, building.transform, isNeedToCorrectPosition: false);
                    placeable.transform.rotation = rotation;
                 
                    DepthTile[x + 1, y].AddPlaceable(placeable, 0);
                }
            }
        }
    }


    public void GenerateObstaclesOnTurn(Building building, Turner turner)
    {
        int nextBuildingIndex = 0;

        // === Generate Obstacles's on a start lines === //

        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < (DepthTile.GetLength(1) / 3); y++)
            {
                if (DepthTile[x + 1, y].IsPath == true || DepthTile[x + 1, y].IsEmpty == false) continue;

                if (nextBuildingIndex > 0)
                {
                    nextBuildingIndex--;

                    continue;
                }

                Vector3 position = new Vector3(x, 0, y * 3 + 1);

                Obstacle obstacle = GetCorrectObstacle(building, x, y, PortalPosition.None);

                if (x + obstacle.Size.x + 1 > DepthTile.GetLength(0) || y * 3 + obstacle.Size.y - 1 > DepthTile.GetLength(1) * 3) continue;

                if (obstacle != null)
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

                    nextBuildingIndex = Random.Range(1, 4) + 1;
                }
            }
        }

        // === Generate Obstacles's on a end lines === //

        Quaternion rotation = Quaternion.Euler(0, (turner.Direction == Direction.Left ? -90 : 90), 0);

        for (int x = -1; x < 2; x++)
        {
            for (int y = DepthTile.GetLength(1) / 3 * 2; y < (DepthTile.GetLength(1)); y++)
            {
                if (DepthTile[x + 1, y].IsPath == true || DepthTile[x + 1, y].IsEmpty == false) continue;

                if (nextBuildingIndex > 0)
                {
                    nextBuildingIndex--;

                    continue;
                }


                Vector3 position = (turner.transform.localPosition) + rotation * new Vector3(x, 0, y * 3 - (DepthTile.GetLength(1) / 2 * 3) + 6.855927f) * 0.746f + rotation * Vector3.forward * 0.373f;

                Obstacle obstacle = GetCorrectObstacle(building, x, y, PortalPosition.None);

                if (x + obstacle.Size.x + 1 > DepthTile.GetLength(0) || y * 3 + obstacle.Size.y - 1 > DepthTile.GetLength(1) * 3) continue;

                if (obstacle != null)
                {
                    Placeable placeable = CreatePlaceable(obstacle, position, building.transform, isNeedToCorrectPosition: false);
                    placeable.transform.localRotation = rotation;

                    for (int obsX = 0; obsX < obstacle.Size.x; obsX++)
                    {
                        for (int obsY = 0; obsY < Mathf.CeilToInt(obstacle.Size.y / 3f); obsY++)
                        {
                            if (obstacle.IsEmpty[obsX, obsY]) continue;

                            DepthTile[obsX + x + 1, obsY + y].AddPlaceable(placeable, 1);
                        }
                    }

                    nextBuildingIndex = Random.Range(1, 4) + 1;
                }
            }
        }
    }


    private void GeneratePath(Building building, RoadLine startLine, PortalPosition isPortalize, bool isBridge, out RoadLine endLine)
    {
        MilkPosition milkPosition = MilkPosition.HasNoMilkInBackward;
        RoadLine x = startLine;

        bool isAlreadyHasDoubleLine = false;
        
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

            if ((y + 1 < DepthTile.GetLength(1) || isPortalize != PortalPosition.Forward))
            {
                DepthTile[(int)x + 1, y].EnablePath();

                if (Random.Range(0, 100) < milkChanсe && (milkPosition != MilkPosition.HasADoubleLineInBackward || x != RoadLine.Venus))
                {
                    DepthTile[(int)x + 1, y].EnableMilk();

                    if( (milkPosition == MilkPosition.HasNoMilkInBackward || milkPosition == MilkPosition.HasADoubleLineInBackward) && x != RoadLine.Venus && y > 0 && y + 1 < DepthTile.GetLength(1) && (isAlreadyHasDoubleLine == false || milkPosition == MilkPosition.HasADoubleLineInBackward) && isPortalize == PortalPosition.None && isBridge == false)
                    {
                        if (x == RoadLine.Mercury)
                        {
                            DepthTile[(int)RoadLine.Earth + 1, y].EnablePath();
                            DepthTile[(int)RoadLine.Earth + 1, y].EnableMilk();
                        }
                        else if( x == RoadLine.Earth )
                        {
                            DepthTile[(int)RoadLine.Mercury + 1, y].EnablePath();
                            DepthTile[(int)RoadLine.Mercury + 1, y].EnableMilk();
                        }

                        milkPosition = MilkPosition.HasADoubleLineInBackward;
                        isAlreadyHasDoubleLine = true;
                    }
                    else
                    {
                        milkPosition = MilkPosition.HasMilkInBackward;
                    }

                    milkChanсe -= 20;
                }
                else
                {
                    milkChanсe += 60;

                    milkPosition = MilkPosition.HasNoMilkInBackward;
                }
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


    private void GenerateObstacles(Building building, PortalPosition isPortalize)
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

                        return;
                    }   
                }

                Vector3 position = new Vector3( x, 0, y * 3 + 1);

                if (x + obstacle.Size.x + 1 > DepthTile.GetLength(0) || y * 3 + obstacle.Size.y - 1 > DepthTile.GetLength(1) * 3) continue;

                if ( obstacle != null )
                {
                    Placeable placeable = CreatePlaceable(obstacle, position, building.transform);

                    for (int obsX = 0; obsX < obstacle.Size.x; obsX++)
                    {
                        for (int obsY = 0; obsY < Mathf.CeilToInt((float)obstacle.Size.y / 3f); obsY++)
                        {
                            if (obstacle.IsEmpty[obsX, obsY]) continue;

                            DepthTile[obsX + x + 1, obsY + y].AddPlaceable(placeable, 1);
                        }
                    }

                    nextBuildingIndex = Random.Range(2,4);
                }
            }
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
                if (DepthTile[x + 1, y].IsPath == false || DepthTile[x + 1, y].IsObstacle || DepthTile[x + 1, y].IsEmpty == false || DepthTile[x + 1,y].IsMilk == false)
                {
                    if (DepthTile[x + 1, y].IsEmpty == false || building.GetTileID(new Vector2Int(x + 1, y * 3 + 1)) == 0) continue;

                    Pickup pickup = pickups.Random();
                    int chance = Random.Range(0, 100);


                    if (pickup.IsCanPlace == true && pickup.chance > chance && isPortalize == PortalPosition.None)
                    {
                        Placeable placeable = CreatePlaceable(pickup, new Vector3(x, 0, y * 3 + 1), building.transform);
                        DepthTile[x + 1, y].AddPlaceable(placeable, 1);
                        
                        (placeable as Pickup).Init();
                    }
                }
                else
                {
                    Vector3 position = new Vector3(x, 0, y * 3);

                    DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.zero * 0, building.transform), 0);
                    DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 1, building.transform), 1);
                    DepthTile[x + 1, y].AddPlaceable(CreatePlaceable(milk, position + Vector3.forward * 2, building.transform), 2);
                }

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
            CreatePlaceable( milk, new Vector3( (int)endLine, 1.65f - (y * 1.65f / 4) + 0.3f, y + building.GetSize().y + 3 ), building.transform );
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
        
    }


    #endregion

    

    #region Extra's

    private Placeable CreatePlaceable(Placeable origin, Vector3 position, Transform parent, bool isInverse = false, bool isNeedToCorrectPosition = true)
    {
        if(isNeedToCorrectPosition)
        {
            position.x *= 0.746f;
            position.z *= 0.746f;
            position.z += 0.373f;
            //position.y = 0.2f;
        }

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


    // ===

    private Obstacle[] possibleObstacles;
    private Obstacle[] returnedObstacles;

    private Obstacle GetCorrectObstacle(Building building, int x, int y, PortalPosition isPortalize)
    {
        Vector2Int buildingSize = building.GetSize();

        possibleObstacles = new Obstacle[obstacles.Length];
        int lastIndex = 0;

        foreach( Obstacle obstacle in obstacles )
        {
            if (x + obstacle.Size.x + 1 > buildingSize.x || y * 3 + obstacle.Size.y - 1 > buildingSize.y) continue;

            if (IsObstacleCanPlaceHere(obstacle, x + 1, y) == false) continue;

            if (obstacle.IsCanPlaceHere(building, DepthTile, x, y, isPortalize) == false) continue;

            possibleObstacles[lastIndex] = obstacle;

            lastIndex++;
        }

        System.Array.Resize(ref possibleObstacles, lastIndex);

        Obstacle returnedObstacle = possibleObstacles[possibleObstacles.GetRandomIndexByChance()];

        return returnedObstacle;
    }


    private bool IsObstacleCanPlaceHere(Obstacle obstacle, int x, int y)
    {
        for (int obsX = 0; obsX < obstacle.Size.x; obsX++)
        {
            for (int obsY = 0; obsY < Mathf.CeilToInt((float)obstacle.Size.y / 3f); obsY++)
            {
                if (obstacle.IsEmpty[obsX, obsY]) continue;

                if (DepthTile[obsX + x, obsY + y].IsPath) return false;
            }
        }

        return true;
    }


    private enum MilkPosition
    {
        HasMilkInBackward,
        HasNoMilkInBackward,
        HasADoubleLineInBackward
    }


    #endregion
}
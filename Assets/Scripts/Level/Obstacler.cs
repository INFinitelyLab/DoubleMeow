using UnityEngine;
using System.Collections;
using System.Linq;

public class Obstacler : MonoBehaviour
{
    [SerializeField] private Obstacle[] _obstacles;
    [SerializeField] private Milk _milk;
    [SerializeField] private Pickup[] _pickups;
    [SerializeField] private Portal _portal;
    [SerializeField] private Accelerator _accelerator;
    [SerializeField] private PerfectJumpDetector _jumpDetector;

    public RoadLine Generate(Building building, RoadLine startLine, RoadLine endLine)
    {
        Vector2Int size = building.GetSize();

        Field field = new Field( size );

        endLine = field.GeneratePath( startLine , endLine , 3, building);
        StartCoroutine(field.GenerateObstacles(_obstacles, 30, building, _portal, _accelerator, _jumpDetector));
        StartCoroutine(field.GeneratePickups( _milk, _pickups , building));

        return endLine;
    }

    private class Field
    {
        private Cell[,] _cells;
        private bool[,] _path;


        public Field(Vector2Int size)
        {
            _cells = new Cell[ size.x, size.y ];
            _path = new bool[ size.x, size.y ];
        }


        public RoadLine GeneratePath(RoadLine start, RoadLine finish, int minDistanceAfterSurf, Building building)
        {
            //int maxDistanceAfterSurf = minDistanceAfterSurf + 3;

            RoadLine current = start;

            int fullLength = _path.GetLength(1);
            //int nextSurfIndex = Random.Range(minDistanceAfterSurf, maxDistanceAfterSurf);

            for( int y = 0; y < fullLength; y++ )
            {
                if (y % 3 == 0)
                {
                    RoadLine newCurrent = y + 3 >= fullLength? finish : (RoadLine)Random.Range(-1, 2);

                    if (building.GetTileDifficulty(new Vector2Int(newCurrent.ToInt() + 1, y)) != 0) current = newCurrent;
                }

                if (building.GetTileDifficulty(new Vector2Int(current.ToInt() + 1, y + 1)) == 0 && y < fullLength - 1)
                {
                    Direction direction = building.GetTileDifficulty(new Vector2Int(current.ToInt() + 2, y + 1)) == 0 ? Direction.Left : Direction.Right;

                    if (y + 1 < fullLength) _path[current.ToInt() + 1, y] = true;

                    current.TrySurf(direction);
                }


                int x = (int)current;

                _path[x + 1, y] = true;
            }

            return current;
        }


        public IEnumerator GenerateObstacles(Obstacle[] obstacles, int chanceToSpawn, Building building, Portal portal, Accelerator accelerator, PerfectJumpDetector detector)
        {
            if (building.IsPortalFriendly && Portal.IsAlreadyExist == false && Portal.IsAlreadyTransite == false)
            {
                if (Portal.IsWaitingForSecondPortal == false)
                {
                    Vector2Int position = new Vector2Int(Random.Range(0, 3), _cells.GetLength(1) - 3);

                    _cells[position.x, position.y] = new Cell(PlaceTo(position, portal, building.transform));

                    Portal.Initialize( new Vector3( position.x - 1, 0.33f, building.transform.position.z + position.y ) );
                }
                else
                {
                    Vector2Int position = new Vector2Int(Random.Range(0, 3), 1);

                    if (_cells[position.x, position.y] != null)
                        Destroy(_cells[position.x, position.y].Placeable.gameObject);

                    _cells[position.x, position.y] = new Cell(PlaceTo(position, portal, building.transform, true));

                    Portal.Initialize(new Vector3(position.x - 1, 0.33f, building.transform.position.z + position.y));
                }
            }
            else
            {
                for (int x = 0; x < _cells.GetLength(0); x++)
                {
                    for (int y = 0; y < _cells.GetLength(1); y++)
                    {
                        if (_path[x, y] == true || building.GetTileDifficulty(new Vector2Int(x, y)) < 2 || Random.Range(0, 100) > chanceToSpawn) continue;

                        _cells[x, y] = new Cell(PlaceTo(new Vector2Int(x, y), obstacles[obstacles.GetRandomIndexByChance()], building.transform));

                        yield return new WaitForFixedUpdate();

                        break;
                    }
                }

            }

            RoadLine[] lines = building.EndLines.ToRoadLine().ToArray();

            foreach(RoadLine line in lines)
            {
                Placeable placeable = Instantiate(detector, building.transform);

                placeable.transform.localPosition = new Vector3( line.ToInt() * 0.746f, 0.01f, building.EndPoint.transform.localPosition.z );
            }

            Vector2Int acceleratorPosition = new Vector2Int( Random.Range(0,3), Random.Range(0, _path.GetLength(1) / 3));

            /*if (IsCanPlaceAccelerator(acceleratorPosition.x, acceleratorPosition.y))
            {
                Placeable placeable = Instantiate(accelerator, building.transform);

                placeable.transform.localPosition = new Vector3( (acceleratorPosition.x - 1) * 0.746f, 0.01f, acceleratorPosition.y * 3 + 1.5f);
            }*/


        }


        public IEnumerator GeneratePickups( Milk milk, Pickup[] pickups , Building building)
        {
            Vector2Int lastPosition = Vector2Int.zero;

            for (int y = 0; y < _cells.GetLength(1); y++)
            {
                for (int x = 0; x < _cells.GetLength(0); x++)
                {
                    if ( _path[x,y] == true && building.GetTileDifficulty(new Vector2Int(x,y)) > 0 )
                    {
                        lastPosition.x = x; 
                        lastPosition.y = y;

                        Pickup pickup = pickups.ToList().Random();

                        if (building.GetTileDifficulty(new Vector2Int(x, y)) > 1 && pickup.IsCanPlace && Random.Range(0, 100) < pickup.chance)
                        {
                            PlaceTo(lastPosition, pickup , building.transform);
                        }
                        else
                        {
                            PlaceTo(lastPosition, milk, building.transform);
                        }
                    }

                    yield return new WaitForFixedUpdate();
                }
            }
        }


        private Placeable PlaceTo( Vector2Int position, Placeable origin, Transform parent, bool isReverse = false )
        {
            Placeable placeable = Instantiate(origin, parent);

            placeable.transform.localPosition = new Vector3( (position.x - 1) * 0.746f , 0.33f , (position.y + 0.5f) * 1f );
            placeable.transform.localRotation = Quaternion.Euler(0f, isReverse ? 180 : 0, 0);

            return placeable;
        }


        private class Cell
        {
            public Placeable Placeable { get; private set; }

            public bool IsEmpty => Placeable == null;

            public Cell(Placeable placeable)
            {
                Placeable = placeable;
            }
        }

        
        private bool IsCanPlaceAccelerator(int x, int quarterY)
        {
            if (quarterY < 0 && quarterY >= _path.GetLength(1) / 3) return false;

            quarterY++;

            for( int y = quarterY * 3 - 3; y < quarterY * 3; y++ )
            {
                if (_path[x, y] == false) return false;
            }

            return true;
        }
    }
}
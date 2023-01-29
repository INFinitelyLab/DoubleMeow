using UnityEngine;

public class RetroObstacler : ObstaclerBase
{
    [SerializeField] private Obstacle[] _obstacles;
    [SerializeField] private Milk _milk;

    private LargeCell[,] _cells;
    private Vector2Int _size;

    private int _nextSurfY;
    private int _nextObstacleDistance;


    public RoadLine Generate(Retrobuilding building, RoadLine line)
    {
        _size = building.GetSize();
        _size.y /= 3;

        _cells = new LargeCell[_size.x, _size.y];

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                _cells[x, y] = new LargeCell();
            }
        }

        GeneratePath(line, out RoadLine endLine);
        GenerateObstacles(building);
        GenerateMilk(building);

        return endLine;
    }

    private void GeneratePath(RoadLine startLine, out RoadLine endLine)
    {
        RoadLine currentLine = startLine;

        for(int y = 0; y < _size.y; y++)
        {
            if (_nextSurfY <= 0)
            {
                _nextSurfY = Random.Range(0, 2);

                int surfValue = 0;

                surfValue += currentLine == RoadLine.Mercury ? 1 : 0;
                surfValue += currentLine == RoadLine.Earth ? 2 : 0;

                if (surfValue == 0)
                    currentLine.TrySurfRandom();
                else
                    currentLine.TrySurf( surfValue == 1? Direction.Right : Direction.Left );
            }
            else
            {
                _nextSurfY--;
            }

            _cells[(int)currentLine + 1, y].EnablePath();
        }

        endLine = currentLine;
    }


    private void GenerateObstacles(Retrobuilding building)
    {
        Vector3 position = Vector3.zero;

        for( int x = 0; x < _size.x; x++ )
        {
            for( int y = 0; y < _size.y; y++ )
            {
                _nextObstacleDistance--;

                if (_nextObstacleDistance > 0) continue;

                if (_cells[x, y].IsPath == true) continue;

                if (_cells[x, y].IsEmpty == false) continue;

                _nextObstacleDistance = Random.Range(3,6);

                position.x = x - 1;
                position.z = y * 3 + 1.5f;

                position *= 0.746f;

                _cells[x, y].AddPlaceable( CreateObstacle( _obstacles.Random(), position, building.transform ), 1 );
            }
        }

    }


    private void GenerateMilk(Retrobuilding building)
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = building.transform.rotation;

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (_cells[x, y].IsPath == false) continue;

                position.x = x - 1;
                position.z = y * 3 + 0.5f;

                position *= 0.746f;

                CreateObstacle( _milk, position + rotation * Vector3.forward * 0.000f, building.transform );
                CreateObstacle( _milk, position + rotation * Vector3.forward * 0.746f, building.transform );
                CreateObstacle( _milk, position + rotation * Vector3.forward * 1.492f, building.transform );
            }
        }
    }


    private Placeable CreateObstacle(Placeable origin, Vector3 position, Transform parent)
    {
        Placeable placeable = Instantiate(origin, parent);

        placeable.transform.localPosition = position;

        return placeable;
    }

}
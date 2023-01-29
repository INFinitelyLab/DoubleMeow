using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Building : MonoBehaviour, IGroundeable
{
    [SerializeField] private RoadLineFlags _startLines;
    [SerializeField] private RoadLineFlags _endLines;
    [SerializeField] private Transform _pigeonPosition;
    [SerializeField] private GameObject _pigeonPrefab;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private bool _isNearly;
    [SerializeField] private bool _isCanConnectToVehicle;
    [SerializeField] [Multiline(15)] private string _tileIDs;
    [SerializeField] private int _maxIdentityCount = 3;
    [SerializeField] private bool _isPortalFriendly = false;
    [SerializeField] private bool _isObstacleInCenter = false;
    [SerializeField] private float _placeOffset;

    private string[] _tileLines;

    private Transform _transform;
    private Vector3 _targetPosition;

    public RoadLineFlags StartLines => _startLines;
    public RoadLineFlags EndLines => _endLines;
    public Transform EndPoint => _endPoint;
    public bool IsNearly => _isNearly;
    public bool IsCanConnectToVehicle => _isCanConnectToVehicle;
    public int MaxIdentityCount => _maxIdentityCount;
    public bool IsPortalFriendly => _isPortalFriendly;
    public bool IsObstacleInCenter => _isObstacleInCenter;
    public float PlaceOffset => _placeOffset;
    

    private static float DecorationBuildingHeight = 5f;
    private static float DecorationBuildingDistanceToBuilding = 4;
    private static float DecorationBulidingDistance = 3;


    public int GetTileID(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= GetSize().x || position.y >= GetSize().y)
            return 0;

        string line = _tileLines[position.y];

        return line[position.x] - 48;
    }


    public Vector2Int GetSize()
    {
        if( _tileLines == null) _tileLines = _tileIDs.Split("\n");

        return new Vector2Int(_tileIDs.Split("\n")[0].Length, _tileIDs.Split("\n").Length);
    }


    private void Awake()
    {
        _tileLines = _tileIDs.Split("\n");

        _transform = transform;

        if (_pigeonPosition != null) SpawnBird();
    }


    protected virtual void SpawnBird()
    {
        if (Random.Range(0, 4) == 2)
        {
            GameObject pi = Instantiate(_pigeonPrefab, _pigeonPosition);

            pi.transform.localPosition = Vector3.zero;
        }
    }


    public virtual void Animate()
    {
        gameObject.SetActive(false);

        _targetPosition = _transform.localPosition;

        _transform.localPosition += Vector3.down * 5;
    }


    protected virtual void Update()
    {
        if (_targetPosition != Vector3.zero ) _transform.localPosition = Vector3.Lerp( _transform.localPosition, _targetPosition, 10 * Time.deltaTime );
    }


    // Static

    public static float PlaceDecorations(float height, float startPoint, Vector3 position, Quaternion rotation, float width, List<DecorationBuilding> _prefabs, int type)
    {
        float point = startPoint;
        float endPoint = width;

        position.y = 0;

        while( point < endPoint )
        {
            float distance = endPoint - point;


            DecorationBuilding origin = _prefabs.Where(p => p.Width < distance).ToList().Random();

            if (origin == null) return 0;


            if ((type == 2 || type == 3) && point == startPoint)
            {
                DecorationBuilding decor3 = Instantiate(origin);
                DecorationBuilding decor4 = Instantiate(origin);

                decor3.transform.localPosition = position + rotation * new Vector3( DecorationBuildingDistanceToBuilding + 2, height + DecorationBuildingHeight + Random.Range(-3, 0.8f), point + 1);
                decor4.transform.localPosition = position + rotation * new Vector3( -DecorationBuildingDistanceToBuilding - 2, height + DecorationBuildingHeight + Random.Range(-3, 0.8f), point + 1);

                decor3.transform.localRotation = Quaternion.Euler(0, -90 + rotation.eulerAngles.y, 0);
                decor4.transform.localRotation = Quaternion.Euler(0, -270 + rotation.eulerAngles.y, 0);

                decor3.transform.localScale = new Vector3(-1, 1, -1);
                decor4.transform.localScale = new Vector3(1, 1, -1);
            }


            DecorationBuilding decor = Instantiate( origin );
            DecorationBuilding decor2 = Instantiate( origin );

            decor.transform.localPosition = position + rotation * new Vector3( DecorationBuildingDistanceToBuilding, height + DecorationBuildingHeight + Random.Range(-3,0.8f), point );
            decor2.transform.localPosition = position + rotation * new Vector3(-DecorationBuildingDistanceToBuilding, height + DecorationBuildingHeight + Random.Range(-3, 0.8f), point);

            decor.transform.localScale = new Vector3(-1,1,1);

            decor.transform.localRotation = rotation;
            decor2.transform.localRotation = rotation;

            point += decor.Width + DecorationBulidingDistance;
        }

        return type == 1? 0 : (point - endPoint);
    }


    public static float PlaceDecorationsForTurn(float height, float startPoint, Turner turner, Vector3 position, Quaternion rotation, List<DecorationBuilding> _prefabs, DecorationBuilding _turnPrefab, bool isLeft)
    {
        Vector3 insidePoint = turner.Offset.position + rotation * new Vector3(isLeft? 2 : -3, 0, 1);

        startPoint = startPoint > DecorationBulidingDistance ? startPoint - DecorationBulidingDistance : 0;
        insidePoint.y = 0;
        position.y = 0;

        //=== Inside building's ===//

        DecorationBuilding box = Instantiate( _turnPrefab, insidePoint + Vector3.up * (height + DecorationBuildingHeight + Random.Range(-3, 0.8f)), turner.transform.rotation); // Main building

        box.transform.localScale = new Vector3( isLeft ? 1 : -1, 1 , 1 );

        DecorationBuilding originBetween = _prefabs.Where(p => p.Width > 4).ToList().Random();
        DecorationBuilding originBefore = _prefabs.Where(p => p.Width < (3 - startPoint)).ToList().Random();
        DecorationBuilding originAfter = _prefabs.Where(p => p.Width < 4).ToList().Random();

        /*if (originBetween != null)
        {
            DecorationBuilding between = Instantiate(originBetween, position + rotation * new Vector3(isLeft? -2.16f : 2.16f, 3.73f, 22.17f), Quaternion.Euler(0, rotation.eulerAngles.y - 90, 0));
            between.transform.localScale = new Vector3(-1, 1, isLeft ? 1 : -1);
        }

        if (originAfter != null)
        {
            DecorationBuilding after = Instantiate(originAfter, insidePoint - (rotation) * new Vector3((isLeft ? 5.5f : -5.5f), -(height + DecorationBuildingHeight + Random.Range(-3, 0.8f)), 1), Quaternion.Euler(0, rotation.eulerAngles.y + (turner.Direction == Direction.Left ? -90 : 90), 0));
            after.transform.localScale = new Vector3(isLeft ? 1 : -1, 1, 1);
        }*/
        if (originBefore != null)
        {
            DecorationBuilding before = Instantiate(originBefore, insidePoint - (rotation) * new Vector3(isLeft ? 1 : -1, -(height + DecorationBuildingHeight + Random.Range(-3, 0.8f)), 7.5f), Quaternion.Euler(0, rotation.eulerAngles.y, 0)); // Before main building
            before.transform.localScale = new Vector3( isLeft? 1 : -1, 1, 1 );
        }

        //=== Outside building's ===//

        float currentPoint = 10;

        DecorationBuilding building = null;
        DecorationBuilding origin = _prefabs.Where(p => p.Width < currentPoint - startPoint).ToList().Random();
        Vector3 offset = new Vector3( isLeft? DecorationBuildingDistanceToBuilding : -DecorationBuildingDistanceToBuilding, (height + DecorationBuildingHeight + Random.Range(-3, 0.8f)), currentPoint );

        if( origin != null )
        {
            offset.z -= origin.Width;
        }

        while( origin != null )
        {
            building = Instantiate( origin, position + rotation * offset, rotation );
            building.transform.localScale = new Vector3( isLeft? -1 : 1, 1, 1 );

            currentPoint -= origin.Width + DecorationBulidingDistance;
            offset.z = currentPoint - origin.Width;

            origin = _prefabs.Where( p => p.Width < currentPoint - startPoint ).ToList().Random();
        }

        currentPoint = 6;

        building = null;
        origin = _prefabs.Where(p => p.Width < 13 - currentPoint).ToList().Random();
        offset = new Vector3( (currentPoint) * (isLeft? -1 : 1), (height + DecorationBuildingHeight + Random.Range(-3, 0.8f)), DecorationBuildingDistanceToBuilding + 16 );

        /*while( origin != null )
        {
            building = Instantiate(origin, position + rotation * offset, Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + (isLeft ? -90 : 90), rotation.eulerAngles.z));
            building.transform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);

            currentPoint += origin.Width + 1;
            offset.x = (currentPoint) * (isLeft? -1 : 1);

            origin = _prefabs.Where(p => p.Width < 13 - currentPoint).ToList().Random();
        }*/

        return 0;
    }
}
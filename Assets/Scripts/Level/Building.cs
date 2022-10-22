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

    private static float DecorationBuildingHeight = 5f;
    private static float DecorationBuildingDistanceToBuilding = 4;
    private static Vector2 DecorationBulidingMinMaxDistance = new Vector2( 1f, 1.5f );


    public int GetTileDifficulty(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x > 2 || position.y > _tileLines.Length-1)
            return 0;

        string line = _tileLines[position.y];

        return line[position.x] - 48;
    }


    public Vector2Int GetSize()
    {
        return new Vector2Int(3, _tileIDs.Split("\n").Length);
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


    private void Update()
    {
        if (_targetPosition != Vector3.zero ) _transform.localPosition = Vector3.Lerp( _transform.localPosition, _targetPosition, 10 * Time.deltaTime );
    }


    // Static

    public static float PlaceDecorations(float startPoint, float positionZ, float width, List<DecorationBuilding> _prefabs, int type)
    {
        float point = startPoint;
        float endPoint = width;

        while( point < endPoint )
        {
            float distance = type == 1 || type == 3? endPoint - point : 999;


            DecorationBuilding origin = _prefabs.Where(p => p.Width < distance).ToList().Random();

            if (origin == null) return 0;


            if ((type == 2 || type == 3) && point == startPoint)
            {
                DecorationBuilding decor3 = Instantiate(origin);
                DecorationBuilding decor4 = Instantiate(origin);

                decor3.transform.localPosition = new Vector3( DecorationBuildingDistanceToBuilding + 2, DecorationBuildingHeight + Random.Range(-3, 0.8f), positionZ + point + 1);
                decor4.transform.localPosition = new Vector3( -DecorationBuildingDistanceToBuilding - 2, DecorationBuildingHeight + Random.Range(-3, 0.8f), positionZ + point + 1);

                decor3.transform.localRotation = Quaternion.Euler(0, -90, 0);
                decor4.transform.localRotation = Quaternion.Euler(0, -270, 0);

                decor3.transform.localScale = new Vector3(-1, 1, -1);
                decor4.transform.localScale = new Vector3(1, 1, -1);
            }


            DecorationBuilding decor = Instantiate( origin );
            DecorationBuilding decor2 = Instantiate( origin );

            decor.transform.localPosition = new Vector3( DecorationBuildingDistanceToBuilding, DecorationBuildingHeight + Random.Range(-3,0.8f), positionZ + point );
            decor2.transform.localPosition = new Vector3(-DecorationBuildingDistanceToBuilding, DecorationBuildingHeight + Random.Range(-3, 0.8f), positionZ + point);

            decor.transform.localScale = new Vector3(-1,1,1);

            point += decor.Width + Random.Range( DecorationBulidingMinMaxDistance.x , DecorationBulidingMinMaxDistance.y );
        }

        return type == 1? 0 : (point - endPoint);
    }
}
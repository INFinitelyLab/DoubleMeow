using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class Curver : MonoBehaviour
{
    [SerializeField] private CurveObstacler _obstacler;
    [SerializeField] private CurveTrigger _curveTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Curve[] _prefabs;
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private Curve _startBuilding;
    [SerializeField] private float _generationDistance;
    [SerializeField] private float _targetMetroDistance;

    private Quaternion _rotation;
    private Transform _player;
    private int _line;

    private List<Curve> _pool;

    private float _distance;
    private Vector3 _startPosition;

    private float _decoEndPoint;

    public System.Action<Vector3> EndGenerate;

    public bool IsEnabled { get; private set; }


    public void Spawn(System.Action<Vector3> OnEndMethod, float decoEndPoint, bool isPreviousVehicle, Vector3 position, Quaternion rotation)
    {
        position.y = 0;

        _player = Player.Movement.transform;

        _pool = new List<Curve>();

        _distance = 0;

        _rotation = rotation;

        _line = 0;

        EndGenerate = OnEndMethod;
        _decoEndPoint = decoEndPoint;

        _startPosition = position;

        _regenTrigger.Triggered += Regenerate;

        CreateNewBuilding(_startBuilding, position, rotation);
        _decoEndPoint = Building.PlaceDecorations(0, _decoEndPoint, position, rotation, _startBuilding.EndPoint.transform.localPosition.z, _decoPrefabs, isPreviousVehicle ? 2 : 1);

        _distance += _startBuilding.EndPoint.transform.localPosition.z;

        IsEnabled = true;
    }


    private Curve CreateNewBuilding(Curve origin, Vector3 position, Quaternion rotation)
    {
        Curve building = Instantiate(origin, position, rotation);

        return building;
    }


    public void Regenerate()
    {
        Debug.Log("Player Distance: " + Player.Movement.GetDistanceTo(_startPosition));
        float targetDistance = Player.Movement.GetDistanceTo(_startPosition) + _generationDistance;

        Curve building = null;

        while (_distance < targetDistance)
        {
            building = CreateNewBuilding(_prefabs.Random(), (_rotation * (Vector3.forward * _distance)) + _startPosition, _rotation);

            _pool.Add(building);

            _line = _obstacler.Generate(building, _line);

            _decoEndPoint = Building.PlaceDecorations(0, _decoEndPoint, (_rotation * (Vector3.forward * _distance)) + _startPosition, _rotation, building.EndPoint.transform.localPosition.z, _decoPrefabs, 1);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo( Player.Movement.transform.position + _rotation * Vector3.forward * (2 * Game.Difficulty));

        if (_distance >= _targetMetroDistance)
        {
            CurveTrigger cT = Instantiate(_curveTrigger, _pool.Last().transform);

            cT.transform.localPosition = _pool.Last().EndPoint.localPosition;

            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke((_rotation * (Vector3.forward * _distance)) + _startPosition);

            IsEnabled = false;
        }
    }


    public void Disable()
    {
        IsEnabled = false;

        _regenTrigger.Triggered -= Regenerate;

        EndGenerate?.Invoke(default);
    }
}

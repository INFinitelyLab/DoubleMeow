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

    private Transform _player;

    private List<Curve> _pool;

    private float _distance;
    private float _startPositionZ;

    private float _decoEndPoint;

    public System.Action<float> EndGenerate;


    public void Spawn(float fromZ, System.Action<float> OnEndMethod, float decoEndPoint, bool isPreviousVehicle)
    {
        _player = Player.Movement.transform;

        _pool = new List<Curve>();

        _distance = 0;

        EndGenerate = OnEndMethod;
        _decoEndPoint = decoEndPoint;

        _startPositionZ = fromZ;
        
        _regenTrigger.Triggered += Regenerate;
        
        CreateNewBuilding( _startBuilding, Vector3.forward * fromZ );
        _decoEndPoint = Building.PlaceDecorations(_decoEndPoint, _distance + _startPositionZ, _startBuilding.EndPoint.transform.localPosition.z, _decoPrefabs, isPreviousVehicle? 2 : 1);

        _startPositionZ += _startBuilding.EndPoint.transform.localPosition.z;
    }


    private Curve CreateNewBuilding(Curve origin, Vector3 position)
    {
        Curve building = Instantiate(origin, position, Quaternion.identity, transform);

        return building;
    }


    public void Regenerate()
    {
        float targetDistance = _player.position.z - _startPositionZ + _generationDistance;

        Curve building = null;

        while (_distance < targetDistance)
        {
            building = CreateNewBuilding(_prefabs.Random(), Vector3.forward * (_distance + _startPositionZ));

            _pool.Add(building);

            _obstacler.Generate(building, RoadLine.Mercury);

            _decoEndPoint = Building.PlaceDecorations(_decoEndPoint, _distance + _startPositionZ, building.EndPoint.transform.localPosition.z, _decoPrefabs, 1);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo( Player.Movement.transform.localPosition.z + 2 * Game.Difficulty);

        if (_distance >= _targetMetroDistance)
        {
            CurveTrigger cT = Instantiate(_curveTrigger, _pool.Last().transform);

            cT.transform.localPosition = _pool.Last().EndPoint.localPosition;

            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke(_distance + _startPositionZ);
        }
    }
}

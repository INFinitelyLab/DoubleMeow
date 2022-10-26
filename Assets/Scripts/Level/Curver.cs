using UnityEngine;
using System.Collections.Generic;


public class Curver : MonoBehaviour
{
    [SerializeField] private CurveTrigger _curveTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Building[] _prefabs;
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private Building _startBuilding;
    [SerializeField] private float _generationDistance;
    [SerializeField] private float _targetMetroDistance;

    private Transform _player;

    private float _distance;
    private float _startPositionZ;

    private float _decoEndPoint;

    public System.Action<float> EndGenerate;


    public void Spawn(float fromZ, System.Action<float> OnEndMethod, float decoEndPoint)
    {
        _player = Player.Movement.transform;

        CreateNewBuilding( _startBuilding, Vector3.forward * fromZ );

        EndGenerate = OnEndMethod;
        _decoEndPoint = decoEndPoint;

        _startPositionZ = fromZ;
        _regenTrigger.Triggered += Regenerate;
    }


    private Building CreateNewBuilding(Building origin, Vector3 position)
    {
        Building building = Instantiate(origin, position, Quaternion.identity, transform);

        return building;
    }


    public void Regenerate()
    {
        float targetDistance = _player.position.z - _startPositionZ + _generationDistance;

        Building building = null;

        while (_distance < targetDistance)
        {
            building = CreateNewBuilding(_prefabs.Random(), Vector3.forward * (_distance + _startPositionZ));

            _decoEndPoint = Building.PlaceDecorations(_decoEndPoint, _distance + _startPositionZ, building.EndPoint.transform.localPosition.z, _decoPrefabs, 1);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo( Player.Movement.transform.localPosition.z + 2 * Game.Difficulty);

        if (_distance >= _targetMetroDistance)
        {
            CurveTrigger cT = Instantiate(_curveTrigger, building.transform);

            cT.transform.localPosition = building.EndPoint.localPosition;

            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke(_distance + _startPositionZ);
        }
    }
}

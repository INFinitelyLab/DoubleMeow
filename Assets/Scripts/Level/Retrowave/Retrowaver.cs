using UnityEngine;


public class Retrowaver : MonoBehaviour
{
    [SerializeField] private RetrowaveDecorator _decorator;
    [SerializeField] private RetroObstacler _obstacler;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Retrobuilding[] _prefabs;
    [SerializeField] private Retrobuilding _startBuilding;
    [SerializeField] private float _generationDistance;
    [SerializeField] private float _targetMetroDistance;

    private Quaternion _rotation;
    private Transform _player;
    private RoadLine _line;

    private float _distance;
    private Vector3 _startPosition;

    public System.Action<Vector3, RoadLine> EndGenerate;

    public bool IsEnabled { get; private set; }


    public void Spawn(System.Action<Vector3, RoadLine> OnEndMethod, RoadLine line, Vector3 position, Quaternion rotation)
    {
        position.y = 0;

        _player = Player.Movement.transform;

        _distance = 0;

        _rotation = rotation;

        _line = line;

        EndGenerate = OnEndMethod;

        _startPosition = position;

        _regenTrigger.Triggered += Regenerate;

        IsEnabled = true;

        Regenerate();
    }


    private Retrobuilding CreateNewBuilding(Retrobuilding origin, Vector3 position, Quaternion rotation)
    {
        Retrobuilding building = Instantiate(origin, position, rotation);

        return building;
    }

    Retrobuilding building;

    public void Regenerate()
    {
        float targetDistance = Vector3.Distance(_player.position, _startPosition) + _generationDistance;

        while (_distance < targetDistance)
        {
            building = CreateNewBuilding(_prefabs.Random(), (_rotation * (Vector3.forward * _distance)) + _startPosition, _rotation);

            //_decorator.Decorate(building);

            _line = _obstacler.Generate(building, _line);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo(Player.Movement.transform.position + _rotation * Vector3.forward * (2 * Game.Difficulty));

        if (_distance >= _targetMetroDistance)
        {
            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke((_rotation * (Vector3.forward * _distance)) + _startPosition, _line);

            IsEnabled = false;
        }
    }


    public void Disable()
    {
        IsEnabled = false;

        _regenTrigger.Triggered -= Regenerate;

        EndGenerate?.Invoke(default, RoadLine.Venus);
    }
}

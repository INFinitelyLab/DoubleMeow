using UnityEngine;


public class MiniRetrowaver : MiniGame
{
    [SerializeField] private RetrowaveDecorator _decorator;
    [SerializeField] private RetroObstacler _obstacler;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Retrobuilding[] _prefabs;
    [SerializeField] private Retrobuilding _startBuilding;
    [SerializeField] private float _generationDistance;

    private Transform _player;
    private RoadLine _line;

    private float _distance;


    public override void Enable()
    {
        _player = Player.Movement.transform;

        _distance = 0;

        Regenerate();

        _regenTrigger.Triggered += Regenerate;
    }


    private Retrobuilding CreateNewBuilding(Retrobuilding origin, Vector3 position)
    {
        Retrobuilding building = Instantiate(origin, position, Quaternion.identity);

        return building;
    }

    Retrobuilding building;

    public void Regenerate()
    {
        float targetDistance = _player.position.magnitude + _generationDistance;

        while (_distance < targetDistance)
        {
            building = CreateNewBuilding(_prefabs.Random(), Vector3.forward * _distance);

            if ( _distance > 20 ) _line = _obstacler.Generate(building, _line);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo(Player.Movement.transform.position + Vector3.forward * (2 * Game.Difficulty));
    }


    public void Disable()
    {
        _regenTrigger.Triggered -= Regenerate;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class MiniCurver : MiniGame
{
    [SerializeField] private CurveObstacler _obstacler;
    [SerializeField] private CurveTrigger _curveTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Curve[] _prefabs;
    [SerializeField] private Curve _startCurve;
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private float _generationDistance;

    private Transform _player;
    private int _line;

    private float _distance;

    private float _decoEndPoint;



    public override void Enable()
    {
        _player = Player.Movement.transform;

        _distance = 0;

        _line = 0;

        _regenTrigger.Triggered += Regenerate;

        Regenerate();

        Game.Mode.EnableCurveMode();
    }



    private Curve CreateNewBuilding(Curve origin, Vector3 position)
    {
        Curve building = Instantiate(origin, position, Quaternion.identity, transform);

        return building;
    }


    public void Regenerate()
    {
        float targetDistance = _player.position.magnitude + _generationDistance;

        Curve building = null;

        while (_distance < targetDistance)
        {
            if (_distance < 10)
                building = CreateNewBuilding(_startCurve, Vector3.forward * _distance);
            else
                building = CreateNewBuilding(_prefabs.Random(), Vector3.forward * _distance);

            _line = _obstacler.Generate(building, _line);

            _decoEndPoint = Building.PlaceDecorations(0, _decoEndPoint, Vector3.forward * _distance, Quaternion.identity, building.EndPoint.transform.localPosition.z, _decoPrefabs, 1);

            _distance += building.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo(Player.Movement.transform.position + Vector3.forward * (2 * Game.Difficulty));
    }
}

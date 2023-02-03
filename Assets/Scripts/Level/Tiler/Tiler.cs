using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tiler : MonoBehaviour
{
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private TilerTrigger _tilerTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Tile _tile;
    [SerializeField] private float _generationDistance;
    [SerializeField] private float _targetDistance;

    private Transform _player;

    private Quaternion _rotation;
    private Vector3 _startPosition;
    private float _distance;

    private float _decoEndPoint;

    private RoadLine _line;
    private int _nextSurfDistance;

    public System.Action<Vector3, RoadLine> EndGenerate;

    public bool IsEnabled { get; private set; }


    private void Awake()
    {
        _player = Player.Movement.transform;
    }


    public void Spawn(Vector3 from, Quaternion rotation, System.Action<Vector3, RoadLine> OnEndMethod, float decoEndPoint, RoadLine line)
    {
        from.y = 0;

        _player = Player.Movement.transform;

        _line = line;
        _rotation = rotation;
        _startPosition = from;
        _decoEndPoint = decoEndPoint;

        EndGenerate = OnEndMethod;

        _distance = 0;

        _nextSurfDistance = 3;

        _regenTrigger.Triggered += Regenerate;

        CreateTrigger(from, true, rotation);

        _decoEndPoint = Building.PlaceDecorations(0, _decoEndPoint, from, rotation, _targetDistance, _decoPrefabs, 1);
    }


    private Tile CreateNewTile(Tile origin, Vector3 position)
    {
        Tile tile = Instantiate(origin, position, _rotation);

        StartCoroutine(WaitForActive(_player, tile.transform));

        return tile;
    }


    private void CreateTrigger(Vector3 position, bool isNeedToEnable, Quaternion rotation)
    {
        TilerTrigger trigger = Instantiate(_tilerTrigger, transform);

        trigger.transform.position = position;
        trigger.transform.rotation = rotation;
        trigger.isNeedToEnableTilerMode = isNeedToEnable;
    }


    public IEnumerator WaitForActive(Transform player, Transform tile)
    {
        bool isActive = true;

        while (isActive)
        {
            if (Vector3.Distance(tile.position, player.position) < _generationDistance)
            {
                isActive = false;

                tile.gameObject.SetActive(true);
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }

        isActive = true;

        while (isActive)
        {
            if (Vector3.Distance(tile.position, player.position) > _generationDistance)
            {
                isActive = false;

                Destroy(tile.gameObject);
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }


    public void Regenerate()
    {
        float targetDistance = Mathf.Clamp(Player.Movement.GetDistanceTo(_startPosition) + 35, 0, _targetDistance);

        Tile tile = null;

        while (_distance < targetDistance)
        {
            _nextSurfDistance--;

            tile = CreateNewTile(_tile, _startPosition + _rotation * (Vector3.forward * (_distance) + Vector3.right * (int)_line * 0.746f));

            if (_nextSurfDistance <= 0)
            {
                _nextSurfDistance = Random.Range(2, 5);

                if (_line == RoadLine.Venus)
                    _line.TrySurfRandom();
                else
                    _line.TrySurf(_line == RoadLine.Mercury ? Direction.Right : Direction.Left);

                tile = CreateNewTile(_tile, _startPosition + _rotation * (Vector3.forward * (_distance) + Vector3.right * (int)_line * 0.746f));
            }


            _distance += 0.746f;
        }

        _regenTrigger.MoveTo(Player.Movement.transform.localPosition + _rotation * Vector3.forward * 0.746f);

        if (_distance >= _targetDistance)
        {
            _regenTrigger.Triggered -= Regenerate;

            CreateTrigger(_rotation * Vector3.forward * _distance + _startPosition, false, _rotation);

            EndGenerate?.Invoke(_rotation * Vector3.forward * _distance + _startPosition, _line);
        }
    }


    public void Disable()
    {
        IsEnabled = false;

        _regenTrigger.Triggered -= Regenerate;

        EndGenerate?.Invoke(default, RoadLine.Venus);
    }
}
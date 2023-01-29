using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MiniTiler : MiniGame
{
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Tile _tile;
    [SerializeField] private float _generationDistance;

    private Transform _player;

    private float _distance;

    private float _decoEndPoint;

    private RoadLine _line;
    private int _nextSurfDistance;

    private Vector3 _lastDecoPosition = Vector3.zero;


    private void Start()
    {
        _player = Player.Movement.transform;
    }


    public override void Enable()
    {
        _line = RoadLine.Venus;

        _player = Player.Movement.transform;

        _distance = 0;

        _nextSurfDistance = 15;

        _regenTrigger.Triggered += Regenerate;

        Player.Camera.EnableTilerMode();

        Regenerate();
    }


    private Tile CreateNewTile(Tile origin, Vector3 position)
    {
        Tile tile = Instantiate(origin, position, Quaternion.identity, transform);

        StartCoroutine(WaitForActive(_player, tile.transform));

        return tile;
    }


    private void FixedUpdate()
    {
        if (_player.position.y < -0.5f && Game.InMiniGames)
            if (Game.IsActive) Game.Stop();
    }


    public IEnumerator WaitForActive(Transform player, Transform tile)
    {
        bool isActive = true;

        while (isActive)
        {
            if (tile == null || player == null) yield break;

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

                if (Game.IsActive) Destroy(tile.gameObject);
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void Regenerate()
    {
        float targetDistance = _player.position.magnitude + 35;

        Tile tile = null;

        while (_distance < targetDistance)
        {
            _nextSurfDistance--;

            tile = CreateNewTile(_tile, (Vector3.forward * (_distance) + Vector3.right * (int)_line * 0.746f));

            if (_distance < _generationDistance) tile.Endplace();

            if (_nextSurfDistance <= 0)
            {
                _nextSurfDistance = Random.Range(2, 5);

                if (_line == RoadLine.Venus)
                    _line.TrySurfRandom();
                else
                    _line.TrySurf(_line == RoadLine.Mercury ? Direction.Right : Direction.Left);

                tile = CreateNewTile(_tile, (Vector3.forward * (_distance) + Vector3.right * (int)_line * 0.746f));
            }

            _distance += 0.746f;
        }

        while( Mathf.Abs(_lastDecoPosition.z - _player.position.z) < 35 )
        {
            _decoEndPoint = Building.PlaceDecorations(0, _decoEndPoint, _lastDecoPosition, Quaternion.identity, 35, _decoPrefabs, 1);

            _lastDecoPosition.z += 35;
        }

        _regenTrigger.MoveTo(Player.Movement.transform.localPosition + Vector3.forward * 0.746f);
    }
}

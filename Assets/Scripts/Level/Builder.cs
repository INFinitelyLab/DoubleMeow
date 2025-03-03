using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

using Random = UnityEngine.Random;

public class Builder : MonoBehaviour
{
    [SerializeField] private List<Building> _prefabs;
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private Building _startBuilding;
    [SerializeField] private Building _vehicle;
    [SerializeField] private float _vehiclePropastWidth;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private RegenTrigger _trigger;
    [SerializeField] private Obstacler _obstacler;
    [SerializeField] private Metroer _metroer;
    [SerializeField] private float _buildingWidth;
    [SerializeField] private int _startCount;

    [Space, Header("Special for xAxIxRx"), Space]
    [SerializeField] private float _distanceForBuildings;
    [SerializeField] private float _distanceForDecoBuildings;

    private string _currentIdentityName;
    private float _decoEndPoint;
    private float _totalVehicleWidth;

    private float _spawnPositionZ;

    private int _currentIdentityCount;

    private Building _pastBuilding;
    private Building _nextBuilding;
    private Building _currentBuilding;

    private Transform _player;

    private RoadLine _endLine = RoadLine.Venus;
    private RoadLine _nextEndLine = RoadLine.Venus;

    private List<Building> _pool = new List<Building>();


    private void Start()
    {
        _player = Player.Movement.transform;

        _nextBuilding = _startBuilding;

        GenerateFor(_startCount, true);

        Regenerate(false);

        StartCoroutine(MetroLoop());
    }


    private void OnEnable()
    {
        _trigger.Triggered += Regenerate;
    }


    private void OnDisable()
    {
        _trigger.Triggered -= Regenerate;
    }


    private void Regenerate() => Regenerate(false);

    private void Regenerate(bool isStartTile)
    {
        float currentPlayerPosition = Player.Movement.transform.position.z;
        float lastBuildingPosition = _pool.Last().transform.position.z;

        while ( currentPlayerPosition + _distanceForDecoBuildings > lastBuildingPosition )
        {
            _pastBuilding = _currentBuilding;
            _currentBuilding = Portal.IsWaitingForSecondPortal? _startBuilding : _nextBuilding;
    
            _nextEndLine = _currentBuilding.EndLines.ToRoadLine().Random();

            _nextBuilding = PickRandomBuilding(_nextEndLine , _currentBuilding == _vehicle, _currentBuilding.IsCanConnectToVehicle? null : _vehicle);

            if(Portal.IsWaitingForSecondPortal)
                CreateNewBuilding(_currentBuilding, isStartTile, _pool.Last().EndPoint.transform.position.z + (3 * Game.Difficulty));
            else
                CreateNewBuilding(_currentBuilding, isStartTile);

            lastBuildingPosition = _pool.Last().transform.position.z;
        }

        _trigger.MoveTo( _trigger.transform.position.z + 2 );


    }


    private void GenerateFor(int count, bool isStartBuilding)
    {
        for( int index = 0; index < count; index++ )
        {
            _pastBuilding = _currentBuilding;
            _currentBuilding = Portal.IsWaitingForSecondPortal ? _startBuilding : _nextBuilding;

            _nextEndLine = _currentBuilding.EndLines.ToRoadLine().Random();

            _nextBuilding = PickRandomBuilding(_nextEndLine, _currentBuilding == _vehicle, _currentBuilding.IsCanConnectToVehicle ? null : _vehicle);

            if (Portal.IsWaitingForSecondPortal)
                CreateNewBuilding(_currentBuilding, isStartBuilding, _pool.Last().EndPoint.transform.position.z + (3 * Game.Difficulty));
            else
                CreateNewBuilding(_currentBuilding, isStartBuilding);
        }
    }


    private Building CreateNewBuilding(Building origin, bool isStartBuilding, float fromZ = 0 )
    {

        Building building = Instantiate( origin );

        building.transform.SetParent(transform);

        if (isStartBuilding == false)
        {
            _endLine = _obstacler.Generate(building, _endLine, _nextEndLine);
        }

        if (_pool.Count > 0)
        {
            if (origin == _vehicle) building.transform.localPosition = Vector3.forward * ((fromZ == 0? _pool.Last().EndPoint.position.z : fromZ) + (_vehiclePropastWidth * Game.Difficulty));
            else

            building.transform.localPosition = Vector3.forward * ((fromZ == 0?_pool.Last().EndPoint.position.z : fromZ) + (_currentBuilding.IsNearly || _pastBuilding.IsNearly? 0 : _buildingWidth * Game.Difficulty));
        }
        else
        {
            building.transform.localPosition = _startPoint.localPosition;
        }

        if (isStartBuilding == false)
        {
            building.Animate();

            StartCoroutine(WaitForActive(building.transform));
        }

        if (origin != _vehicle)
        {
            _decoEndPoint = Building.PlaceDecorations(_decoEndPoint, building.transform.position.z , building.EndPoint.localPosition.z, _decoPrefabs, (_pastBuilding == _vehicle ? 2 : 0) + (_nextBuilding == _vehicle ? 1 : 0));

            _totalVehicleWidth = 0;
        }
        else
        {
            _decoEndPoint = 0;

            _totalVehicleWidth += _vehicle.EndPoint.localPosition.z + _vehiclePropastWidth;
        }

        _pool.Add( building );

        return building;
    }


    private void DeleteLastBuilding()
    {
        if (_pool.Count == 0) return;

        Destroy(_pool.Last().gameObject);

        _pool.RemoveAt( _pool.Count );
    }


    private Building PickRandomBuilding(RoadLine startLine, bool exceptVehiclephobia, Building except = null, Building except2 = null)
    {
        if (_pool.Count == 0 || Portal.IsWaitingForSecondPortal)
            return _startBuilding;
        else
        {
            List<Building> localPool = new List<Building>();

            if (_pool.Last().IsCanConnectToVehicle == true && Random.Range(0, 3) == 1 && except != _vehicle && except2 != _vehicle)
            {
                if (_currentIdentityName == _vehicle.name)
                {
                    _currentIdentityCount++;

                    if (_currentIdentityCount >= _vehicle.MaxIdentityCount) 
                        _currentIdentityCount = 0;
                    else 
                        return _vehicle;
                }
                else
                {
                    _currentIdentityName = _vehicle.name;
                    _currentIdentityCount = 0;

                    return _vehicle;
                }
            }

            List<RoadLine> endLines = _nextBuilding.StartLines.ToRoadLine();

            foreach( Building building in _prefabs )
            {
                List<RoadLine> startLines = building.StartLines.ToRoadLine();

                if (startLines.Contains(startLine) && except != building && except2 != building)
                {
                    if (exceptVehiclephobia == false ? true : (building.IsCanConnectToVehicle == true))
                    {
                        if (_currentIdentityName != building.name || _currentIdentityCount < building.MaxIdentityCount) localPool.Add(building);
                    }
                }
            }

            Building toReturn = localPool.Random();

            _currentIdentityCount = _currentIdentityName == toReturn.name ? _currentIdentityCount + 1 : 1;
            _currentIdentityName = toReturn.name;

            return toReturn;
        }
    }


    private void EnableMetroGeneration()
    {
        _trigger.Triggered -= Regenerate;

        if (_pool.Last().IsCanConnectToVehicle == false) CreateNewBuilding(PickRandomBuilding(_endLine, true, _vehicle), false);
        if (Portal.IsWaitingForSecondPortal) CreateNewBuilding(_startBuilding, false);

        if( _currentIdentityName != _vehicle.name )
        {
            _currentIdentityCount = 0;
            _currentIdentityName = _vehicle.name;
        }

        while( _currentIdentityCount < 3 )
        {
            CreateNewBuilding(_vehicle, false);

            _currentIdentityCount++;
        }

        _metroer.Spawn( _pool.Last().EndPoint.position.z + _buildingWidth, DisableMetroGeneration );

        Building.PlaceDecorations(0, _pool.Last().EndPoint.position.z + _buildingWidth, 15, _decoPrefabs, 3);
    }


    public void DisableMetroGeneration(float fromZ)
    {
        _trigger.Triggered += Regenerate;

        CreateNewBuilding( _vehicle, false, fromZ );
        CreateNewBuilding(PickRandomBuilding(_endLine, true, _vehicle), false);

        Fog.Instance.gameObject.SetActive(true);

        Player.Presenter.DisableCurvatization();
    }


    private IEnumerator MetroLoop()
    {
        yield return new WaitForSeconds(60);

        while(Game.IsActive)
        {
            EnableMetroGeneration();

            yield return new WaitForSeconds(120);
        }
    }


    private IEnumerator WaitForActive(Transform building)
    {
        bool isActive = false;

        while( isActive == false )
        {
            if ( Mathf.Abs(_player.position.z - building.position.z) < _distanceForBuildings )
            {
                isActive = true;

                building.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
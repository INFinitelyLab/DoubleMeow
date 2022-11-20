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
    [SerializeField] private DecorationBuilding _decoTurnPrefab;
    [SerializeField] private Building _startBuilding;
    [SerializeField] private Building _bridge;
    [SerializeField] private Building _vehicle;
    [SerializeField] private Building _buildingWhoTurnLeft;
    [SerializeField] private Building _buildingWhoTurnRight;
    [SerializeField] private float _vehiclePropastWidth;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private RegenTrigger _trigger;
    [SerializeField] private Obstacler _obstacler;
    [SerializeField] private Metroer _metroer;
    [SerializeField] private Curver _curver;
    [SerializeField] private Tiler _tiler;
    [SerializeField] private float _buildingWidth;
    [SerializeField] private int _startCount;

    [Space, Header("Special for xAxIxRx"), Space]
    [SerializeField] private float _distanceForBuildings;
    [SerializeField] private float _distanceForDecoBuildings;

    private Quaternion _rotation;

    private string _currentIdentityName;
    private float _decoEndPoint;

    private int _height;
    private int _pastHeight;
    private int _nextHeight;

    private int _nextErIndex;

    private bool _isNeedToEnableMetroMode;
    private bool _isNeedToEnableTilerMode;
    private bool _isNeedToEnableCurveMode;

    private bool _isNeedToTurn;


    private float _totalVehicleWidth;

    private int _currentIdentityCount;

    private Building _pastBuilding;
    private bool _nextBuildingIsVehicle;
    private bool _pastBuildingIsVehicle;
    private Building _currentBuilding;

    private Transform _player;

    private RoadLine _endLine = RoadLine.Venus;

    private Building _lastBuilding;
    private bool IsCanPlaceEr => _isNeedToEnableCurveMode == false && _isNeedToEnableMetroMode == false && _isNeedToEnableTilerMode == false;

    
    public void Rotate(bool isLeft)
    {
        _rotation.eulerAngles += new Vector3(0, isLeft? -90 : 90, 0);
    }


    private void Start()
    {
        _player = Player.Movement.transform;

        _currentBuilding = _startBuilding;
        _nextBuildingIsVehicle = false;

        CreateNewBuilding(_startBuilding, true);

        Regenerate(false);

        StartCoroutine(ErLoop());
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
        Vector3 currentPlayerPosition = Player.Movement.transform.position;
        Vector3 lastBuildingPosition = _lastBuilding.transform.position;

        while ( Vector3.Distance(currentPlayerPosition, lastBuildingPosition) < _distanceForDecoBuildings - _totalVehicleWidth)
        {

            _pastBuilding = _currentBuilding;
            _currentBuilding = Portal.IsWaitingForSecondPortal ? _startBuilding : (_nextBuildingIsVehicle? _vehicle : PickRandomBuilding(_endLine, _nextBuildingIsVehicle, _vehicle));

            _nextBuildingIsVehicle = GetVehicleIfRandom();

            if (_currentBuilding != _vehicle && _pastBuilding != _vehicle && Portal.IsWaitingForSecondPortal == false)
            {
                if ( _isNeedToTurn == false && IsCanPlaceEr == true)
                {
                    _pastHeight = _height;
                    _height = _nextHeight;
                    _nextHeight = _height == 1 || _height == -1 ? 0 : (Random.Range(0, 2) == 0 ? 1 : -1);
                }
                else
                {
                    _pastHeight = _height;
                    _height = _nextHeight;
                    _nextHeight = 0;
                }
            }
            else
            {
                _nextHeight = _height;
            }

            if (Portal.IsWaitingForSecondPortal)
            {
                CreateNewBuilding(_currentBuilding, isStartTile, new Vector3(_lastBuilding.EndPoint.position.x, 0, _lastBuilding.EndPoint.position.z) + _rotation * (Vector3.forward * (3 * Game.Difficulty)));
            }
            else
            {
                CreateNewBuilding(_currentBuilding, isStartTile);

                if (_currentBuilding == _vehicle)
                {
                    _pastBuilding = _currentBuilding;
                    _pastBuildingIsVehicle = true;
                    _currentBuilding = PickRandomBuilding(_endLine, true, _vehicle);

                    CreateNewBuilding(_currentBuilding, false);
                }
                else
                {
                    _pastBuildingIsVehicle = false;
                }
            }

            lastBuildingPosition = _lastBuilding.transform.position;

            if (IsCanPlaceTurn())
            {
                EnableTurn();
                DisableTurn();
            }
            else if (IsCanPlaceMetro())
            {
                EnableMetroGeneration();
             
                break;
            }
            else if( IsCanPlaceTiler() )
            {
                EnableTilerGeneration();

                break;
            }
            else if( IsCanPlaceCurve() )
            {
                EnableCurveGeneration();

                break;
            }
            else if( IsCanPlaceBridge() )
            {
                EnableBridge();
                DisableBridge();
            }
        }

        _trigger.MoveTo( Player.Movement.transform.position + Player.Movement.transform.rotation * (Vector3.forward * 3));
    }


    private Building CreateNewBuilding(Building origin, bool isStartBuilding, Vector3 from = default, bool isNeedToRise = false )
    {
        Building building = Instantiate( origin );


        if (isStartBuilding == false && origin != _vehicle)
        {
            _endLine = _obstacler.Generate(building, _endLine, (_height < _nextHeight && _nextBuildingIsVehicle == false) || isNeedToRise , origin == _bridge);
        }

        if (isStartBuilding == false)
        {
            building.transform.localRotation = _rotation;

            if (origin == _vehicle)
                building.transform.localPosition = (from == default? new Vector3(_lastBuilding.EndPoint.position.x, 0, _lastBuilding.EndPoint.position.z) : from) + (_rotation * Vector3.forward * ((_vehiclePropastWidth + 1) * Game.Difficulty - 1)) + Vector3.up * _height * 1f;
            else
                building.transform.localPosition = (from == default ? new Vector3(_lastBuilding.EndPoint.position.x, 0, _lastBuilding.EndPoint.position.z) : from) + (_rotation * Vector3.forward * ((_currentBuilding.IsNearly || _pastBuilding.IsNearly) ? 0 : (((_buildingWidth + 1 + building.PlaceOffset) * Game.Difficulty) - 1))) + Vector3.up * _height * 1f;
        }
        else
        {
            building.transform.localPosition = _startPoint.localPosition;
        }

        if (isStartBuilding == false)
        {
            building.Animate();

            if (_pastBuildingIsVehicle == false)
            {
                StartCoroutine(WaitForActive(building.transform));
            }
            else
            {
                building.gameObject.SetActive(true);
            }
        }

        if (origin != _vehicle)
        {
            if (origin != _buildingWhoTurnLeft && origin != _buildingWhoTurnRight)
                _decoEndPoint = Building.PlaceDecorations(_height, _decoEndPoint, building.transform.position, _rotation, building.EndPoint.localPosition.z, _decoPrefabs, (_pastBuilding == _vehicle ? 2 : 0) + (_nextBuildingIsVehicle ? 1 : 0));
            else
                _decoEndPoint = Building.PlaceDecorationsForTurn(_height, _decoEndPoint, building.GetComponentsInChildren<Turner>()[0], building.transform.position, _rotation, _decoPrefabs, _decoTurnPrefab, origin == _buildingWhoTurnLeft );

            _totalVehicleWidth = 0;
        }
        else
        {
            _decoEndPoint = 0;

            _totalVehicleWidth += _vehicle.EndPoint.localPosition.z + _vehiclePropastWidth;
        }

        _lastBuilding = building;

        return building;
    }


    private Building PickRandomBuilding(RoadLine startLine, bool exceptVehiclephobia, Building except = null, Building except2 = null)
    {
        if (Portal.IsWaitingForSecondPortal)
            return _startBuilding;
        else
        {
            List<Building> localPool = new List<Building>();

            //List<RoadLine> endLines = _nextBuilding.StartLines.ToRoadLine();

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


    private bool GetVehicleIfRandom()
    {
        if (Portal.IsWaitingForSecondPortal)
            return false;

        if (_lastBuilding.IsCanConnectToVehicle == true && Random.Range(0, 3) == 1)
        {
            if (_currentIdentityName == _vehicle.name)
            {
                _currentIdentityCount++;

                if (_currentIdentityCount >= _vehicle.MaxIdentityCount)
                    _currentIdentityCount = 0;
                else
                    return true;
            }
            else
            {
                _currentIdentityName = _vehicle.name;
                _currentIdentityCount = 0;

                return true;
            }
        }

        return false;
    }



    private void EnableMetroGeneration()
    {
        _trigger.Triggered -= Regenerate;

        _height = 0;
        _nextHeight = 0;

        for (int index = 0; index < 3; index++)
        {
            CreateNewBuilding(_vehicle, false);
        }

        _metroer.Spawn( _lastBuilding.EndPoint.position + _rotation * Vector3.forward * _buildingWidth, _rotation , DisableMetroGeneration );

        Building.PlaceDecorations(_height, 0, _lastBuilding.EndPoint.position + _rotation * Vector3.forward * _buildingWidth, _rotation, 15, _decoPrefabs, 3);
    }

    public void DisableMetroGeneration(Vector3 from)
    {
        _trigger.Triggered += Regenerate;

        _endLine = RoadLine.Venus;
        _height = 0;
        _nextHeight = 0;

        _isNeedToEnableMetroMode = false;

        CreateNewBuilding( _vehicle, false, from );
        CreateNewBuilding(_startBuilding, false);

        Player.Presenter.DisableCurvatization();

        Fog.Instance.gameObject.SetActive(true);
    }

    public bool IsCanPlaceMetro()
    {
        return (_isNeedToEnableMetroMode && _height == 0 && _nextHeight == _height && _currentBuilding != _vehicle && Portal.IsWaitingForSecondPortal == false);
    }



    public void EnableCurveGeneration()
    {
        _trigger.Triggered -= Regenerate;

        _height = 0;
        _nextHeight = 0;

        _curver.Spawn(DisableCurveGeneration, _decoEndPoint, _nextBuildingIsVehicle, _lastBuilding.EndPoint.position + (_rotation * Vector3.forward * (((_buildingWidth - 1) * Game.Difficulty) + 1)), _rotation);
    }

    public void DisableCurveGeneration(Vector3 from)
    {
        _trigger.Triggered += Regenerate;

        _endLine = RoadLine.Venus;
        _height = 0;
        _nextHeight = 0;

        _isNeedToEnableCurveMode = false;

        CreateNewBuilding(_startBuilding, false, from);

        Fog.Instance.gameObject.SetActive(true);
    }

    public bool IsCanPlaceTiler()
    {
        return (_isNeedToEnableTilerMode && _nextHeight == _height && _height == 0 && _currentBuilding == _startBuilding && Portal.IsWaitingForSecondPortal == false);
    }



    public void EnableTilerGeneration()
    {
        _trigger.Triggered -= Regenerate;

        _height = 0;
        _nextHeight = 0;

        _tiler.Spawn(_lastBuilding.EndPoint.position, _rotation, DisableTilerGeneration, _decoEndPoint, _endLine);
    }

    public void DisableTilerGeneration(Vector3 from, RoadLine line)
    {
        _trigger.Triggered += Regenerate;

        _endLine = line;
        _height = 0;
        _nextHeight = 0;

        _decoEndPoint = 0;

        _isNeedToEnableTilerMode = false;

        CreateNewBuilding(_startBuilding, false, from - _rotation * Vector3.forward * (_currentBuilding.IsNearly || _pastBuilding.IsNearly ? 0 : (((_buildingWidth + 1 + _startBuilding.PlaceOffset) * Game.Difficulty) - 1)) );
    }

    public bool IsCanPlaceCurve()
    {
        return (_isNeedToEnableCurveMode && _height == 0 && _nextHeight == 0 && Portal.IsWaitingForSecondPortal == false);
    }


    public void EnableBridge()
    {
        _currentBuilding = _bridge;

        CreateNewBuilding( _bridge, false );

        _pastBuildingIsVehicle = false;
    }

    public void DisableBridge()
    {
        _pastBuilding = _bridge;
        _currentBuilding = _startBuilding;

        CreateNewBuilding( _startBuilding, false );
    }

    public bool IsCanPlaceBridge()
    {
        return (Portal.IsWaitingForSecondPortal == false && _currentBuilding == _startBuilding && _height < 1 && _nextHeight == _height );
    }


    public void EnableTurn()
    {
        bool isLeft = Random.Range(0, 2) == 0;

        CreateNewBuilding( isLeft? _buildingWhoTurnLeft : _buildingWhoTurnRight, false );
        Rotate( isLeft );
    }

    public void DisableTurn()
    {
        _isNeedToTurn = false;
    }

    public bool IsCanPlaceTurn()
    {
        return ( Portal.IsWaitingForSecondPortal == false && _height == _nextHeight && _isNeedToTurn );
    }


    private void StartNextEr()
    {
        switch( _nextErIndex )
        {
            case 0:
                _isNeedToEnableTilerMode = true;
                break;
            case 1:
                _isNeedToEnableCurveMode = true;
                break;
            case 2:
                _isNeedToEnableMetroMode = true;
                break;
            default: throw new Exception("Неверный индекс допольнительного режима");
        }

        _nextErIndex = _nextErIndex == 2 ? 0 : (_nextErIndex + 1);
    }


    private IEnumerator ErLoop()
    {
        yield return new WaitForSeconds(5);

        _nextErIndex = Random.Range(0,3);


        while(Game.IsActive)
        {
            StartNextEr();

            yield return new WaitUntil(() => IsCanPlaceEr == true);
            yield return new WaitForSeconds(10 * Game.Difficulty);

            StartNextEr();

            yield return new WaitUntil(() => IsCanPlaceEr == true);
            yield return new WaitForSeconds(10 * Game.Difficulty);

            _isNeedToTurn = true;

            yield return new WaitUntil(() => _isNeedToTurn == false);
            yield return new WaitForSeconds(10 * Game.Difficulty);
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
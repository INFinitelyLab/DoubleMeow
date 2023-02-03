using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

using Random = UnityEngine.Random;

public class Builder : SingleBehaviour<Builder>
{
    [SerializeField] private List<Building> _prefabs;
    [SerializeField] private List<DecorationBuilding> _decoPrefabs;
    [SerializeField] private DecorationBuilding _decoTurnPrefab;
    [SerializeField] private Building _startBuilding;
    [SerializeField] private Building _realStartBuilding;
    [SerializeField] private Building _bridge;
    [SerializeField] private Building _vehicle;
    [SerializeField] private Building _train;
    [SerializeField] private Building _buildingWhoTurnLeft;
    [SerializeField] private Building _buildingWhoTurnRight;
    [SerializeField] private Building _upper;
    [SerializeField] private LoseTrigger _loseTrigger;
    [SerializeField] private float _vehiclePropastWidth;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private RegenTrigger _trigger;
    [SerializeField] private Obstacler _obstacler;
    [SerializeField] private Metroer _metroer;
    [SerializeField] private Curver _curver;
    [SerializeField] private Tiler _tiler;
    [SerializeField] private Retrowaver _retrowaver;
    [SerializeField] private float _buildingWidth;
    [SerializeField] private int _startCount;

    [Space, Header("Special for xAxIxRx"), Space]
    [SerializeField] private float _distanceForBuildings;
    [SerializeField] private float _distanceForDecoBuildings;

    public Metroer Metroer => _metroer;
    public Curver Curver => _curver;
    public Tiler Tiler => _tiler;
    public Retrowaver Retroer => _retrowaver;

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
    private bool _isNeedToEnableRetroMode;

    private bool _isNeedToTurn;

    private Vector3 _lastBuildingEndPosition;
    private Vector3 _lastBuildingPosition;

    private float _totalVehicleWidth;

    private int _currentIdentityCount;

    private Building _pastBuilding;
    private bool _nextBuildingIsVehicle;
    private bool _pastBuildingIsVehicle;
    private Building _currentBuilding;

    private Transform _player;

    private RoadLine _endLine = RoadLine.Venus;

    private Building _lastBuilding;
    
    public bool IsCanPlaceEr => _isNeedToEnableCurveMode == false && _isNeedToEnableMetroMode == false && _isNeedToEnableTilerMode == false && _isNeedToEnableRetroMode == false;
    public Quaternion Rotation => _rotation;

    private List<Building> _buildingAfterHulkPickup;
    private bool _isNeedToRecordBuildings = false;

    
    public void Rotate(bool isLeft)
    {
        _rotation.eulerAngles += new Vector3(0, isLeft? -90 : 90, 0);

        Game.OnTurn();
    }


    public void CreateBuildingFrom(Vector3 position, Quaternion rotation)
    {
        Portal.Reset();

        position.y = 0;

        _rotation = rotation;

        _height = 0;
        _nextHeight = 0;
        _nextBuildingIsVehicle = false;

        CreateNewBuilding(_startBuilding, true, position);

        _trigger.MoveTo(position + _rotation * Vector3.forward * 5);

        Regenerate();
    }


    public void Initialize()
    {
        _player = Player.Movement.transform;

        _currentBuilding = _startBuilding;
        _nextBuildingIsVehicle = false;

        CreateNewBuilding(_realStartBuilding, isStartBuilding: true);

        Regenerate(false);

        StartCoroutine(ErLoop());
    }


    protected override void OnActive()
    {
        if (Game.InMiniGames) return;

        if( Metroer.IsEnabled == false && Curver.IsEnabled == false && Tiler.IsEnabled == false && Retroer.IsEnabled == false)

        _trigger.Triggered += Regenerate;
    }


    protected override void OnDisactive()
    {
        if (Game.InMiniGames) return;

        _trigger.Triggered -= Regenerate;
    }


    private void Regenerate() => Regenerate(false);

    private void Regenerate(bool isStartTile)
    {
        if (isActiveAndEnabled == false) return;

        Vector3 currentPlayerPosition = Player.Movement.transform.position;
        Vector3 lastBuildingPosition = _lastBuildingPosition;

        while ( -Player.Movement.GetDistanceTo(lastBuildingPosition) < _distanceForDecoBuildings)
        {
            _pastBuilding = _currentBuilding;
            _currentBuilding = Portal.IsWaitingForSecondPortal ? _startBuilding : (_nextBuildingIsVehicle? (Random.Range(0,2) == 1? _vehicle : _train) : PickRandomBuilding(_endLine, _nextBuildingIsVehicle, _vehicle));

            _nextBuildingIsVehicle = (Game.Mode.InParachuteMode || Game.Mode.InxAxIxRxMode? false : GetVehicleIfRandom());

            if (_currentBuilding != _train && _pastBuilding != _train && _currentBuilding != _vehicle && _pastBuilding != _vehicle && Portal.IsWaitingForSecondPortal == false)
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

                if (_currentBuilding == _vehicle || _currentBuilding == _train)
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
            if (IsCanPlaceMetro())
            {
                EnableMetroGeneration();
            }
            if (IsCanPlaceTiler())
            {
                EnableTilerGeneration();
            }
            if (IsCanPlaceCurve())
            {
                EnableCurveGeneration();
            }
            if (IsCanPlaceRetro())
            {
                EnableRetroMode();
            }
            if (IsCanPlaceBridge())
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


        if (isStartBuilding == false || from != default)
        {
            building.transform.localRotation = _rotation;

            if (origin == _vehicle || origin == _train)
                building.transform.position = (from == default? new Vector3(_lastBuildingEndPosition.x, 0, _lastBuildingEndPosition.z) : from) + (_rotation * Vector3.forward * ((_vehiclePropastWidth + 1) * Game.Difficulty - 1)) + Vector3.up * _height * 1f;
            else
                building.transform.position = (from == default ? new Vector3(_lastBuildingEndPosition.x, 0, _lastBuildingEndPosition.z) : from) + (_rotation * Vector3.forward * ((_currentBuilding.IsNearly || _pastBuilding.IsNearly) ? 0 : (((_buildingWidth + 1 + building.PlaceOffset) * Game.Difficulty) - 1))) + Vector3.up * _height * 1f;
        }
        else
        {
            building.transform.position = _startPoint.position;
        }

        _lastBuilding = building;

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

        if (origin != _vehicle && origin != _train)
        {
            if (origin != _buildingWhoTurnLeft && origin != _buildingWhoTurnRight)
                _decoEndPoint = Building.PlaceDecorations(_height, _decoEndPoint, building.transform.position, _rotation, building.EndPoint.localPosition.z, _decoPrefabs, (_pastBuilding == _vehicle || _pastBuilding == _train? 2 : 0) + (_nextBuildingIsVehicle ? 1 : 0));
            else
                _decoEndPoint = Building.PlaceDecorationsForTurn(_height, _decoEndPoint, building.GetComponentsInChildren<Turner>()[0], building.transform.position, _rotation, _decoPrefabs, _decoTurnPrefab, origin == _buildingWhoTurnLeft );

            _totalVehicleWidth = 0;
        }
        else
        {
            _decoEndPoint = 0;

            _totalVehicleWidth += _vehicle.EndPoint.localPosition.z + _vehiclePropastWidth;
        }

        if (_isNeedToRecordBuildings)
        {
            if (_buildingAfterHulkPickup.Count > 9)
                _isNeedToRecordBuildings = false;
            else
                _buildingAfterHulkPickup.Add( building );
        }

        CreateLoseTrigger(_lastBuildingEndPosition, building.transform.position, building.transform);

        _lastBuildingPosition = building.transform.position;
        _lastBuildingEndPosition = building.EndPoint.position;

        if (isStartBuilding == false && origin != _vehicle && origin != _train && origin != _upper)
        {
            _endLine = _obstacler.Generate(building, _endLine, origin == _bridge);

            if (((_height < _nextHeight && _nextBuildingIsVehicle == false) || isNeedToRise) && origin != _upper && Portal.IsWaitingForSecondPortal == false)
            {
                Building upper = Instantiate(_upper);

                upper.transform.position = new Vector3(building.EndPoint.position.x, _height, building.EndPoint.position.z) + _rotation * Vector3.right * (int)_endLine * 0.746f;
                upper.transform.rotation = _rotation;

                _lastBuilding = upper;
                _currentBuilding = upper;
                _lastBuildingEndPosition = upper.EndPoint.position + _rotation * Vector3.left * (int)_endLine * 0.746f;

                Vector3 endPointLocalPosition = upper.transform.GetChild(1).localPosition;
                endPointLocalPosition.x = (int)_endLine * -0.746f;
                upper.transform.GetChild(1).localPosition = endPointLocalPosition;
            }
        }

        return building;
    }


    public void CreateLoseTrigger(Vector3 startPosition, Vector3 endPosition, Transform parent)
    {
        Vector3 losePosition = (startPosition + endPosition) / 2;
        Vector3 loseSize = new Vector3(3, 3, Vector3.Distance(startPosition, endPosition));

        losePosition.y = Mathf.Min(startPosition.y, endPosition.y) - 1.5f;

        LoseTrigger trigger = Instantiate(_loseTrigger, parent);

        trigger.transform.rotation = _rotation;
        trigger.transform.position = losePosition;
        trigger.transform.localScale = loseSize;
    }


    public void DeleteAllRegeneratePoints()
    {
        RegeneratePoint[] points = FindObjectsOfType<RegeneratePoint>();

        foreach(RegeneratePoint point in points)
        {
            Destroy(point.gameObject);
        }
    }


    public void DestroyAllTrash(bool includeBuildings)
    {
        List<Component> components = new List<Component>(50);

        components.AddRange(FindObjectsOfType<Tile>(true));
        components.AddRange(FindObjectsOfType<Trashable>(true).Where( t => t.PhaseOnCreate < Game.Phase ));
        components.AddRange(FindObjectsOfType<RegeneratePoint>(true));

        if (includeBuildings)
        {
            components.AddRange(FindObjectsOfType<Milk>(true));
            components.AddRange(FindObjectsOfType<Building>(true));
            components.AddRange(FindObjectsOfType<Trashable>(true));
            components.AddRange(FindObjectsOfType<DecorationBuilding>(true));
        }
        else
        {
            components.AddRange(FindObjectsOfType<Trashable>(true).Where(t => t.PhaseOnCreate < Game.Phase));
        }

        foreach (Component component in components)
        {
            Destroy(component.gameObject);
        }
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


    public void DisableAllObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>(true);

        foreach(Obstacle obstacle in obstacles)
        {
            obstacle.DisableCollision();
        }
    }

    public void EnableAllObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>(true);

        foreach(Obstacle obstacle in obstacles)
        {
            obstacle.EnableCollision();
        }
    }


    public void EnableMetroGeneration()
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

        _isNeedToEnableMetroMode = false;

        if (from == default)
            return;

        _endLine = RoadLine.Venus;
        _height = 0;
        _nextHeight = 0;

        CreateNewBuilding( _vehicle, false, from );
        CreateNewBuilding(_startBuilding, false);

        Player.Presenter.DisableCurvatization();

        Fog.Instance.gameObject.SetActive(true);
    }

    public bool IsCanPlaceMetro()
    {
        return (Game.Mode.InxAxIxRxMode == false && Rocket.IsAlreadyExist == false && _isNeedToEnableMetroMode && _height == 0 && _nextHeight == _height && _currentBuilding != _vehicle && Portal.IsWaitingForSecondPortal == false);
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

        _isNeedToEnableCurveMode = false;

        if (from == default)
            return;

        _endLine = RoadLine.Venus;
        _height = 0;
        _nextHeight = 0;

        CreateNewBuilding(_startBuilding, false, from);

        Fog.Instance.gameObject.SetActive(true);
    }

    public bool IsCanPlaceTiler()
    {
        return (Game.Mode.InxAxIxRxMode == false && Rocket.IsAlreadyExist == false && _isNeedToEnableTilerMode && _nextHeight == _height && _height == 0 && _currentBuilding == _startBuilding && Portal.IsWaitingForSecondPortal == false);
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
        return (Game.Mode.InxAxIxRxMode == false && Rocket.IsAlreadyExist == false && _isNeedToEnableCurveMode && _height == 0 && _nextHeight == 0 && Portal.IsWaitingForSecondPortal == false);
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
        return (Portal.IsWaitingForSecondPortal == false && _currentBuilding == _startBuilding && _height < 1 && _nextHeight == _height && IsCanPlaceEr == true);
    }


    public void EnableTurn()
    {
        bool isLeft = Random.Range(0, 2) == 0;

        _lastBuilding = CreateNewBuilding( isLeft? _buildingWhoTurnLeft : _buildingWhoTurnRight, false );
        Rotate( isLeft );
    }

    public void DisableTurn()
    {
        _isNeedToTurn = false;
    }

    public bool IsCanPlaceTurn()
    {
        return (Game.Mode.InxAxIxRxMode == false && Rocket.IsAlreadyExist == false && Portal.IsWaitingForSecondPortal == false && _height == _nextHeight && _isNeedToTurn );
    }


    public void EnableRetroMode()
    {
        _trigger.Triggered -= Regenerate;

        _height = 0;
        _nextHeight = 0;

        _retrowaver.Spawn(DisableRetroMode, _endLine, _lastBuilding.EndPoint.position + (_rotation * Vector3.forward * (((_buildingWidth - 1) * Game.Difficulty) + 1)), _rotation);
    }

    public void DisableRetroMode(Vector3 from, RoadLine line)
    {
        _trigger.Triggered += Regenerate;

        _endLine = line;
        _height = 0;
        _nextHeight = 0;

        _decoEndPoint = 0;

        _isNeedToEnableRetroMode = false;

        CreateNewBuilding(_startBuilding, false, from);

    }

    public bool IsCanPlaceRetro()
    {
        return (Game.Mode.InxAxIxRxMode == false && Rocket.IsAlreadyExist == false && _isNeedToEnableRetroMode && _height == 0 && _nextHeight == 0 && Portal.IsWaitingForSecondPortal == false);
    }



    public void PrepareHulkGeneration()
    {
        _buildingAfterHulkPickup = new List<Building>(10);

        _isNeedToRecordBuildings = true;
    }



    private void StartNextEr()
    {
        switch( _nextErIndex )
        {
            case 0:
                _isNeedToEnableCurveMode = true;
                break;
            case 1:
                _isNeedToEnableMetroMode = true;
                break;
            case 2:
                _isNeedToEnableTilerMode = true;
                break;
            case 3:
                _isNeedToEnableRetroMode = true;
                break;
            default: throw new Exception("Ќеверный индекс допольнительного режима");
        }

        _nextErIndex = _nextErIndex == 3 ? 0 : (_nextErIndex + 1);
    }


    private IEnumerator WaitForActive(Transform building)
    {
        bool isActive = false;

        while( isActive == false )
        {
            if (building == null) yield break;

            if ( Mathf.Abs(_player.position.z - building.position.z) < _distanceForBuildings )
            {
                isActive = true;

                building.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }



    // Er loop (€ хреновый архитектор кода, идите в жопу!)



    private float _erLoopTimer;
    private bool _erLoopTimerIsTicking = true;

    private IEnumerator ErLoop()
    {
        yield return new WaitUntil(() => IsCanPlaceEr == true);
        yield return new WaitForSeconds(5);

        _nextErIndex = 2; // Random.Range(0, 4);

        while (isActiveAndEnabled)
        {
            _isNeedToTurn = true;

            yield return new WaitUntil(() => _isNeedToTurn == false);
            yield return new WaitUntil(() => Drone.Instance.IsEnabled == false);
            yield return StartCoroutine(WaitTimer(10));

            StartNextEr();

            yield return new WaitUntil(() => IsCanPlaceEr == true);
            yield return new WaitUntil(() => Drone.Instance.IsEnabled == false);
            yield return StartCoroutine(WaitTimer(10));
        
            StartNextEr();

            yield return new WaitUntil(() => IsCanPlaceEr == true);
            yield return new WaitUntil(() => Drone.Instance.IsEnabled == false);
            yield return StartCoroutine(WaitTimer(10));
        }
    }

    private IEnumerator WaitTimer(float duration)
    {
        _erLoopTimer = duration;

        while (_erLoopTimer > 0 )
        {
            if (_erLoopTimerIsTicking == true) _erLoopTimer -= Time.deltaTime;

            yield return null;
        }
    }

    public void CancelErLoopTimer()
    {
        _erLoopTimerIsTicking = false;
    }

    public void ContinueErLoopTimer()
    {
        _erLoopTimerIsTicking = true;
    }
}
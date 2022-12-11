using UnityEngine;
using System.Collections;

public class Metroer : MonoBehaviour
{
    [SerializeField] private MetroTrigger _metroTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Building _metroStartPrefab;
    [SerializeField] private Building _metroPrefab;
    [SerializeField] private Milk _milk;
    [SerializeField] private int _milkCount;
    [SerializeField] private float _milkSurfIntensity;
    [SerializeField] private MetroVehicle _vehicle;
    [SerializeField] private float _targetMetroDistance;
    [SerializeField] private float _vehicleXOffset;
    [SerializeField] private float _distanceBetweenVehicles;
    [SerializeField] private float _generationDistance;

    private Vector3 _lineSpeeds;

    private Quaternion _rotation;
    private Vector3 _startPosition;
    private float _distance;
    private float _vehicleDistance;

    private Building _lastMetro;

    private Transform _player;

    private RoadLine _vehicleLine;
    private RoadLine _pastVehicleLine;

    public System.Action<Vector3> EndGenerate;


    public void Spawn(Vector3 from, Quaternion rotation, System.Action<Vector3> OnEndMethod)
    {
        from.y = 0;

        _player = Player.Movement.transform;

        _rotation = rotation;

        CreateNewBuilding(_metroStartPrefab, from, rotation);

        EndGenerate = OnEndMethod;

        _distance = 0;

        _lineSpeeds.x = Random.Range(0.5f, 1f);
        _lineSpeeds.y = Random.Range(0.5f, 1f);
        _lineSpeeds.z = Random.Range(0.5f, 1f);

        _vehicleDistance = 10;
        _regenTrigger.Triggered += Regenerate;
        _startPosition = from;
    }


    private Building CreateNewBuilding(Building origin, Vector3 position, Quaternion rotation)
    {
        Building metro = Instantiate(origin, position, rotation, transform);

        return metro;
    }


    private MetroVehicle CreateNewVehicle(MetroVehicle origin, Vector3 position, Quaternion rotation, RoadLine line)
    {
        MetroVehicle vehicle = Instantiate(origin, transform);

        vehicle.Body.gameObject.SetActive(false);

        StartCoroutine(WaitForActive(vehicle.Body.gameObject));

        vehicle.transform.localPosition = position;
        vehicle.transform.localRotation = rotation;

        vehicle.SetSpeedMultiplier( line == RoadLine.Mercury? _lineSpeeds.x : line == RoadLine.Venus? _lineSpeeds.y : _lineSpeeds.z );

        return vehicle;
    }


    private void CreateMilk(Vector3 position, Transform parent)
    {
        Instantiate(_milk, position, Quaternion.identity);
    }


    public void Regenerate()
    {
        float targetDistance = Vector3.Distance(_player.position, _startPosition) + _generationDistance;

        Building metro = null;

        while (_distance < targetDistance)
        {
            metro = CreateNewBuilding(_metroPrefab, _startPosition + _rotation * Vector3.forward * _distance, _rotation);

            _lastMetro = metro;

            _distance += _metroPrefab.EndPoint.transform.localPosition.z;
        }

        if (_distance + 5 < _targetMetroDistance)
        {
            while( _vehicleDistance < targetDistance )
            {
                    _pastVehicleLine = _vehicleLine;

                    int swipeID = _vehicleLine == RoadLine.Mercury? 0 : 1;

                    swipeID += _vehicleLine == RoadLine.Earth ? 0 : 2;

                    if (swipeID == 3)
                        _vehicleLine.TrySurfRandom();
                    else
                        _vehicleLine.TrySurf( swipeID == 1? Direction.Left : Direction.Right );

                    int rand = Random.Range(0, 4);
                    
                    if (_vehicleLine != RoadLine.Mercury && rand != 0) CreateNewVehicle(_vehicle, _startPosition + _rotation * new Vector3(-1 * _vehicleXOffset, 0, Random.Range(0f,0.75f) + _vehicleDistance + _distanceBetweenVehicles), _rotation, RoadLine.Mercury);
                    if (_vehicleLine != RoadLine.Venus && rand != 1) CreateNewVehicle(_vehicle, _startPosition + _rotation * new Vector3(0, 0, Random.Range(0f, 0.75f)+ _vehicleDistance + _distanceBetweenVehicles), _rotation, RoadLine.Venus);
                    if (_vehicleLine != RoadLine.Earth && rand != 2) CreateNewVehicle(_vehicle, _startPosition + _rotation * new Vector3(1 * _vehicleXOffset, 0, Random.Range(0f, 0.75f) + _vehicleDistance + _distanceBetweenVehicles), _rotation, RoadLine.Venus);

                Vector3 position = new Vector3((int)_pastVehicleLine * _vehicleXOffset, -0.5f, _vehicleDistance + (_distanceBetweenVehicles / 2));

                float stepIntensityZ = _distanceBetweenVehicles / _milkCount;

                for (int y = 0; y < _milkCount; y++)
                {
                    position.z += stepIntensityZ;
                    position.x = Mathf.MoveTowards( position.x, (int)_vehicleLine * _vehicleXOffset, _milkSurfIntensity / _milkCount);

                    CreateMilk(_startPosition + _rotation * position, _lastMetro.transform);
                }

                _vehicleDistance += _distanceBetweenVehicles;
            }
        }


        _regenTrigger.MoveTo( Player.Movement.transform.localPosition + _rotation * Vector3.forward * 2 * Game.Difficulty);

        if (_distance >= _targetMetroDistance )
        {
            MetroTrigger mT = Instantiate(_metroTrigger, metro.transform);

            mT.transform.localPosition = metro.EndPoint.localPosition + Vector3.back * 5f * Game.Difficulty;

            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke(_rotation * Vector3.forward * _distance + _startPosition);
        }
    }


    private IEnumerator WaitForActive(GameObject toActive)
    {
        yield return new WaitForFixedUpdate();

        bool isActive = false;

        while(isActive == false)
        {
            if (Mathf.Abs(_player.position.z - toActive.transform.position.z) < _generationDistance)
            {
                toActive.SetActive(true);
                isActive = true;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}